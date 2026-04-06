using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class CodigoFamiliaProductoRule : IValidationRule
    {
        protected readonly string _value;
        protected readonly IUnitOfWork _uow;

        public CodigoFamiliaProductoRule(IUnitOfWork uow, string value)
        {
            this._value = value;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (this._uow.ProductoFamiliaRepository.AnyFamiliaProducto(int.Parse(this._value)))
                errors.Add(new ValidationError("General_Sec0_Error_CodigoFamiliaProductoExistente"));

            return errors;
        }
    }
}
