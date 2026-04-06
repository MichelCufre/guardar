using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    class ExisteCodigoExternoProductoProveedorValidationRule : IValidationRule
    {
        protected readonly string _codigoExterno;
        protected readonly string _empresa;
        protected readonly string _cliente;
        protected readonly string _producto;
        protected readonly IUnitOfWork _uow;

        public ExisteCodigoExternoProductoProveedorValidationRule(IUnitOfWork uow, string codigoExterno, string empresa, string cliente, string producto)
        {
            this._codigoExterno = codigoExterno;
            this._uow = uow;
            this._empresa = empresa;
            this._cliente = cliente;
            this._producto = producto;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!string.IsNullOrEmpty(_empresa) && !string.IsNullOrEmpty(_cliente) && !string.IsNullOrEmpty(_producto))
            {
                if (_uow.ProductoRepository.AnyCodigoExternoDistintoProductoProveedor(int.Parse(_empresa), _cliente, _codigoExterno, _producto))
                    errors.Add(new ValidationError("REG015_Sec0_Error_CodigoExternoExiste"));
            }

            return errors;
        }
    }
}
