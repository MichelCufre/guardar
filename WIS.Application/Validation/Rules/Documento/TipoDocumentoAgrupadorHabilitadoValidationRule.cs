using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Documento
{
    public class TipoDocumentoAgrupadorHabilitadoValidationRule : IValidationRule
    {
        protected readonly string _tipoValue;
        protected readonly IUnitOfWork _uow;

        public TipoDocumentoAgrupadorHabilitadoValidationRule(string tipoValue, IUnitOfWork uow)
        {
            this._tipoValue = tipoValue;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var agrupadorTipo = this._uow.DocumentoRepository.GetDocumentoAgrupadorTipo(this._tipoValue);

            if (agrupadorTipo == null)
            {
                errors.Add(new ValidationError("General_Sec0_Error_Error49"));
            }
            else if (!agrupadorTipo.Habilitado)
            {
                errors.Add(new ValidationError("General_Sec0_Error_Error50"));
            }

            return errors;
        }
    }
}
