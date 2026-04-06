using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExisteHerramientaValidationRule : IValidationRule
    {
        protected readonly string _cdHerramienta;
        protected readonly IUnitOfWork _uow;

        public ExisteHerramientaValidationRule(IUnitOfWork uow, string cdHerramienta)
        {
            this._cdHerramienta = cdHerramienta;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!short.TryParse(_cdHerramienta, out short parseValue))
                errors.Add(new ValidationError("General_Sec0_Error_Error14"));
            else if (!this._uow.EquipoRepository.AnyHerramienta(parseValue))
                errors.Add(new ValidationError("REG010_msg_Error_HerramientaNoExiste"));

            return errors;
        }
    }
}
