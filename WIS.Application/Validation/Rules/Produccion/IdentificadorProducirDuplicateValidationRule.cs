using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Produccion
{
    public class IdentificadorProducirDuplicateValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _ubicacion;
        protected readonly int _empresa;
        protected readonly int _orden;
        protected readonly string _producto;

        public IdentificadorProducirDuplicateValidationRule(IUnitOfWork uow, string ubicacion, int empresa, int orden, string producto)
        {
            this._uow = uow;
            this._ubicacion = ubicacion;
            this._empresa = empresa;
            this._orden = orden;
            this._producto = producto;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if (this._uow.IdentificadorProducirRepository.AnyIdentificador(this._ubicacion, this._empresa, this._orden, this._producto))
                errors.Add(new ValidationError("PRD100_Sec0_error_IdentificadorOrdenDuplicado"));

            return errors;
        }
    }
}
