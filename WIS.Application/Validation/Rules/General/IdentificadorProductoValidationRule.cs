using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.Logic;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.General
{
    public class IdentificadorProductoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _empresa;
        protected readonly string _codigoProducto;
        protected readonly string _value;

        public IdentificadorProductoValidationRule(IUnitOfWork uow, int empresa, string codigoProducto, string identificador)
        {
            this._uow = uow;
            this._empresa = empresa;
            this._codigoProducto = codigoProducto;
            this._value = identificador;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            
            var producto = this._uow.ProductoRepository.GetProductoEmpresaAsignadaUsuario(this._empresa, this._codigoProducto);

            if ((producto.IsIdentifiedByLote() || producto.IsIdentifiedBySerie()) && LIdentificador.ContieneCaracteresNoPermitidos(_uow, _value))
                errors.Add(new ValidationError("General_Sec0_Error_IdentificadorInvalido"));

            return errors;
        }
    }
}