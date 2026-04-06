using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Impresiones;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class STO700CreacionLPNValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _usuario;
        protected readonly string _predio;

        public STO700CreacionLPNValidationModule(IUnitOfWork uow, int usuarioLogueado, string predioLogueado)
        {
            this._uow = uow;
            this._usuario = usuarioLogueado;
            this._predio = predioLogueado;

            this.Schema = new FormValidationSchema
            {
                ["tipo"] = this.ValidateTipo,
                ["cantidadLPN"] = this.ValidateCantidadLPN,
                ["empresa"] = this.ValidateEmpresa,
                ["generarImprimir"] = this.ValidateGenerarImprimir,
                ["predio"] = this.ValidatePredio,
                ["impresora"] = this.ValidateImpresora,
                ["estilo"] = this.ValidateEstilo,
                ["lenguaje"] = this.ValidateLenguaje,
                ["descripcion"] = this.ValidateDescripcion,
                ["cantidadCopias"] = this.ValidateCantidadCopias,
                ["packingList"] = this.ValidatePackingList
            };
        }

        public virtual FormValidationGroup ValidateTipo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ExisteTipoLPNValidationRule(this._uow, field.Value)
                },
                OnSuccess = ValidateTipoOnSuccess
            };
        }
        public virtual void ValidateTipoOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var estilo = _uow.ManejoLpnRepository.GetEstiloEtiquetaLPN(field.Value);
            form.GetField("estilo").Value = estilo;
        }

        public virtual FormValidationGroup ValidateEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new ExisteEmpresaValidationRule(this._uow, field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateCantidadLPN(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveIntMayorACeroValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateGenerarImprimir(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new BooleanStringValidationRule(field.Value)
                },
                OnSuccess = ValidateGenerarImprimirOnSuccess
            };
        }
        public virtual void ValidateGenerarImprimirOnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            switch (field.Value)
            {
                case "false":
                    form.GetField("predio").Value = "";
                    form.GetField("impresora").Value = "";
                    form.GetField("estilo").Value = "";
                    form.GetField("lenguaje").Value = "";
                    form.GetField("descripcion").Value = "";
                    form.GetField("cantidadCopias").Value = "";
                    break;
            }

            AgregarParametroValidation(parameters, "generarImprimir", field.Value);
        }
        public virtual void AgregarParametroValidation(List<ComponentParameter> parameters, string Id, string value)
        {
            var genericParam = new ComponentParameter()
            {
                Id = Id,
                Value = value
            };

            if (parameters == null)
            {
                parameters = new List<ComponentParameter>();
                parameters.Add(genericParam);
            }
            else
            {
                if (parameters.FirstOrDefault(p => p.Id == Id) != null)
                    parameters.FirstOrDefault(p => p.Id == Id).Value = genericParam.Value;
                else
                    parameters.Add(genericParam);
            }
        }

        public virtual FormValidationGroup ValidatePredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var generarImprimir = form.GetField("generarImprimir").Value;
            var rules = new List<IValidationRule>();

            if (!this._predio.Equals(GeneralDb.PredioSinDefinir) && generarImprimir == "true")
                rules.Add(new ExistePredioValidationRule(_uow, this._usuario, this._predio, field.Value));

            if (generarImprimir == "true")
            {
                rules.Add(new NonNullValidationRule(field.Value));
                rules.Add(new ExisteImpresoraPredioValidationRule(_uow, field.Value));
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = ValidatePredio_OnSucess,
                OnFailure = ValidatePredio_OnFailure
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
                var impresion = _uow.ImpresionRepository.ObtenerImpresoraUltimaImpresion(this._usuario, field.Value);

                if (impresion != null && !string.IsNullOrEmpty(impresion.CodigoImpresora))
                {
                    if (_uow.ImpresoraRepository.ExisteImpresora(impresion.CodigoImpresora, impresion.Predio))//Por si se cambia el codigo de impresora
                    {
                        var impresora = _uow.ImpresoraRepository.GetImpresora(impresion.CodigoImpresora, impresion.Predio);
                        var lenguajeImpresion = _uow.ImpresionRepository.GetLenguajeImpresion(impresora?.CodigoLenguajeImpresion);

                        form.GetField("impresora").Value = impresion.CodigoImpresora;
                        form.GetField("lenguaje").Value = lenguajeImpresion?.Id;
                        form.GetField("descripcion").Value = lenguajeImpresion?.Descripcion;
                    }
                    form.GetField("impresora").ReadOnly = false;
                }
                else
                {
                    form.GetField("lenguaje").Value = string.Empty;
                    form.GetField("descripcion").Value = string.Empty;
                }
            }
        }
        public virtual void ValidatePredio_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            form.GetField("impresora").ReadOnly = true;
            form.GetField("impresora").Value = string.Empty;
            form.GetField("lenguaje").Value = string.Empty;
            form.GetField("descripcion").Value = string.Empty;
        }

        public virtual FormValidationGroup ValidateImpresora(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var generarImprimir = form.GetField("generarImprimir").Value;
            var rules = new List<IValidationRule>();

            if (generarImprimir == "true")
                rules.Add(new NonNullValidationRule(field.Value));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
                OnSuccess = this.ValidateImpresora_OnSucess,
            };
        }

        public virtual void ValidateImpresora_OnSucess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var generarImprimir = form.GetField("generarImprimir").Value;

            if (generarImprimir == "true")
            {
                var impresoraLenguaje = _uow.ImpresoraRepository.GetImpresora(field.Value, form.GetField("predio").Value).CodigoLenguajeImpresion;

                var lenguaje = _uow.ImpresionRepository.GetLenguajeImpresion(impresoraLenguaje);

                form.GetField("lenguaje").Value = lenguaje.Id;
                form.GetField("descripcion").Value = lenguaje.Descripcion;
            }
        }

        public virtual FormValidationGroup ValidateEstilo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var generarImprimir = form.GetField("generarImprimir").Value;
            var rules = new List<IValidationRule>();

            if (generarImprimir == "true")
                rules.Add(new NonNullValidationRule(field.Value));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateLenguaje(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var generarImprimir = form.GetField("generarImprimir").Value;
            var rules = new List<IValidationRule>();

            if (generarImprimir == "true")
                rules.Add(new NonNullValidationRule(field.Value));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateDescripcion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var generarImprimir = form.GetField("generarImprimir").Value;
            var rules = new List<IValidationRule>();

            if (generarImprimir == "true")
                rules.Add(new NonNullValidationRule(field.Value));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateCantidadCopias(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var generarImprimir = form.GetField("generarImprimir").Value;
            var rules = new List<IValidationRule>();

            if (generarImprimir == "true")
            {
                rules.Add(new NonNullValidationRule(field.Value));
                rules.Add(new PositiveIntMayorACeroValidationRule(field.Value));
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidatePackingList(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var rules = new List<IValidationRule>();

            if (!string.IsNullOrEmpty(field.Value))
                rules.Add(new StringMaxLengthValidationRule(field.Value, 50));

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }
    }
}
