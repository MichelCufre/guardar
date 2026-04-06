using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Persistence.Database;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class EsTipoContenedorPredefinidoValidationRule : IValidationRule
    {
        protected readonly string _tipo;
        protected readonly IUnitOfWork _uow;

        public EsTipoContenedorPredefinidoValidationRule(IUnitOfWork uow, string tipo)
        {
            this._tipo = tipo;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            List<IValidationError> errors = new List<IValidationError>();

            if (this._uow.ContenedorRepository.TipoContenedorPredefinido(_tipo))
            {
                errors.Add(new ValidationError("General_Sec0_Error_ImpModule_FuncionalidadContPredifNoHab"));
            }

            return errors;
        }
    }
}
