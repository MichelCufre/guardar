using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Registro
{
    public class IdProductoNoExistenteValidationRule : IValidationRule
    {
        protected readonly string _producto;
        protected readonly string _empresa;
        protected readonly IUnitOfWork _uow;

        public IdProductoNoExistenteValidationRule(IUnitOfWork uow, string idProducto, string idEmpresa)
        {
            this._producto = idProducto;
            this._empresa = idEmpresa;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (int.TryParse(_empresa, out int valorEmpresa))
                if (_uow.ProductoRepository.ExisteProducto(valorEmpresa,this._producto))
                    errors.Add(new ValidationError("General_Sec0_Error_IdProductoExistente", new List<string> { this._producto, this._empresa}));

            return errors;
        }
    }
}
