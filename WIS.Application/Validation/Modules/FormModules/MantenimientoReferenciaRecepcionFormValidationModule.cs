using System.Collections.Generic;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.General;
using WIS.Application.Validation.Rules.Recepcion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class MantenimientoReferenciaRecepcionFormValidationModule : FormValidationModule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _idUsuario;

        public MantenimientoReferenciaRecepcionFormValidationModule(IUnitOfWork uow, int idUsuario)
        {
            this._uow = uow;
            this._idUsuario = idUsuario;

            this.Schema = new FormValidationSchema
            {
                ["idEmpresa"] = this.ValidateIdEmpresa,
                ["tipoReferencia"] = this.ValidateTipoReferencia,
                ["codigoInternoAgente"] = this.ValidateCodigoInternoAgente,
                ["codigo"] = this.ValidateCodigo,
                ["numeroPredio"] = this.ValidateNumeroPredio,
                ["fechaVencimiento"] = this.ValidateFechaVencimiento,
                ["fechaEmitida"] = this.ValidateFechaEmitida,
                ["fechaEntrega"] = this.ValidateFechaEntrega,
                ["memo"] = this.ValidateMemo,
                ["anexo1"] = this.ValidateAnexo1,
                ["anexo2"] = this.ValidateAnexo2,
                ["anexo3"] = this.ValidateAnexo3,
                ["moneda"] = this.ValidateMoneda,
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
            };
        }
        public virtual FormValidationGroup ValidateCodigoInternoAgente(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 10),
                    new ExisteClienteValidationRule(_uow,field.Value, form.GetField("idEmpresa").Value),

                },
                Dependencies = { "idEmpresa" }
            };

        }
        public virtual FormValidationGroup ValidateTipoReferencia(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 6),
                    new TipoReferenciaRecepcionExistenteValidationRule(_uow,field.Value),
                    new TipoReferenciaRecepcionEnTipoRecepcionValidationRule(_uow,field.Value),
                    new AgenteTipoReferenciaRecepcionValidationRule(_uow, form.GetField("codigoInternoAgente").Value,field.Value )
                },
                Dependencies = { "codigoInternoAgente" }
            };

        }

        public virtual FormValidationGroup ValidateCodigo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (field.ReadOnly) return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 20),
                    new NumeroReferenciaNoExistenteValidationRule(_uow, form.GetField("idEmpresa").Value, form.GetField("codigoInternoAgente").Value, form.GetField("tipoReferencia").Value, field.Value)
                },
                Dependencies = { "idEmpresa", "codigoInternoAgente", "tipoReferencia" }
            };

        }
        public virtual FormValidationGroup ValidateNumeroPredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule> {
                    new NonNullValidationRule(field.Value),
                    new PredioUsuarioExistenteValidationRule(this._uow, this._idUsuario ,field.Value)
                },
            };
        }
        public virtual FormValidationGroup ValidateFechaVencimiento(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new DateTimeValidationRule(field.Value)
                    }
            };

        }
        public virtual FormValidationGroup ValidateFechaEmitida(FormField field, Form form, List<ComponentParameter> parameters)
        {

            List<IValidationRule> reglas = new List<IValidationRule>();

            reglas.Add(new NonNullValidationRule(field.Value));
            reglas.Add(new DateTimeValidationRule(field.Value));

            if (!string.IsNullOrEmpty( form.GetField("fechaEntrega").Value))
            {
                reglas.Add(new DateTimeGreaterThanValidationRule(form.GetField("fechaEntrega").Value, field.Value, "General_Sec0_Error_ReferenciaRecepcionFechaEmitidaMenorFechaEntrega"));
            }

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = reglas
            };

        }
        public virtual FormValidationGroup ValidateFechaEntrega(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        //new NonNullValidationRule(field.Value),
                        new DateTimeValidationRule(field.Value),
                        new DateTimeGreaterThanValidationRule(field.Value, form.GetField("fechaEmitida").Value, "General_Sec0_Error_ReferenciaRecepcionFechaEmitidaMayorFechaEntrega")
                    },
                Dependencies = { "fechaEmitida" }
            };

        }
        public virtual FormValidationGroup ValidateMemo(FormField field, Form form, List<ComponentParameter> parameters)
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
        public virtual FormValidationGroup ValidateAnexo1(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                        new StringMaxLengthValidationRule(field.Value, 200)
                    }
            };
        }
        public virtual FormValidationGroup ValidateAnexo2(FormField field, Form form, List<ComponentParameter> parameters)
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
        public virtual FormValidationGroup ValidateAnexo3(FormField field, Form form, List<ComponentParameter> parameters)
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
        public virtual FormValidationGroup ValidateMoneda(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = false,
                Rules = new List<IValidationRule>
                    {
                    new ExisteMonedaValidationRule(this._uow, field.Value)
                    }
            };
        }

    }
}
