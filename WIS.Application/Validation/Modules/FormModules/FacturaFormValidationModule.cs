using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.FormComponent.Validation;
using WIS.FormComponent;
using WIS.Validation;
using WIS.Domain.DataModel;
using WIS.Application.Validation.Rules.Recepcion;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class FacturaFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _idUsuario;
        protected readonly string _predioLogueado;
        protected readonly IFormatProvider _culture;

        public FacturaFormValidationModule(IUnitOfWork uow, int idUsuario, string predioLogueado, IFormatProvider culture)
        {
            this._uow = uow;
            this._idUsuario = idUsuario;
            this._predioLogueado = predioLogueado;
            this._culture = culture;

            this.Schema = new FormValidationSchema
            {
                ["idEmpresa"] = this.ValidateIdEmpresa,
                ["codigoInternoAgente"] = this.ValidateCodigoInternoAgente,
                ["numeroPredio"] = this.ValidateNumeroPredio,
                ["emision"] = this.ValidateEmision,
                ["anexo1"] = this.ValidateAnexo,
                ["anexo2"] = this.ValidateAnexo,
                ["anexo3"] = this.ValidateAnexo,
                ["vencimiento"] = this.ValidateVencimiento,
                ["totalDigitado"] = this.ValidateTotalDigitado,
                ["numeroFactura"] = this.ValidateNumeroFactura,
                ["numeroSerie"] = this.ValidateNumeroSerie,
                ["observacion"] = this.ValidateObservacion,
                ["ivaBase"] = this.ValidateIvaBase,
                ["ivaMinimo"] = this.ValidateIvaMinimo,
            };
        }

        public virtual FormValidationGroup ValidateObservacion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value, 200),
                },
            };
        }

        public virtual FormValidationGroup ValidateNumeroSerie(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var validateGroup = new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>()
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 3),
                }
            };

            return validateGroup;
        }

        public virtual FormValidationGroup ValidateTotalDigitado(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveDecimalValidationRule(this._culture, field.Value),
                    new DecimalGreaterThanValidationRule(this._culture, field.Value,0),
                    new DecimalLengthWithPresicionValidationRule(field.Value, 15, 4, this._culture),
                }
            };
        }

        public virtual FormValidationGroup ValidateIvaMinimo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveDecimalValidationRule(this._culture, field.Value),
                    new DecimalGreaterThanValidationRule(this._culture, field.Value,0),
                    new DecimalLengthWithPresicionValidationRule(field.Value, 15, 4, this._culture),
                    //new IvaBaseValidationRule(this._culture, form.GetField("totalDigitado").Value, field.Value, form.GetField("ivaBase").Value)

                }
            };
        }

        public virtual FormValidationGroup ValidateIvaBase(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveDecimalValidationRule(this._culture, field.Value),
                    new DecimalGreaterThanValidationRule(this._culture, field.Value,0),
                    new DecimalLengthWithPresicionValidationRule(field.Value, 15, 4, this._culture),
                    //new IvaBaseValidationRule(this._culture, form.GetField("totalDigitado").Value, form.GetField("ivaMinimo").Value, field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateNumeroFactura(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value,12)
                }
            };
        }

        public virtual FormValidationGroup ValidateVencimiento(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules =
                {
                    new DateTimeValidationRule(field.Value)
                }
            };
        }

        public virtual FormValidationGroup ValidateIdEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PositiveNumberMaxLengthValidationRule(field.Value,10),
                    new ExisteEmpresaValidationRule(_uow, field.Value)
                },
                OnSuccess = this.ValidateIdEmpresa_OnSuccess,
                OnFailure = this.ValidateIdEmpresa_OnFailure,
            };
        }
        public virtual void ValidateIdEmpresa_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                this.LimpiarCamposDependientesDeEmpresa(field, form, parameters);
            }
        }
        public virtual void ValidateIdEmpresa_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            this.LimpiarCamposDependientesDeEmpresa(field, form, parameters);
        }
        public virtual void LimpiarCamposDependientesDeEmpresa(FormField field, Form form, List<ComponentParameter> parameters)
        {
            parameters.Add(new ComponentParameter() { Id = "tipoSeleccion", Value = "Fail" });

            //form.GetField("autoCargarDetalle").Disabled = true;
            //form.GetField("autoCargarDetalle").Value = "false";
        }

        public virtual FormValidationGroup ValidateCodigoInternoAgente(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            if (string.IsNullOrEmpty(form.GetField("idEmpresa").Value))
            {
                form.GetField("idEmpresa").SetError(new ComponentError("General_Sec0_Error_CampoEmpresaReuqerido", new List<string>()));
                return null;
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10),
                    new ExisteClienteValidationRule(_uow,field.Value, form.GetField("idEmpresa").Value),

                },
                // Dependencies = { "idEmpresa" },
                OnSuccess = this.ValidateCodigoInternoAgente_OnSuccess,
                OnFailure = this.ValidateCodigoInternoAgente_OnFailure,
            };
        }
        public virtual void ValidateCodigoInternoAgente_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (!parameters.Any(s => s.Id == "isSubmit"))
            {
                int idEmpresa = int.Parse(form.GetField("idEmpresa").Value);

                string tipoAgente = _uow.AgenteRepository.GetTipoAgente(field.Value);

                //var campo = form.GetField("tipoRecepcionExterno");

                //campo.ReadOnly = false;
                //campo.Options.Clear();
                //campo.Options = new List<SelectOption>();

                // Tipos de recepción
                //List<EmpresaRecepcionTipoCustom> tiposRecepcionExterno = _uow.RecepcionTipoCustomRepository.GetRecepcionTiposEmpresaHabilitados(idEmpresa, tipoAgente);

                //foreach (var tipo in tiposRecepcionExterno)
                //{
                //    campo.Options.Add(new SelectOption(tipo.TipoExterno, $"{tipo.TipoExterno} - {tipo.DescripcionExterna }"));
                //}
            }
        }
        public virtual void ValidateCodigoInternoAgente_OnFailure(FormField field, Form form, List<ComponentParameter> parameters)
        {
            //var campo = form.GetField("tipoRecepcionExterno");
            //campo.Options.Clear();

            //  campo.ReadOnly = true;

            parameters.Add(new ComponentParameter() { Id = "tipoSeleccion", Value = "Fail" });
        }

        public virtual FormValidationGroup ValidateNumeroPredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new PredioUsuarioExistenteValidationRule(this._uow, this._idUsuario ,field.Value)
                },
                OnSuccess = this.ValidatePredio_OnSuccess,
            };
        }
        public virtual void ValidatePredio_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
        }

        public virtual FormValidationGroup ValidateEmision(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new DateTimeValidationRule(field.Value),
                    //new DateTimeGreaterThanValidationRule(field.Value,DateTimeExtension.ToIsoString( DateTime.Now.Date), "General_Sec0_Error_InvalidDateAnteriorHoy")
                }
            };
        }

        public virtual FormValidationGroup ValidateAnexo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                {
                    new StringMaxLengthValidationRule(field.Value,200)
                }
            };
        }
    }
}
