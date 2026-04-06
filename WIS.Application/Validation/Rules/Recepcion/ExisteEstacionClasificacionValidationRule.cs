using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class ExisteEstacionClasificacionValidationRule : IValidationRule
    {
        protected readonly string _codigoEstacion;
        protected readonly IUnitOfWork _uow;

        public ExisteEstacionClasificacionValidationRule(IUnitOfWork uow, string codigoEstacion)
        {
            this._codigoEstacion = codigoEstacion;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (int.TryParse(this._codigoEstacion, out int codigoEstacion))
                if (!this._uow.MesaDeClasificacionRepository.AnyEstacionDeClasificacion(codigoEstacion))
                    errors.Add(new ValidationError("General_Sec0_Error_NoExisteEstacionDeClasificacion"));

            return errors;
        }
    }
}