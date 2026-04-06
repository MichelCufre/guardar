using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Recepcion
{
    public class TipoReferenciaRecepcionEnTipoRecepcionValidationRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public TipoReferenciaRecepcionEnTipoRecepcionValidationRule(IUnitOfWork uow, string tipoReferencia)
        {
            this._value = tipoReferencia;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!this._uow.ReferenciaRecepcionRepository.AnyRecepcionTipo(this._value))
                errors.Add(new ValidationError("General_Sec0_Error_NoExisteTipoReferenciaAsigandoATipoRecepcion"));

            return errors;
        }
    }
}