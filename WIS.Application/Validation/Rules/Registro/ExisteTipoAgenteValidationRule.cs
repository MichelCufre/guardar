using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExisteTipoAgenteValidationRule : IValidationRule
    {
        protected readonly string _valueClase;
        protected readonly IUnitOfWork _uow;

        public ExisteTipoAgenteValidationRule(IUnitOfWork uow, string tipoAgente)
        {
            this._valueClase = tipoAgente;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            List<DominioDetalle> tiposAgente = this._uow.AgenteRepository.GetTiposAgente();

            if (!tiposAgente.Any(d => d.Valor == _valueClase))
                errors.Add(new ValidationError("General_Sec0_Error_NoExisteTipoAgente"));

            return errors;
        }
    }
}