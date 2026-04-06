using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.OrdenTarea
{
    class ExisteEquipoValidationRule : IValidationRule
    {
        protected readonly int _cdEquipo;
        protected readonly IUnitOfWork _uow;


        public ExisteEquipoValidationRule(IUnitOfWork uow, int cdEquipo)
        {
            this._uow = uow;
            this._cdEquipo = cdEquipo;

        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.EquipoRepository.AnyEquipoManual(this._cdEquipo))
                errors.Add(new ValidationError("General_Sec0_Error_EquipoNoExiste"));

            return errors;
        }
    }
}
