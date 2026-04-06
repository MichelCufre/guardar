using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Helpers;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Tracking;
using WIS.Extension;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.STO
{
    public class STO710ModificarLpnAtributo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;

        public STO710ModificarLpnAtributo(IIdentityService identity, IUnitOfWorkFactory uowFactory, IFormValidationService formValidationService, ISecurityService security)
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
                string lpnTipo = query.Parameters.FirstOrDefault(x => x.Id == "LpnTipo").Value;
                string idAtributo = query.Parameters.FirstOrDefault(x => x.Id == "IdAtributo").Value;
                long nuLpn = long.Parse(query.Parameters.FirstOrDefault(x => x.Id == "NuLpn").Value);
                LpnAtributo lpnAtributo = uow.ManejoLpnRepository.GetLpnAtributo(nuLpn, int.Parse(idAtributo), lpnTipo);
                Atributo atributo = uow.AtributoRepository.GetAtributo(lpnAtributo.Id);
                
                form.GetField("DS_ATRIBUTO").Value = $"{lpnAtributo.Id.ToString()} - {atributo.Descripcion}";
                
                string formaterDateTime = uow.ParametroRepository.GetParameter(ParamManager.DATETIME_FORMAT_DATE_SECONDS);

                query.Parameters.Add(new ComponentParameter { Id = "isDominio", Value = "F" });
                query.Parameters.Add(new ComponentParameter { Id = "isSistema", Value = "F" });
                query.Parameters.Add(new ComponentParameter { Id = "isTexto", Value = "F" });
                query.Parameters.Add(new ComponentParameter { Id = "isHora", Value = "F" });
                query.Parameters.Add(new ComponentParameter { Id = "isFecha", Value = "F" });
                query.Parameters.Add(new ComponentParameter { Id = "isNumerico", Value = "F" });
                
                string separador = uow.ParametroRepository.GetParameter(ParamManager.NUMBER_DECIMAL_SEPARATOR);

                switch (atributo.IdTipo)
                {
                    case TipoAtributoDb.NUMERICO:
                        query.Parameters.RemoveAll(w => w.Id == "isNumerico");
                        query.Parameters.Add(new ComponentParameter { Id = "isNumerico", Value = "T" });
                        if (!string.IsNullOrEmpty(atributo.Separador))
                        {
                            form.GetField("NUMERO").Value = lpnAtributo.Valor.Replace(separador, atributo.Separador);
                        }
                        else
                        {
                            form.GetField("NUMERO").Value = lpnAtributo.Valor;
                        }
                        break;

                    case TipoAtributoDb.FECHA:
                        query.Parameters.RemoveAll(w => w.Id == "isFecha");
                        query.Parameters.Add(new ComponentParameter { Id = "isFecha", Value = "T" });

                        if (!string.IsNullOrEmpty(lpnAtributo.Valor))
                        {
                            form.GetField("FECHA").Value = DateTime.ParseExact(lpnAtributo.Valor, formaterDateTime, _identity.GetFormatProvider()).ToIsoString();
                        }
                        break;
                    case TipoAtributoDb.HORA:
                        query.Parameters.RemoveAll(w => w.Id == "isHora");
                        query.Parameters.Add(new ComponentParameter { Id = "isHora", Value = "T" });

                        if (!string.IsNullOrEmpty(lpnAtributo.Valor))
                        {
                            form.GetField("HORA").Value = lpnAtributo.Valor;
                        }
                        break;
                    case TipoAtributoDb.TEXTO:
                        query.Parameters.RemoveAll(w => w.Id == "isTexto");
                        query.Parameters.Add(new ComponentParameter { Id = "isTexto", Value = "T" });

                        form.GetField("TEXTO").Value = lpnAtributo.Valor;
                        break;
                    case TipoAtributoDb.DOMINIO:
                        query.Parameters.RemoveAll(w => w.Id == "isDominio");
                        query.Parameters.Add(new ComponentParameter { Id = "isDominio", Value = "T" });
                        FormField selectDominio = form.GetField("CD_DOMINIO");
                        var ListaDominios = uow.DominioRepository.GetDominios(atributo.CodigoDominio);

                        selectDominio.Options = new List<SelectOption>();
                        foreach (var dominio in ListaDominios)
                        {
                            selectDominio.Options.Add(new SelectOption(dominio.Id.ToString(), $"{dominio.Id} - {dominio.Descripcion}"));
                        }
                        selectDominio.Value = lpnAtributo.Valor;
                        break;
                }

                string descripcionAtributoTipo = uow.AtributoRepository.GetDescripcionAtributoTipo(atributo.IdTipo);
                form.GetField("DS_ATRIBUTO_TIPO").Value = descripcionAtributoTipo;

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

            uow.CreateTransactionNumber("STO710  Modificar Atributo");
            uow.BeginTransaction();
            
            try
            {
                string tplpn = "";
                string id_Atributo = context.Parameters.FirstOrDefault(x => x.Id == "IdAtributo").Value;
                var pLpnTipo = context.Parameters.FirstOrDefault(x => x.Id == "LpnTipo");
                
                if (pLpnTipo != null)
                {
                    tplpn = pLpnTipo.Value;
                }

                long NU_LPN = long.Parse(context.Parameters.FirstOrDefault(x => x.Id == "NuLpn").Value);

                AtributoMapper atributoMapper = new AtributoMapper();
                LpnAtributo lpnAtributo = uow.ManejoLpnRepository.GetLpnAtributo(NU_LPN, int.Parse(id_Atributo), tplpn);

                lpnAtributo.Id = int.Parse(id_Atributo);
                lpnAtributo.Tipo = tplpn;
                lpnAtributo.NumeroTransaccion = uow.GetTransactionNumber();

                string formaterDateTime = uow.ParametroRepository.GetParameter(ParamManager.DATETIME_FORMAT_DATE_SECONDS);
                var culture = _identity.GetFormatProvider();
                var atributo = uow.AtributoRepository.GetAtributo(int.Parse(id_Atributo));
                string separador = uow.ParametroRepository.GetParameter(ParamManager.NUMBER_DECIMAL_SEPARATOR);

                lpnAtributo.Valor = AtributoHelper.GetValorByIdTipo(form, atributo, separador, formaterDateTime);
                lpnAtributo.Estado = EstadoLpnAtributo.Ingresado;

                uow.ManejoLpnRepository.UpdateLpnAtributo(lpnAtributo);
                uow.SaveChanges();
                context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");

                uow.Commit();
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
            return this._formValidationService.Validate(new LpnAtributoFormValidationModule(uow, this._identity, this._security, this._identity.GetFormatProvider()), form, context);
        }

    }
}
