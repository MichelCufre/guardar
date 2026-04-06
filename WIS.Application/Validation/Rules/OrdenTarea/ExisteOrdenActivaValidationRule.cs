using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.OrdenTarea
{
    public class ExisteOrdenActivaValidationRule : IValidationRule
    {
        protected readonly string _nuOrden;
        protected readonly IUnitOfWork _uow;

        public ExisteOrdenActivaValidationRule(IUnitOfWork uow, string nuOrden)
        {
            this._nuOrden = nuOrden;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var nuOrden = int.Parse(this._nuOrden);

            var errors = new List<IValidationError>();

            if (!_uow.OrdenRepository.AnyOrdenActiva(nuOrden))
                errors.Add(new ValidationError("General_ORT090_Error_OrdenActivaNoExiste"));

            return errors;
        }
    }
}
