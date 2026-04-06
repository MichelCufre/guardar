using System;
using System.Collections.Generic;
using System.Text;
using WIS.Application.Validation.Rules;
using WIS.Application.Validation.Rules.Registro;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.FormComponent;
using WIS.FormComponent.Validation;
using WIS.Validation;

namespace WIS.Application.Validation.Modules.FormModules
{
    public class UpdateVehiculoFormValidationModule : FormValidationModule
    {
        protected readonly int _userId;
        protected readonly string _userPredio;
        protected readonly IUnitOfWork _uow;
        protected readonly IFormatProvider _formatProvider;

        public UpdateVehiculoFormValidationModule(IUnitOfWork uow, int userId, string userPredio, IFormatProvider culture)
        {
            this._uow = uow;
            this._formatProvider = culture;
            this._userId = userId;
            this._userPredio = userPredio;

            this.Schema = new FormValidationSchema
            {
                ["estado"] = this.ValidateEstado,
                ["descripcion"] = this.ValidateDescripcion,
                ["transportista"] = this.ValidateTransportista,
                ["placa"] = this.ValidateMatricula,
                ["tipo"] = this.ValidateTipoVehiculo,
                ["predio"] = this.ValidatePredio,
                ["disponibilidadDesde"] = this.ValidateDisponibilidadDesde,
                ["disponibilidadHasta"] = this.ValidateDisponibilidadHasta
            };
        }

        public virtual FormValidationGroup ValidateEstado(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new VehiculoEstadoValidationRule(this._uow, field.Value)
                }
            };
        }
        public virtual FormValidationGroup ValidateDescripcion(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 100)
                }
            };
        }
        public virtual FormValidationGroup ValidateTransportista(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 10),
                   new PositiveIntValidationRule(field.Value),
                   new ExisteTransportadoraValidationRule(this._uow, field.Value)
                }
            };
        }
        public virtual FormValidationGroup ValidateMatricula(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                    new NonNullValidationRule(field.Value),
                    new StringMaxLengthValidationRule(field.Value, 15)
                }
            };
        }
        public virtual FormValidationGroup ValidateTipoVehiculo(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new StringMaxLengthValidationRule(field.Value, 10),
                   new ExisteTipoVehiculoValidationRule(this._uow, field.Value)
                }
            };
        }
        public virtual FormValidationGroup ValidatePredio(FormField field, Form form, List<ComponentParameter> parameters)
        {
            if (string.IsNullOrEmpty(field.Value))
                return null;

            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new StringMaxLengthValidationRule(field.Value, 10),
                   new ExistePredioValidationRule(this._uow, this._userId, this._userPredio, field.Value)
                }
            };
        }
        public virtual FormValidationGroup ValidateDisponibilidadDesde(FormField field, Form form, List<ComponentParameter> parameters)
        {
            return new FormValidationGroup
            {
                BreakValidationChain = true,
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new TimeSpanValidationRule(field.Value, this._formatProvider),
                }
            };
        }
        public virtual FormValidationGroup ValidateDisponibilidadHasta(FormField field, Form form, List<ComponentParameter> parameters)
        {
            var group = new FormValidationGroup
            {
                BreakValidationChain = true,
                Dependencies = { "disponibilidadDesde" },
                Rules = new List<IValidationRule>
                {
                   new NonNullValidationRule(field.Value),
                   new TimeSpanValidationRule(field.Value, this._formatProvider)
                }
            };

            return group;
        }
    }
}
