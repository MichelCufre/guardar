using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Impresiones;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    class ImpresionUTValidationModule : FormValidationModule
    {
        protected readonly bool _reimprimir;
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;

        public ImpresionUTValidationModule(IUnitOfWork uow, IIdentityService identity, bool reimprimir)
        {
            this._uow = uow;
            this._identity = identity;
            this._reimprimir = reimprimir;

            this.Schema = new FormValidationSchema
            {
                ["impresora"] = this.ValidateImpresora,
                ["predio"] = this.ValidatePredio,
                ["lenguaje"] = this.ValidateLenguaje,
                ["estilo"] = this.ValidateEstilo,
                ["numCopias"] = this.ValidateCopias,
                ["cantGenerar"] = this.ValidateNuevosGenerados
            };
        }

        #region Predio

        public virtual FormValidationGroup ValidatePredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule> {
                new NonNullValidationRule(field.Value),
                new ExisteImpresoraPredioValidationRule(_uow, field.Value),
            };

            if (_identity.Predio != GeneralDb.PredioSinDefinir)
                rules.Add(new ExistePredioValidationRule(_uow, this._identity.UserId, this._identity.Predio, field.Value));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.ValidatePredio_OnSucess,
                OnFailure = this.ValidatePredio_OnFailure
            };
        }
        public virtual void ValidatePredio_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {

            var selectorImpresora = form.GetField("impresora");
            selectorImpresora.Options = new List<SelectOption>();

            var impresoras = _uow.ImpresoraRepository.GetListaImpresorasPredio(field.Value);

            foreach (var impresora in impresoras)
            {
                selectorImpresora.Options.Add(new SelectOption(impresora.Id, $"{impresora.Id} - {impresora.Descripcion}"));
            };

            form.GetField("impresora").ReadOnly = false;

            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                //Le sugiere la ultima impresora utilizada para ese predio si es posible
                var impresion = _uow.ImpresionRepository.ObtenerImpresoraUltimaImpresion(_identity.UserId, _identity.Predio);

                if (impresion != null && !string.IsNullOrEmpty(impresion.CodigoImpresora))
                {
                    if (_uow.ImpresoraRepository.ExisteImpresora(impresion.CodigoImpresora, impresion.Predio))//Por si se cambia el codigo de impresora
                    {
                        var impresora = _uow.ImpresoraRepository.GetImpresora(impresion.CodigoImpresora, impresion.Predio);
                        var lenguajeImpresion = _uow.ImpresionRepository.GetLenguajeImpresion(impresora?.CodigoLenguajeImpresion);

                        form.GetField("impresora").Value = impresion.CodigoImpresora;
                        form.GetField("lenguaje").Value = lenguajeImpresion?.Id;
                        form.GetField("descripcionLenguaje").Value = lenguajeImpresion?.Descripcion;
                    }
                    form.GetField("impresora").ReadOnly = false;
                }
                else
                {
                    form.GetField("lenguaje").Value = string.Empty;
                    form.GetField("descripcionLenguaje").Value = string.Empty;
                }
            }
        }
        public virtual void ValidatePredio_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("impresora").ReadOnly = true;
            form.GetField("impresora").Value = string.Empty;
            form.GetField("lenguaje").Value = string.Empty;
            form.GetField("descripcionLenguaje").Value = string.Empty;

        }

        #endregion

        #region Impresora

        public virtual FormValidationGroup ValidateImpresora(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(form.GetField("predio").Value))
            {
                form.GetField("predio").SetError(new ComponentError("General_Sec0_Error_Error25", new List<string>()));
                return null;
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ImpresoraPertenecePredioValidatonRule(_uow,field.Value, this._identity.Predio)
                },
                OnSuccess = this.ValidateImpresora_OnSucess,
                OnFailure = this.ValidateImpresora_OnFailure,
            };
        }

        public virtual void ValidateImpresora_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string impresoraLenguaje = _uow.ImpresoraRepository.GetImpresora(field.Value, form.GetField("predio").Value).CodigoLenguajeImpresion;

            LenguajeImpresion lenguaje = _uow.ImpresionRepository.GetLenguajeImpresion(impresoraLenguaje);

            form.GetField("lenguaje").Value = lenguaje.Id;
            form.GetField("descripcionLenguaje").Value = lenguaje.Descripcion;

            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                FormField selectorEstilo = form.GetField("estilo");

                selectorEstilo.Options = new List<SelectOption>();

                List<EtiquetaEstiloLenguaje> listaEstilos = _uow.ImpresionRepository.GetEstiloByLenguaje(lenguaje.Id, EstiloEtiquetaDb.UnidadTransporte);

                foreach (var estilo in listaEstilos)
                {
                    selectorEstilo.Options.Add(new SelectOption(estilo.CodigoLabel, $"{estilo.CodigoLabel} - {estilo.Descripcion}"));
                }
            }
        }
        public virtual void ValidateImpresora_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("lenguaje").Value = string.Empty;
            form.GetField("descripcionLenguaje").Value = string.Empty;
        }

        #endregion

        public virtual FormValidationGroup ValidateLenguaje(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                }
            };
        }

        public virtual FormValidationGroup ValidateEstilo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                }
            };
        }

        public virtual FormValidationGroup ValidateCopias(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!_reimprimir)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveIntValidationRule(field.Value, false),
                    new CantidadMaximaImpresionesValidationRule(field.Value, _uow, _identity.GetFormatProvider()),
                }
            };
        }

        public virtual FormValidationGroup ValidateNuevosGenerados(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (_reimprimir)
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveIntValidationRule(field.Value, false),
                    new CantidadMaximaImpresionesValidationRule(field.Value, _uow, _identity.GetFormatProvider()),
                }
            };
        }
    }
}
