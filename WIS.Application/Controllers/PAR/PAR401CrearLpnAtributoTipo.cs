using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Helpers;
using WIS.Domain.Parametrizacion;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.PRE
{
    public class PAR401CrearLpnAtributoTipo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;

        public PAR401CrearLpnAtributoTipo(IIdentityService identity, IUnitOfWorkFactory uowFactory, IFormValidationService formValidationService, ISecurityService security)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._formValidationService = formValidationService;
            this._security = security;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var selectAtributo = form.GetField("ID_ATRIBUTO");
                selectAtributo.Options = new List<SelectOption>();

                var tipoLpn = context.Parameters.FirstOrDefault(x => x.Id == "LpnTipo")?.Value;

                var atributos = uow.AtributoRepository.GetAtributosDisponibles(tipoLpn);
                foreach (var atributo in atributos)
                {
                    selectAtributo.Options.Add(new SelectOption(atributo.Id.ToString(), $"{atributo.Id} - {atributo.Descripcion}"));
                }

                var selectConsolidadorTipo = form.GetField("ID_CONSOLIDACION_TIPO");
                selectConsolidadorTipo.Options = new List<SelectOption>();


                var selectEstado = form.GetField("ID_ESTADO_INICIAL");
                selectEstado.Options = new List<SelectOption>();

                var estados = uow.AtributoRepository.GetEstados();
                foreach (var estado in estados)
                {
                    selectEstado.Options.Add(new SelectOption(estado.Id, $"{estado.Id} - {estado.Descripcion}"));
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
                var tplpn = string.Empty;
                var parameter = context.Parameters.FirstOrDefault(x => x.Id == "LpnTipo");
                if (parameter != null)
                    tplpn = parameter.Value;

                var idAtributo = form.GetField("ID_ATRIBUTO").Value;
                var existeAtributoTipo = uow.AtributoRepository.AnyLpnTipoAtributo(tplpn, int.Parse(idAtributo));

                if (existeAtributoTipo)
                    context.AddErrorNotification("PAR401_Sec0_Error_AtributoYaRegistrado");
                else
                {
                    var atributoLpn = new LpnTipoAtributo
                    {
                        IdAtributo = int.Parse(idAtributo),
                        TipoLpn = tplpn,
                        Requerido = Mapper.MapStringToBooleanString(form.GetField("FL_REQUERIDO").Value),
                        ValidoInterfaz = Mapper.MapStringToBooleanString(form.GetField("VL_VALIDO_INTERFAZ").Value),
                        EstadoInicial = form.GetField("ID_ESTADO_INICIAL").Value
                    };

                    if (!string.IsNullOrEmpty(form.GetField("ID_CONSOLIDACION_TIPO").Value))
                        atributoLpn.IdConsolidador = int.Parse(form.GetField("ID_CONSOLIDACION_TIPO").Value);

                    var tipoAtributo = uow.AtributoRepository.GetIdAtributoTipo(int.Parse(idAtributo));
                    var cantidadTipoAtributo = uow.AtributoRepository.GetCantidadAtributoTipo(tplpn);

                    atributoLpn.Orden = short.Parse(cantidadTipoAtributo.ToString());

                    var formaterDateTime = uow.ParametroRepository.GetParameter(ParamManager.DATETIME_FORMAT_DATE_SECONDS);
                    var separador = uow.ParametroRepository.GetParameter(ParamManager.NUMBER_DECIMAL_SEPARATOR);
                    var atributo = uow.AtributoRepository.GetAtributo(int.Parse(form.GetField("ID_ATRIBUTO").Value));

                    atributoLpn.ValorInicial = AtributoHelper.GetValorByIdTipo(form, atributo, separador, formaterDateTime);

                    uow.ManejoLpnRepository.AddLpnTipoAtributo(atributoLpn);
                    uow.SaveChanges();
                    context.AddSuccessNotification("General_Sec0_Error_Er021_SaveSuccess");
                }

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

            return this._formValidationService.Validate(new CreateAtributoTipoLpnFormValidationModule(uow, this._identity, this._security, this._identity.GetFormatProvider()), form, context);
        }
    }
}
