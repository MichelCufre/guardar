using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Parametrizacion;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PRE
{
    public class PAR400AtributoValidacionValor : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;

        public PAR400AtributoValidacionValor(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IFormValidationService formValidationService,
            ISecurityService security)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var listaAtributoValidacion = JsonConvert.DeserializeObject<List<AtributoValidacion>>(query.Parameters.FirstOrDefault(x => x.Id == "listValidaciones").Value);
                var atributo = uow.AtributoRepository.GetAtributoValidacion(listaAtributoValidacion.OrderBy(x => x.Id).FirstOrDefault().Id);
                var formaterDateTime = uow.ParametroRepository.GetParameter(ParamManager.DATETIME_FORMAT_DATE_SECONDS);

                query.Parameters.Add(new ComponentParameter { Id = "isURL", Value = "F" });
                query.Parameters.Add(new ComponentParameter { Id = "isTexto", Value = "F" });
                query.Parameters.Add(new ComponentParameter { Id = "isHora", Value = "F" });
                query.Parameters.Add(new ComponentParameter { Id = "isFecha", Value = "F" });
                query.Parameters.Add(new ComponentParameter { Id = "isNumerico", Value = "F" });
                query.Parameters.Add(new ComponentParameter { Id = "isDecimal", Value = "F" });

                var separador = uow.ParametroRepository.GetParameter(ParamManager.NUMBER_DECIMAL_SEPARATOR);

                foreach (var field in form.Fields)
                {
                    field.Value = "";
                }

                query.Parameters.RemoveAll(w => w.Id == "idValidacion");
                query.Parameters.RemoveAll(w => w.Id == "nmValidacion");

                query.AddParameter("idValidacion", atributo.Id.ToString());
                query.AddParameter("nmValidacion", atributo.NombreValidacion);

                query.Parameters.Add(new ComponentParameter { Id = "labeltxt", Value = atributo.NombreArgumento });

                switch (atributo.TipoArgumento)
                {
                    case TipoAtributoValidacionDb.NUMERICO:
                        query.Parameters.RemoveAll(w => w.Id == "isNumerico");
                        query.Parameters.Add(new ComponentParameter { Id = "isNumerico", Value = "T" });
                        break;
                    case TipoAtributoValidacionDb.FECHA:
                        query.Parameters.RemoveAll(w => w.Id == "isFecha");
                        query.Parameters.Add(new ComponentParameter { Id = "isFecha", Value = "T" });
                        break;
                    case TipoAtributoValidacionDb.HORA:
                        query.Parameters.RemoveAll(w => w.Id == "isHora");
                        query.Parameters.Add(new ComponentParameter { Id = "isHora", Value = "T" });
                        break;
                    case TipoAtributoValidacionDb.TEXTO:
                        query.Parameters.RemoveAll(w => w.Id == "isTexto");
                        query.Parameters.Add(new ComponentParameter { Id = "isTexto", Value = "T" });
                        break;
                    case TipoAtributoValidacionDb.URL:
                        query.Parameters.RemoveAll(w => w.Id == "isURL");
                        query.Parameters.Add(new ComponentParameter { Id = "isURL", Value = "T" });
                        break;
                    case TipoAtributoValidacionDb.DECIMAL:
                        query.Parameters.RemoveAll(w => w.Id == "isDecimal");
                        query.Parameters.Add(new ComponentParameter { Id = "isDecimal", Value = "T" });
                        break;
                }
            }

            return form;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();

            try
            {
                var listaAtributoValidacion = JsonConvert.DeserializeObject<List<AtributoValidacion>>(context.Parameters.FirstOrDefault(x => x.Id == "listValidaciones").Value);
                if (listaAtributoValidacion.Count > 0)
                {
                    var registro = listaAtributoValidacion.OrderBy(x => x.Id).FirstOrDefault();
                    var newAsociada = new AtributoValidacionAsociada();
                    int idAtributo = int.Parse(context.GetParameter("codigoAtributo"));

                    newAsociada.IdValidacion = registro.Id;
                    newAsociada.IdAtributo = idAtributo;

                    var separador = uow.ParametroRepository.GetParameter(ParamManager.NUMBER_DECIMAL_SEPARATOR);
                    var atributo = uow.AtributoRepository.GetAtributo(int.Parse(context.Parameters.FirstOrDefault(x => x.Id == "codigoAtributo").Value));
                    var formaterDateTime = uow.ParametroRepository.GetParameter(ParamManager.DATETIME_FORMAT_DATE_SECONDS);

                    switch (registro.TipoArgumento)
                    {
                        case TipoAtributoValidacionDb.NUMERICO:
                            if (!string.IsNullOrEmpty(atributo.Separador))
                            {
                                newAsociada.Valor = form.GetField("NUMERO").Value.Replace(atributo.Separador, separador);
                            }
                            else
                            {
                                newAsociada.Valor = form.GetField("NUMERO").Value;
                            }
                            break;
                        case TipoAtributoValidacionDb.FECHA:
                            if (!string.IsNullOrEmpty(form.GetField("FECHA").Value))
                            {
                                newAsociada.Valor = DateTimeExtension.ParseFromIso(form.GetField("FECHA").Value)?.Date.ToString(formaterDateTime);
                            }
                            break;
                        case TipoAtributoValidacionDb.HORA:
                            newAsociada.Valor = form.GetField("HORA").Value;
                            break;
                        case TipoAtributoValidacionDb.TEXTO:
                            newAsociada.Valor = form.GetField("TEXTO").Value;
                            break;
                        case TipoAtributoValidacionDb.URL:
                            newAsociada.Valor = form.GetField("URL").Value;
                            break;
                        case TipoAtributoValidacionDb.DECIMAL:
                            if (!string.IsNullOrEmpty(atributo.Separador))
                            {
                                newAsociada.Valor = form.GetField("DECIMAL").Value.Replace(atributo.Separador, separador);
                            }
                            else
                            {
                                newAsociada.Valor = form.GetField("DECIMAL").Value;
                            }
                            break;
                    }

                    uow.AtributoRepository.AddAtributoValidacionAsociada(newAsociada);

                    listaAtributoValidacion.Remove(registro);

                    context.Parameters.RemoveAll(w => w.Id == "listValidaciones");

                    if (listaAtributoValidacion.Count > 0)
                    {
                        context.Parameters.Add(new ComponentParameter() { Id = "ListValidacion", Value = JsonConvert.SerializeObject(listaAtributoValidacion) });
                    }
                    else
                    {
                        context.Parameters.Add(new ComponentParameter() { Id = "ListValidacion", Value = "" });
                    }
                }
                else
                {
                    context.Parameters.Add(new ComponentParameter() { Id = "ListValidacion", Value = "" });
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw ex;
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new CreateAtributoValidacionValorFormValidationModule(uow, this._identity, this._security), form, context);
        }
    }
}
