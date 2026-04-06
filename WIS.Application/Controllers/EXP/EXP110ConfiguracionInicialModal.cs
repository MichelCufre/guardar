using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Expedicion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto;
using WIS.Domain.Impresiones;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.Security;

namespace WIS.Application.Controllers.EXP
{
    public class EXP110ConfiguracionInicialModal : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormValidationService _formValidationService;

        public EXP110ConfiguracionInicialModal(
          ISecurityService security,
          IUnitOfWorkFactory uowFactory,
          IIdentityService identity,
          IFormValidationService formValidationService)
        {
            this._security = security;
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._formValidationService = formValidationService;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            if (this._identity.Predio.Equals(GeneralDb.PredioSinDefinir))
            {
                form.GetField("predio").Disabled = true;
                form.GetField("impresora").Disabled = true;
                form.GetField("estilo").Disabled = true;
                form.GetField("lenguaje").Disabled = true;
                form.GetField("descripcionLenguaje").Disabled = true;
                form.GetField("ubicacion").Disabled = true;

                context.AddErrorNotification("General_Sec0_Error_PredioSinDefinir");
                return form;
            }

            using var uow = this._uowFactory.GetUnitOfWork();

            var confInicial = context.GetParameter("CONF_INICIAL");

            if (!string.IsNullOrEmpty(confInicial))
            {
                this.InicializarSelect(uow, form, JsonConvert.DeserializeObject<ConfiguracionInicial>(confInicial));
            }
            else
            {
                var impresion = uow.ImpresionRepository.ObtenerImpresoraUltimaImpresion(this._identity.UserId, this._identity.Predio);

                if (impresion != null && !string.IsNullOrEmpty(impresion.CodigoImpresora))
                {
                    if (uow.ImpresoraRepository.ExisteImpresora(impresion.CodigoImpresora, impresion.Predio))
                    {
                        var impresora = uow.ImpresoraRepository.GetImpresora(impresion.CodigoImpresora, impresion.Predio);
                        var lenguajeImpresion = uow.ImpresionRepository.GetLenguajeImpresion(impresora?.CodigoLenguajeImpresion);

                        form.GetField("impresora").Value = impresion.CodigoImpresora;
                        form.GetField("lenguaje").Value = lenguajeImpresion?.Id;
                        form.GetField("descripcionLenguaje").Value = lenguajeImpresion?.Descripcion;
                    }

                    form.GetField("impresora").ReadOnly = false;
                    form.GetField("predio").Value = impresion.Predio;
                }

                this.InicializarSelect(uow, form);
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            var confInicial = context.GetParameter("CONF_INICIAL");
            ConfiguracionInicial configuracion = null;

            if (string.IsNullOrEmpty(confInicial))
            {
                configuracion = new ConfiguracionInicial
                {
                    Estilo = form.GetField("estilo").Value,
                    Impresora = form.GetField("impresora").Value,
                    Predio = form.GetField("predio").Value,
                    Ubicacion = form.GetField("ubicacion").Value,
                    DescripcionLenguaje = form.GetField("descripcionLenguaje").Value,
                    Lenguaje = form.GetField("lenguaje").Value
                };
            }
            else
            {
                configuracion = JsonConvert.DeserializeObject<ConfiguracionInicial>(confInicial);

                configuracion.Estilo = form.GetField("estilo").Value;
                configuracion.Impresora = form.GetField("impresora").Value;
                configuracion.Predio = form.GetField("predio").Value;
                configuracion.Ubicacion = form.GetField("ubicacion").Value;
                configuracion.DescripcionLenguaje = form.GetField("descripcionLenguaje").Value;
                configuracion.Lenguaje = form.GetField("lenguaje").Value;
            }

            context.AddParameter("CONF_INICIAL", JsonConvert.SerializeObject(configuracion));

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new EXP110ConfiguracionInicialValidationModule(uow, this._identity.UserId, this._identity.Predio), form, context);
        }

        public virtual void InicializarSelect(IUnitOfWork uow, Form form, ConfiguracionInicial configuracion = null)
        {
            var selectorEstilo = form.GetField("estilo");
            var selectorImpresora = form.GetField("impresora");
            var selectorPredios = form.GetField("predio");
            var selectorUbicacion = form.GetField("ubicacion");

            selectorEstilo.Options = new List<SelectOption>();
            selectorPredios.Options = new List<SelectOption>();
            selectorImpresora.Options = new List<SelectOption>();
            selectorUbicacion.Options = new List<SelectOption>();

            //Estilo
            var dbQuery = new EtiquetasEmpaquetadoPickingQuery();
            uow.HandleQuery(dbQuery);

            var listaEstilos = dbQuery.GetEtiquetasEstilo();

            foreach (var estilo in listaEstilos)
            {
                selectorEstilo.Options.Add(new SelectOption(estilo.Id, $"{estilo.Id} - {estilo.Descripcion}"));
            }

            form.GetField("estilo").Value = "WIS-CONENT";
            form.GetField("estilo").ReadOnly = true;

            //Predio
            var userPredios = uow.PredioRepository.GetPrediosUsuario(_identity.UserId);

            foreach (var p in userPredios)
            {
                selectorPredios.Options.Add(new SelectOption(p.Numero, $"{p.Numero} - {p.Descripcion}"));
            }

            form.GetField("predio").Value = this._identity.Predio;
            form.GetField("predio").ReadOnly = true;

            //Impresora
            List<Impresora> listaImpresoras;
            if (this._identity.Predio == GeneralDb.PredioSinDefinir)
                listaImpresoras = uow.ImpresoraRepository.GetListaImpresorasPredio(form.GetField("predio").Value);
            else
            {
                listaImpresoras = uow.ImpresoraRepository.GetListaImpresorasPredio(this._identity.Predio);
                form.GetField("impresora").ReadOnly = false;
            }

            foreach (var impresora in listaImpresoras)
            {
                selectorImpresora.Options.Add(new SelectOption(impresora.Id, $"{impresora.Id} - {impresora.Descripcion}"));
            }

            if (configuracion != null)
            {
                form.GetField("impresora").Value = configuracion.Impresora;
                form.GetField("lenguaje").Value = configuracion.Lenguaje;
                form.GetField("descripcionLenguaje").Value = configuracion.DescripcionLenguaje;
            }

            //Ubicaciones
            var colParams = new Dictionary<string, string>
            {
                [ParamManager.PARAM_PRED] = string.Format("{0}_{1}", ParamManager.PARAM_PRED, this._identity.Predio)
            };
            var estaciones = uow.ParametroRepository.GetParameter("EXP110_ESTACIONES", colParams);

            var ubicaciones = estaciones.Split('-').ToList();

            var lstUbicaciones = uow.UbicacionRepository.GetUbicaciones(ubicaciones, this._identity.Predio);

            foreach (var ubicacion in lstUbicaciones)
            {
                selectorUbicacion.Options.Add(new SelectOption(ubicacion.Id, ubicacion.Id));
            }

            if (configuracion != null)
                form.GetField("ubicacion").Value = configuracion.Ubicacion;
        }
    }
}
