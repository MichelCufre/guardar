using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExisteTipoVehiculoValidationRule : IValidationRule
    {
        protected readonly string idTipo;
        protected readonly IUnitOfWork _uow;

        public ExisteTipoVehiculoValidationRule(IUnitOfWork uow, string idTipo)
        {
            this.idTipo = idTipo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.TipoVehiculoRepository.AnyTipo(int.Parse(idTipo)))
                errors.Add(new ValidationError("General_Sec0_Error_TipoVehiculoNoExiste"));

            return errors;
        }
    }
}
