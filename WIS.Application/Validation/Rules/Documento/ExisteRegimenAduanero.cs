using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class ExisteRegimenAduanero : IValidationRule
    {
        protected readonly string _valueRegimen;
        protected readonly IUnitOfWork _uow;

        public ExisteRegimenAduanero(IUnitOfWork uow, string valueTRegimen)
        {
            this._valueRegimen = valueTRegimen;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var cdRegimenAduanero = int.Parse(this._valueRegimen);

            var errors = new List<IValidationError>();

            if (!this._uow.RegimenAduaneroRepository.AnyRegimenAduanero(cdRegimenAduanero))
                errors.Add(new ValidationError("General_Sec0_Error_RegimenAduaneroNoExiste"));

            return errors;
        }
    }
}
