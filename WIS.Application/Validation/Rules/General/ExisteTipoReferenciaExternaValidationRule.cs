using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ExisteTipoReferenciaExternaValidationRule : IValidationRule
    {
        protected readonly string _tipoReferenciaExternaValue;
        protected readonly string _tipoDocumento;
        protected readonly IUnitOfWork _uow;

        public ExisteTipoReferenciaExternaValidationRule(IUnitOfWork uow, 
            string tipoDocumento,
            string tipoReferenciaExternaValue)
        {
            this._tipoDocumento = tipoDocumento;
            this._tipoReferenciaExternaValue = tipoReferenciaExternaValue;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.TipoReferenciaExternaRepository.AnyTipoReferenciaExterna(this._tipoDocumento, this._tipoReferenciaExternaValue))
                errors.Add(new ValidationError("General_Sec0_Error_TipoReferenciaExternaNoExiste"));

            return errors;
        }
    }
}
