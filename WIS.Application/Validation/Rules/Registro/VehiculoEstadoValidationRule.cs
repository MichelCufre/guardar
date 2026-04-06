using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class VehiculoEstadoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _value;

        public VehiculoEstadoValidationRule(IUnitOfWork uow, string value)
        {
            this._uow = uow;
            this._value = value;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            List<DominioDetalle> estados = this._uow.VehiculoRepository.GetEstadosEditables();

            if(!estados.Any(d => d.Id == this._value))
                errors.Add(new ValidationError("General_Sec0_Error_EstadoVehiculoNoValido"));

            return errors;
        }
    }
}