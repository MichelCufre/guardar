using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Facturacion
{
    public class ExisteListaPrecioEmpresaValidationRule : IValidationRule
    {
        protected readonly int _empresa;
        protected readonly int _listaPecio;
        protected readonly IUnitOfWork _uow;

        public ExisteListaPrecioEmpresaValidationRule(IUnitOfWork uow, int valueEmpresa, int valueListaPrecio)
        {
            this._empresa = valueEmpresa;
            this._listaPecio = valueListaPrecio;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.EmpresaRepository.AnyEmpresaListaPrecio(this._empresa, this._listaPecio))
                errors.Add(new ValidationError("General_Sec0_Error_PrecioListaEmpresaExiste"));

            return errors;
        }
    }
}
