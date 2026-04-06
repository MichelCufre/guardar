using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules
{
    public class PalletYaDefinido : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly string _valueString;
        protected readonly int _codigoEmpresa;
        protected readonly string _codigoProduco;

        public PalletYaDefinido(IUnitOfWork uow, string valueString, int codigoEmpresa, string codigoProducto)
        {
            this._uow = uow;
            this._valueString = valueString;
            this._codigoEmpresa = codigoEmpresa;
            this._codigoProduco = codigoProducto;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.ProductoRepository.ExistePalletAsignadoProducto(_codigoEmpresa, _codigoProduco , short.Parse(_valueString)))
                errors.Add(new ValidationError("REG605_Sec0_Error_ProductoYaTieneEstePalet"));

            return errors;
        }
    }
}
