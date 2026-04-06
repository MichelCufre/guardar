using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Impresion
{
    public class ExisteContenedorValidationRule : IValidationRule
    {
        protected readonly int _contenedor;
        protected readonly IUnitOfWork _uow;

        public ExisteContenedorValidationRule(int nuContenedor, IUnitOfWork uow)
        {
            this._contenedor = nuContenedor;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.ContenedorRepository.ExisteContenedor(_contenedor))
                errors.Add(new ValidationError("General_Sec0_Error_ContenedorNoExiste"));

            return errors;
        }
    }
}
