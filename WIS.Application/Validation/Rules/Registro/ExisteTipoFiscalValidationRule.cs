using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExisteTipoFiscalValidationRule : IValidationRule
    {
        protected readonly string _valueClase;
        protected readonly IUnitOfWork _uow;

        public ExisteTipoFiscalValidationRule(IUnitOfWork uow, string tipoAgente)
        {
            this._valueClase = tipoAgente;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            List<DominioDetalle> tiposAgente = this._uow.AgenteRepository.GetTiposAgenteFiscales();

            if (!tiposAgente.Any(d => d.Id == _valueClase))
                errors.Add(new ValidationError("General_Sec0_Error_NoExisteTipoFiscal"));

            return errors;
        }
    }
}