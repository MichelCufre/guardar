using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class CodigoFacturacionExistenteValidationRule : IValidationRule
    {
        protected readonly string _componente;
        protected readonly IUnitOfWork _uow;

        public CodigoFacturacionExistenteValidationRule(IUnitOfWork uow, string componente)
        {
            this._componente = componente;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

                if (!_uow.FacturacionRepository.AnyFacturacion(this._componente))
                    errors.Add(new ValidationError("REG009_Sec0_Error_Er007_NoExisteCodigoComponente", new List<string> { this._componente}));

            return errors;
        }
    }
}
