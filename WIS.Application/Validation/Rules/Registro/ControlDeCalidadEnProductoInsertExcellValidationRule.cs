using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ControlDeCalidadEnProductoInsertExcellValidationRule : IValidationRule

    {
        protected readonly string _empresa;
        protected readonly string _producto;
        protected readonly string _celdaEmpresa;
        protected readonly string _celdaProducto;
        protected readonly IUnitOfWork _uow;
        public ControlDeCalidadEnProductoInsertExcellValidationRule(IUnitOfWork uow, string empresa, string producto, string celdaEmpresa, string celdaProducto)

        {
            this._empresa = empresa;
            this._producto = producto;
            this._celdaEmpresa = celdaEmpresa;
            this._celdaProducto = celdaProducto;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            if (!_celdaEmpresa.Equals(_empresa))
                errors.Add(new ValidationError("REG602_Sec0_Error_EmpresaParametro"));


            if (!_celdaProducto.Equals(_producto))
                errors.Add(new ValidationError("REG602_Sec0_Error_ProductoParametro"));

            return errors;
        }
    }
}
