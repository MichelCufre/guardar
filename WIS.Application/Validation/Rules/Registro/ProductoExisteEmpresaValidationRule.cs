using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class ProductoExisteEmpresaValidationRule : IValidationRule
    {
        protected readonly string _idEmpresa;
        protected readonly string _idProducto;
        protected readonly IUnitOfWork _uow;

        public ProductoExisteEmpresaValidationRule(IUnitOfWork uow, string idEmpresa, string idProducto)
        {
            this._idEmpresa = idEmpresa;
            this._idProducto = idProducto;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (!_uow.ProductoRepository.ExisteProducto(int.Parse(_idEmpresa), _idProducto))
                errors.Add(new ValidationError("General_Sec0_Error_ProductoNoExisteEmpresa"));

            return errors;
        }
    }
}
