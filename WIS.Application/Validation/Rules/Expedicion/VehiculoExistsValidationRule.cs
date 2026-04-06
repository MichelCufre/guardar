using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Expedicion
{
    public class VehiculoExistsValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _codigoVehiculo;

        public VehiculoExistsValidationRule(IUnitOfWork uow, string codigoVehiculo)
        {
            this._uow = uow;
            this._codigoVehiculo = codigoVehiculo;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (string.IsNullOrEmpty(this._codigoVehiculo))
                return errors;

            if (!this._uow.VehiculoRepository.AnyVehiculo(int.Parse(this._codigoVehiculo)))
                errors.Add(new ValidationError("Vehiculo no existe"));

            return errors;
        }
    }
}
