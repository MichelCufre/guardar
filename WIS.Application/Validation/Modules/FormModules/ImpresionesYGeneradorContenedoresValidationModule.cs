using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Impresion;
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
    public class ImpresionesYGeneradorContenedoresValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;

        public ImpresionesYGeneradorContenedoresValidationModule(IUnitOfWork uow, IIdentityService identity)
        {
            this._uow = uow;
            this._identity = identity;


            this.Schema = new FormValidationSchema
            {
                ["impresora"] = this.ValidateImpresora,
                ["predio"] = this.ValidatePredio,
                ["lenguaje"] = this.ValidateLenguaje,
                ["estilo"] = this.ValidateEstilo,
                ["tipoContenedor"] = this.ValidateTipoContenedor,
                ["numCopias"] = this.ValidateCopias,
                ["cantGenerar"] = this.ValidateGenerar,
                ["contenedor"] = this.ValidateContenedor,
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

            FormField selectorImpresora = form.GetField("impresora");
            selectorImpresora.Options = new List<SelectOption>();

            List<Impresora> ListaImpresoras = _uow.ImpresoraRepository.GetListaImpresorasPredio(field.Value);

            foreach (var impresora in ListaImpresoras)
            {
                selectorImpresora.Options.Add(new SelectOption(impresora.Id, $"{impresora.Id} - {impresora.Descripcion}"));
            };
            form.GetField("impresora").ReadOnly = false;

            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                //Le sugiere la ultima impresora utilizada para ese predio si es posible
                Impresion impresion = _uow.ImpresionRepository.ObtenerImpresoraUltimaImpresion(this._identity.UserId, field.Value);

                if (impresion != null && !string.IsNullOrEmpty(impresion.CodigoImpresora))
                {
                    if (_uow.ImpresoraRepository.ExisteImpresora(impresion.CodigoImpresora, impresion.Predio))//Por si se cambia el codigo de impresora
                    {
                        Impresora impresora = _uow.ImpresoraRepository.GetImpresora(impresion.CodigoImpresora, impresion.Predio);
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
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                FormField selectorEstilo = form.GetField("estilo");
                FormField selectorTipoContenedor = form.GetField("tipoContenedor");

                selectorTipoContenedor.Value = "";
                selectorEstilo.Options = new List<SelectOption>();
            }

            string impresoraLenguaje = _uow.ImpresoraRepository.GetImpresora(field.Value, form.GetField("predio").Value).CodigoLenguajeImpresion;

            LenguajeImpresion lenguaje = _uow.ImpresionRepository.GetLenguajeImpresion(impresoraLenguaje);

            form.GetField("lenguaje").Value = lenguaje.Id;
            form.GetField("descripcionLenguaje").Value = lenguaje.Descripcion;

        }
        public virtual void ValidateImpresora_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("lenguaje").Value = string.Empty;
            form.GetField("descripcionLenguaje").Value = string.Empty;
        }

        #endregion

        #region TipoContendor
        public virtual FormValidationGroup ValidateTipoContenedor(FormField field, Form form, List<ComponentParameter> parameters)
        {

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new EsTipoContenedorPredefinidoValidationRule(_uow, field.Value)
                },
                OnSuccess = this.ValidateTipoContenedor_OnSucess,
                OnFailure = this.ValidateTipoContenedor_OnFailure
            };
        }
        public virtual void ValidateTipoContenedor_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string tipoContenedor = form.GetField("tipoContenedor").Value;
            string lenguaje = form.GetField("lenguaje").Value;

            FormField selectorEstilo = form.GetField("estilo");

            selectorEstilo.Options = new List<SelectOption>();

            List<EtiquetaEstiloLenguaje> listaEstilos = _uow.ImpresionRepository.GetEstiloByLenguajeContenedor(lenguaje, EstiloEtiquetaDb.Contenedor, tipoContenedor);

            foreach (var estilo in listaEstilos)
            {
                selectorEstilo.Options.Add(new SelectOption(estilo.CodigoLabel, $"{estilo.CodigoLabel} - {estilo.Descripcion}"));
            }

            form.GetField("estilo").ReadOnly = false;
        }

        public virtual void ValidateTipoContenedor_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {

        }
        #endregion

        #region Lenguaje / Estilo

        public virtual FormValidationGroup ValidateLenguaje(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,100),
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
        #endregion

        #region Copias / Generar

        public virtual FormValidationGroup ValidateGenerar(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.Any(p => p.Id == "isReimprimir"))
            {
                var isReimprimirParam = parameters.FirstOrDefault(p => p.Id == "isReimprimir").Value;
                bool isReimprimir = bool.TryParse(isReimprimirParam, out bool isReimprimirValue) ? isReimprimirValue : false;

                if (isReimprimir)
                    return null;
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveIntValidationRule(field.Value, false),
                    new CantidadMaximaImpresionesValidationRule(field.Value, _uow, _identity.GetFormatProvider()),
                },
                OnSuccess = this.ValidateGenerar_OnSucess
            };
        }

        public virtual void ValidateGenerar_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("numCopias").Value = string.Empty;
        }

        public virtual FormValidationGroup ValidateCopias(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (parameters.Any(p => p.Id == "isReimprimir"))
            {
                var isReimprimirParam = parameters.FirstOrDefault(p => p.Id == "isReimprimir").Value;
                bool isReimprimir = bool.TryParse(isReimprimirParam, out bool isReimprimirValue) ? isReimprimirValue : false;

                if (!isReimprimir)
                    return null;
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveIntValidationRule(field.Value, false),
                    new CantidadMaximaImpresionesValidationRule(field.Value, _uow, _identity.GetFormatProvider()),
                },
                OnSuccess = this.ValidateCopias_OnSucess
            };
        }

        public virtual void ValidateCopias_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("cantGenerar").Value = string.Empty;
        }
        #endregion

        #region Contenedor
        public virtual FormValidationGroup ValidateContenedor(FormField field, Form form, List<ComponentParameter> parameters)
        {

            if (parameters.Any(p => p.Id == "isReimprimir"))
            {
                var isReimprimirParam = parameters.FirstOrDefault(p => p.Id == "isReimprimir").Value;
                bool isReimprimir = bool.TryParse(isReimprimirParam, out bool isReimprimirValue) ? isReimprimirValue : false;

                if (!isReimprimir)
                    return null;
            }

            int nuContenedor = int.TryParse(field.Value, out int contenedor) ? contenedor : -1;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ExisteContenedorValidationRule(nuContenedor, this._uow)
                },
                OnSuccess = this.ValidateContenedor_OnSuccess
            };
        }
        public virtual void ValidateContenedor_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            string tipoContenedor = _uow.ContenedorRepository.GetTipoContenedorByNroContenedor(int.Parse(field.Value));
            form.GetField("tipoContenedor").Value = tipoContenedor;

            List<SelectOption> OpcionesSelectorEstilos = form.GetField("estilo").Options;

            List<EtiquetaEstiloLenguaje> listaEstilos = new List<EtiquetaEstiloLenguaje>();

            listaEstilos = _uow.ImpresionRepository.GetEstiloByTipoContenedor("CON", tipoContenedor);

            OpcionesSelectorEstilos.Clear();

            foreach (var estilo in listaEstilos)
            {
                OpcionesSelectorEstilos.Add(new SelectOption(estilo.CodigoLabel, $"{estilo.CodigoLabel} - {estilo.Descripcion}"));
            }

            form.GetField("estilo").ReadOnly = false;
        }

        #endregion

    }
}
