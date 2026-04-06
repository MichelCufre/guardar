using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    class ExisteProductoProveedorValidationRule : IValidationRule
    {
        protected readonly string _idEmpresa;
        protected readonly string _idProducto;
        protected readonly string _cliente;
        protected readonly IUnitOfWork _uow;

        public ExisteProductoProveedorValidationRule(IUnitOfWork uow, string idEmpresa, string cliente, string producto)
        {
            this._idEmpresa = idEmpresa;
            this._idProducto = producto;
            this._cliente = cliente;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.ProductoRepository.AnyProductoProveedor(int.Parse(_idEmpresa), _cliente, _idProducto))
                errors.Add(new ValidationError("General_Sec0_Error_YaExisteUnProvedorParaEseProducto"));

            return errors;
        }
    }
}
