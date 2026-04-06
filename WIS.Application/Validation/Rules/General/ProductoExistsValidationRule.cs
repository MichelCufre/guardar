using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class ProductoExistsValidationRule : IValidationRule
    {
        protected readonly string _valueEmpresa;
        protected readonly string _valueProducto;
        protected readonly IUnitOfWork _uow;

        public ProductoExistsValidationRule(IUnitOfWork uow, string valueEmpresa, string valueProducto)
        {
            this._valueEmpresa = valueEmpresa;
            this._valueProducto = valueProducto;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!int.TryParse(this._valueEmpresa, out int cdEmpresa))
            {
                errors.Add(new ValidationError("REG602_Sec0_Error_EmpresaNecesariaProducto"));
                return errors;
            }

            if (!this._uow.ProductoRepository.ExisteProducto(cdEmpresa, this._valueProducto))
                errors.Add(new ValidationError("General_Sec0_Error_Error05"));

            return errors;
        }
    }
}
