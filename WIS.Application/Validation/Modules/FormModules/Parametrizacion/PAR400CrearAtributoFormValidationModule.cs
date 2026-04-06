using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Parametrizacion;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Parametrizacion
{
    public class PAR400CrearAtributoFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly IFormatProvider _culture;

        public PAR400CrearAtributoFormValidationModule(
            IUnitOfWork uow,
            IIdentityService identity)
        {
            this._uow = uow;
            this._identity = identity;
            this._culture = identity.GetFormatProvider();

            Schema = new FormValidationSchema
            {
                ["tipo"] = this.ValidateTipo,
                ["dominio"] = this.ValidateDominio,
                ["nombre"] = this.ValidateNombre,
                ["descripcion"] = this.ValidateDescripcion,
                ["mascaraIngresoFecha"] = this.ValidateMascaraIngresoFecha,
                ["mascaraIngresoHora"] = this.ValidateMascaraIngresoHora,
                ["largo"] = this.ValidateLargo,
                ["decimales"] = this.ValidateDecimales,
                ["texto"] = this.ValidateTexto,
                ["displayFecha"] = this.ValidateDisplayFecha,
                ["displayHora"] = this.ValidateDisplayHora,
                ["separador"] = this.ValidateSeparador,
                ["campo"] = this.ValidateCampo,
            };
        }

        public virtual FormValidationGroup ValidateTipo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value)
                },
                OnSuccess = this.ValidateTipoOnSucces
            };
        }

        public virtual void ValidateTipoOnSucces(FormField field, Form form, List<ComponentParameter> parameters)
        {
            AtributoTipo tipo = _uow.AtributoRepository.GetTipoAtributoById(field.Value);

            switch (field.Value)
            {
                case TipoAtributoDb.NUMERICO:
                    form.GetField("mascaraIngresoFecha").Value = "";
                    form.GetField("mascaraIngresoHora").Value = "";
                    form.GetField("dominio").Value = "";
                    form.GetField("campo").Value = "";
                    form.GetField("displayFecha").Value = "";
                    form.GetField("displayHora").Value = "";
                    break;
                case TipoAtributoDb.FECHA:
                    form.GetField("largo").Value = "";
                    form.GetField("decimales").Value = "";
                    form.GetField("mascaraIngresoHora").Value = "";
                    form.GetField("dominio").Value = "";
                    form.GetField("campo").Value = "";
                    form.GetField("separador").Value = "";
                    form.GetField("displayHora").Value = "";
                    break;
                case TipoAtributoDb.HORA:
                    form.GetField("mascaraIngresoFecha").Value = "";
                    form.GetField("largo").Value = "";
                    form.GetField("decimales").Value = "";
                    form.GetField("dominio").Value = "";
                    form.GetField("campo").Value = "";
                    form.GetField("displayFecha").Value = "";
                    form.GetField("separador").Value = "";
                    break;
                case TipoAtributoDb.TEXTO:
                    form.GetField("mascaraIngresoFecha").Value = "";
                    form.GetField("largo").Value = "";
                    form.GetField("decimales").Value = "";
                    form.GetField("mascaraIngresoHora").Value = "";
                    form.GetField("dominio").Value = "";
                    form.GetField("campo").Value = "";
                    form.GetField("displayFecha").Value = "";
                    form.GetField("separador").Value = "";
                    form.GetField("displayHora").Value = "";
                    break;
                case TipoAtributoDb.DOMINIO:
                    form.GetField("mascaraIngresoFecha").Value = "";
                    form.GetField("largo").Value = "";
                    form.GetField("decimales").Value = "";
                    form.GetField("mascaraIngresoHora").Value = "";
                    form.GetField("campo").Value = "";
                    form.GetField("displayFecha").Value = "";
                    form.GetField("separador").Value = "";
                    form.GetField("displayHora").Value = "";
                    break;
                case TipoAtributoDb.SISTEMA:
                    form.GetField("mascaraIngresoFecha").Value = "";
                    form.GetField("largo").Value = "";
                    form.GetField("decimales").Value = "";
                    form.GetField("mascaraIngresoHora").Value = "";
                    form.GetField("dominio").Value = "";
                    form.GetField("displayFecha").Value = "";
                    form.GetField("separador").Value = "";
                    form.GetField("displayHora").Value = "";
                    break;
            }

            this.AgregarParametroValidation(parameters, "tipo", field.Value);
        }

        public virtual FormValidationGroup ValidateDominio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var validacionNoNull = new NonNullValidationRule(field.Value);
            var validacionDominio = new DominioValidationRule(field.Value, this._uow);
            var tipo = form.GetField("tipo").Value;
            var rules = new List<IValidationRule>();

            if (tipo == TipoAtributoDb.DOMINIO)
            {
                rules.Add(validacionNoNull);
                rules.Add(validacionDominio);
            }


            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateMascaraIngresoFecha(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var validacionNoNull = new NonNullValidationRule(field.Value);
            var validacionFormato = new FormatoFechaValidationRule(field.Value, _uow);
            var tipo = form.GetField("tipo").Value;
            var rules = new List<IValidationRule>();

            if (tipo == TipoAtributoDb.FECHA)
            {
                rules.Add(validacionNoNull);
                rules.Add(validacionFormato);
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateMascaraIngresoHora(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var validacionNoNull = new NonNullValidationRule(field.Value);
            var validacionFormato = new FormatoHoraValidationRule(field.Value, _uow);
            var tipo = form.GetField("tipo").Value;
            var rules = new List<IValidationRule>();

            if (tipo == TipoAtributoDb.HORA)
            {
                rules.Add(validacionNoNull);
                rules.Add(validacionFormato);
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateDisplayFecha(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var validacionNoNull = new NonNullValidationRule(field.Value);
            var validacionFormato = new FormatoFechaValidationRule(field.Value, _uow);
            var tipo = form.GetField("tipo").Value;
            var rules = new List<IValidationRule>();

            if (tipo == TipoAtributoDb.FECHA)
            {
                rules.Add(validacionNoNull);
                rules.Add(validacionFormato);
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateDisplayHora(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var validacionNoNull = new NonNullValidationRule(field.Value);
            var validacionFormato = new FormatoHoraValidationRule(field.Value, _uow);
            var tipo = form.GetField("tipo").Value;
            var rules = new List<IValidationRule>();

            if (tipo == TipoAtributoDb.HORA)
            {
                rules.Add(validacionNoNull);
                rules.Add(validacionFormato);
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateLargo(FormField field, Form form, List<ComponentParameter> parameters)
        {

            var tipo = form.GetField("tipo").Value;
            var rules = new List<IValidationRule>();

            if (tipo == TipoAtributoDb.NUMERICO)
            {
                rules.Add(new NonNullValidationRule(field.Value));
                rules.Add(new PositiveDecimalValidationRule(_culture, field.Value, false));
                decimal.TryParse(field.Value, NumberStyles.Number, _culture, out decimal valor);
                rules.Add(new NumeroDecimalMenorOIgualQueValidationRule(valor, 15));
                rules.Add(new StringMaxLengthValidationRule(field.Value, 2));
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateDecimales(FormField field, Form form, List<ComponentParameter> parameters)
        {

            var tipo = form.GetField("tipo").Value;
            var rules = new List<IValidationRule>();

            if (tipo == TipoAtributoDb.NUMERICO)
            {
                rules.Add(new NonNullValidationRule(field.Value));
                rules.Add(new PositiveDecimalValidationRule(_culture, field.Value));
                decimal.TryParse(field.Value, NumberStyles.Number, _culture, out decimal valor);
                decimal.TryParse(form.GetField("largo").Value, NumberStyles.Number, _culture, out decimal valorComparador);
                rules.Add(new NumeroDecimalMenorOIgualQueValidationRule(valor, valorComparador - 1));
                rules.Add(new StringMaxLengthValidationRule(field.Value, 1));
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateSeparador(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var validacionNoNull = new NonNullValidationRule(field.Value);
            var tipo = form.GetField("tipo").Value;
            var rules = new List<IValidationRule>();

            if (tipo == TipoAtributoDb.NUMERICO)
                rules.Add(validacionNoNull);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateTexto(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var validacionNoNull = new NonNullValidationRule(field.Value);
            var tipo = form.GetField("tipo").Value;
            var rules = new List<IValidationRule>();

            if (tipo == TipoAtributoDb.TEXTO)
                rules.Add(validacionNoNull);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateCampo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var validacionNoNull = new NonNullValidationRule(field.Value);
            var tipo = form.GetField("tipo").Value;
            var rules = new List<IValidationRule>();

            if (tipo == TipoAtributoDb.SISTEMA)
                rules.Add(validacionNoNull);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateNombre(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 50),
                    new NombreAtributoValidationRule(_uow, field.Value)
                },
            };
        }

        public virtual FormValidationGroup ValidateDescripcion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 400)
                },
            };
        }

        public virtual void AgregarParametroValidation(List<ComponentParameter> parameters, string Id, string value)
        {
            ComponentParameter genericParam = new ComponentParameter()
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
                {
                    parameters.FirstOrDefault(p => p.Id == Id).Value = genericParam.Value;
                }
                else
                {
                    parameters.Add(genericParam);
                }
            }
        }
    }
}
