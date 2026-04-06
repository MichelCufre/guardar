using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation.Rules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Security;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules.Parametrizacion
{
    public class PAR400ModificarAtributoFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly IIdentityService _identity;
        protected readonly IFormatProvider _culture;

        public PAR400ModificarAtributoFormValidationModule(
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
            var tipoAtributo = _uow.AtributoRepository.GetTipoAtributoByDescription(field.Value);
            this.AgregarParametroValidation(parameters, "tipo", tipoAtributo.Id);
        }

        public virtual FormValidationGroup ValidateDominio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var validacionNoNull = new NonNullValidationRule(field.Value);
            var validacionDominio = new DominioValidationRule(field.Value, this._uow);
            var valorSelect = form.GetField("tipo").Value;
            var tipoAtributo = _uow.AtributoRepository.GetTipoAtributoByDescription(valorSelect);
            var rules = new List<IValidationRule>();

            if (tipoAtributo.Id == TipoAtributoDb.DOMINIO)
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
            var valorSelect = form.GetField("tipo").Value;
            var tipoAtributo = _uow.AtributoRepository.GetTipoAtributoByDescription(valorSelect);
            var rules = new List<IValidationRule>();

            if (tipoAtributo.Id == TipoAtributoDb.FECHA)
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
            var valorSelect = form.GetField("tipo").Value;
            var tipoAtributo = _uow.AtributoRepository.GetTipoAtributoByDescription(valorSelect);
            var rules = new List<IValidationRule>();

            if (tipoAtributo.Id == TipoAtributoDb.HORA)
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
            var valorSelect = form.GetField("tipo").Value;
            var tipoAtributo = _uow.AtributoRepository.GetTipoAtributoByDescription(valorSelect);
            var rules = new List<IValidationRule>();

            if (tipoAtributo.Id == TipoAtributoDb.FECHA)
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
            var valorSelect = form.GetField("tipo").Value;
            var validacionFormato = new FormatoHoraValidationRule(field.Value, _uow);
            var tipoAtributo = _uow.AtributoRepository.GetTipoAtributoByDescription(valorSelect);
            var rules = new List<IValidationRule>();

            if (tipoAtributo.Id == TipoAtributoDb.HORA)
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
            var validacionNoNull = new NonNullValidationRule(field.Value);
            var validacionPositiveShort = new PositiveShortValidationRule(field.Value);
            var validacionMaxLenght = new StringMaxLengthValidationRule(field.Value, 2);
            var valorSelect = form.GetField("tipo").Value;
            var tipoAtributo = _uow.AtributoRepository.GetTipoAtributoByDescription(valorSelect);
            var rules = new List<IValidationRule>();

            if (tipoAtributo.Id == TipoAtributoDb.NUMERICO)
            {
                rules.Add(validacionNoNull);
                rules.Add(validacionPositiveShort);
                rules.Add(validacionMaxLenght);
            }

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateDecimales(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var validacionNoNull = new NonNullValidationRule(field.Value);
            var validacionPositiveShort = new PositiveShortValidationRule(field.Value);
            var validacionMaxLenght = new StringMaxLengthValidationRule(field.Value, 1);
            var valorSelect = form.GetField("tipo").Value;
            var tipoAtributo = _uow.AtributoRepository.GetTipoAtributoByDescription(valorSelect);
            var rules = new List<IValidationRule>();

            if (tipoAtributo.Id == TipoAtributoDb.NUMERICO)
            {
                rules.Add(validacionNoNull);
                rules.Add(validacionPositiveShort);
                rules.Add(validacionMaxLenght);
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
            var valorSelect = form.GetField("tipo").Value;
            var tipoAtributo = _uow.AtributoRepository.GetTipoAtributoByDescription(valorSelect);
            var rules = new List<IValidationRule>();

            if (tipoAtributo.Id == TipoAtributoDb.NUMERICO)
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
            var valorSelect = form.GetField("tipo").Value;
            var tipoAtributo = _uow.AtributoRepository.GetTipoAtributoByDescription(valorSelect);
            var rules = new List<IValidationRule>();

            if (tipoAtributo.Id == TipoAtributoDb.TEXTO)
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
            var valorSelect = form.GetField("tipo").Value;
            var tipoAtributo = _uow.AtributoRepository.GetTipoAtributoByDescription(valorSelect);
            var rules = new List<IValidationRule>();

            if (tipoAtributo.Id == TipoAtributoDb.SISTEMA)
                rules.Add(validacionNoNull);

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = rules,
            };
        }

        public virtual FormValidationGroup ValidateNombre(FormField field, Form form, List<ComponentParameter> parameters)
        {
            int.TryParse(parameters.Find(x => x.Id == "codigoAtributo").Value, out int codigoAtributo);

            return new FormValidationGroup
            {
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 50),
                    new NombreAtributoValidationRule(_uow, field.Value,codigoAtributo)
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
