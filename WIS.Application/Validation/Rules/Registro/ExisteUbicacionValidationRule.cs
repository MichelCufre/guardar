using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ExisteUbicacionValidationRule : IValidationRule
    {
        protected readonly string _idUbicacion;
        protected readonly IUnitOfWork _uow;

        public ExisteUbicacionValidationRule(IUnitOfWork uow, string idUbicacion)
        {
            this._idUbicacion = idUbicacion.ToUpper();
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.UbicacionRepository.AnyUbicacion(_idUbicacion))
                errors.Add(new ValidationError("General_Sec0_Error_UbicacionNoExiste"));

            return errors;
        }
    }
}
