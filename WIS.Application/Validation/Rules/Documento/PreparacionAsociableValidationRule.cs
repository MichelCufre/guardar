using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class PreparacionAsociableValidationRule : IValidationRule
    {
        protected readonly string _tpOperativa;
        protected readonly string _nuPreparacion;
        protected readonly IUnitOfWork _uow;

        public PreparacionAsociableValidationRule(string nuPreparacion, string tpOperativa, IUnitOfWork uow)
        {
            this._nuPreparacion = nuPreparacion;
            this._tpOperativa = tpOperativa;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(this._tpOperativa) && !string.IsNullOrEmpty(this._nuPreparacion))
            {
                if (!this._uow.PreparacionRepository.IsPreparacionAsociable(this._tpOperativa, int.Parse(this._nuPreparacion)))
                    errors.Add(new ValidationError("General_Sec0_Error_PreparacionAsociada"));
            }

            return errors;
        }
    }
}
