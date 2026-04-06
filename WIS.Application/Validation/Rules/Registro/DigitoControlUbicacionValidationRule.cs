using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    /// <summary>
    /// Se controla que no exista otra ubicación en la misma columna con el caracter de control indicado
    /// Se controla que el caracter no sea I o O
    /// </summary>
    public class DigitoControlUbicacionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly int _valueColumna;
        protected readonly IUnitOfWork _uow;

        public DigitoControlUbicacionValidationRule(IUnitOfWork uow, string control, int columna)
        {
            this._value = control;
            this._valueColumna = columna;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            string[] digitosNoUtilizables = new string[] { "I", "O" };

            if (digitosNoUtilizables.Contains(_value))
            {
                errors.Add(new ValidationError("General_Sec0_Error_CaracteresIOControlUbicacionNoUtilizable"));
            }

            var ubicaciones = this._uow.UbicacionRepository.GetUbicacionColumna(this._valueColumna);

            if (ubicaciones.Select(s => s.CodigoControl).Contains(this._value))
            {
                errors.Add(new ValidationError("General_Sec0_Error_CaracterControlExistenteEnColumna"));
            }

            return errors;
        }
    }
}