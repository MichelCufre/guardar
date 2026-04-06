using System;
using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class TipoLpnFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IFormatProvider _culture;

        public TipoLpnFormValidationModule(IUnitOfWork uow, IIdentityService identity, ISecurityService security)
        {
            this._uow = uow;
            this._identity = identity;
            this._security = security;
            this._culture = identity.GetFormatProvider();
            this.Schema = new FormValidationSchema
            {
                ["TP_LPN_TIPO"] = this.ValidateTipoLpn,
                ["NM_LPN_TIPO"] = this.ValidateNombreTipoLpn,
                ["DS_LPN_TIPO"] = this.ValidateDescripcionTipoLpn,
                ["NU_COMPONENTE"] = this.ValidateComponente,
                ["NU_TEMPLATE_ETIQUETA"] = this.ValidateTemplate,
                ["NU_SEQ_LPN"] = this.ValidateNumeroInicioSecuencia,
                //["VL_PICKING_SCORE_EQNR"] = this.ValidateIgualSinReserva,
                //["VL_PICKING_SCORE_EQR"] = this.ValidateIgualConReserva,
                //["VL_PICKING_SCORE_LTNR"] = this.ValidateMenorSinReserva,
                //["VL_PICKING_SCORE_LTR"] = this.ValidateMenorConReserva,
                //["VL_PICKING_SCORE_GT"] = this.ValidateMayor,
                //["VL_PICKING_SCORE_NE"] = this.ValidateInexistente,
                //["VL_PICKING_SCORE_BONUS"] = this.ValidateBonus,
                ["VL_PREFIJO"] = this.ValidatePrefijo,
                ["FL_CONTENEDOR_LPN"] = this.ValidateContenedorLPN,
                ["FL_PERMITE_AGREGAR_LINEAS"] = this.ValidatePermiteAgregarLineas,
            };
        }

        public virtual FormValidationGroup ValidateTipoLpn(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new TipoLPNYaRegistradoValidationRule (field.Value, this._uow),
                   new StringMaxLengthValidationRule(field.Value, 10),
                }
            };
        }

        public virtual FormValidationGroup ValidatePrefijo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 3),
                   new StringMinLengthValidationRule(field.Value, 3),
                   new TipoEtiquetaValidationRule(field.Value,_uow)
                }
            };
        }

        public virtual FormValidationGroup ValidateNombreTipoLpn(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 30),
                }
            };
        }

        public virtual FormValidationGroup ValidateDescripcionTipoLpn(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 400),
                }
            };
        }

        public virtual FormValidationGroup ValidateNumeroInicioSecuencia(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value,15),
                   new NumeroEnteroValidationRule(field.Value),
                   new PositiveDecimalValidationRule(this._culture,field.Value),
                }
            };
        }

        public virtual FormValidationGroup ValidateComponente(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 10),
                }
            };
        }

        public virtual FormValidationGroup ValidateTemplate(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 15),
                }
            };
        }

        public virtual FormValidationGroup ValidateIgualSinReserva(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new PositiveOrNegativeDecimalValidationRule(this._culture,field.Value),
                }
            };
        }

        public virtual FormValidationGroup ValidateIgualConReserva(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new PositiveOrNegativeDecimalValidationRule(this._culture,field.Value),
                }
            };
        }

        public virtual FormValidationGroup ValidateMenorSinReserva(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            string puntajeIgualSinRes = form.GetField("VL_PICKING_SCORE_EQNR")?.Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new PositiveOrNegativeDecimalValidationRule(this._culture,field.Value),
                   new PuntajeMenorSinReservaValidationRule(field.Value, puntajeIgualSinRes, this._culture),
                }
            };
        }

        public virtual FormValidationGroup ValidateMenorConReserva(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            string puntajeIgualSinRes = form.GetField("VL_PICKING_SCORE_EQR")?.Value;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new PositiveOrNegativeDecimalValidationRule(this._culture,field.Value),
                   new PuntajeMenorConReservaValidationRule(field.Value, puntajeIgualSinRes, this._culture),
                }
            };
        }

        public virtual FormValidationGroup ValidateMayor(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new PositiveOrNegativeDecimalValidationRule(this._culture,field.Value),
                }
            };
        }

        public virtual FormValidationGroup ValidateInexistente(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new PositiveOrNegativeDecimalValidationRule(this._culture,field.Value),
                }
            };
        }

        public virtual FormValidationGroup ValidateBonus(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new PositiveOrNegativeDecimalValidationRule(this._culture,field.Value),
                }
            };
        }

        public virtual FormValidationGroup ValidateContenedorLPN(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new BooleanStringValidationRule(field.Value)
                },
                OnSuccess = this.ValidateContenedorLPN_OnSuccess
            };
        }

        public virtual void ValidateContenedorLPN_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var contenedorLPN = form.GetField("FL_CONTENEDOR_LPN");
            var permiteAgregarLineas = form.GetField("FL_PERMITE_AGREGAR_LINEAS");


            if (contenedorLPN.Value == "true")
            {
                permiteAgregarLineas.Value = "true";
            }
        }

        public virtual FormValidationGroup ValidatePermiteAgregarLineas(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new BooleanStringValidationRule(field.Value)
                },
                OnSuccess = this.ValidatePermiteAgregarLineas_OnSuccess
            };
        }

        public virtual void ValidatePermiteAgregarLineas_OnSuccess(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var contenedorLPN = form.GetField("FL_CONTENEDOR_LPN");
            var permiteAgregarLineas = form.GetField("FL_PERMITE_AGREGAR_LINEAS");

            if (permiteAgregarLineas.Value == "false")
            {
                contenedorLPN.Value = "false";
            }
        }
    }
}
