using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class TipoReferenciaRecepcionExistenteValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public TipoReferenciaRecepcionExistenteValidationRule(IUnitOfWork uow, string tipoReferencia)
        {
            this._value = tipoReferencia;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.ReferenciaRecepcionRepository.AnyReferenciaRecapcionTipo(this._value))
                errors.Add(new ValidationError("General_Sec0_Error_NoExisteTipoRecepcionReferencia"));

            return errors;
        }
    }
}