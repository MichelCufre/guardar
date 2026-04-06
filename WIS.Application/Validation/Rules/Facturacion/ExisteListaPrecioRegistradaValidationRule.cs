using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Facturacion
{
    public class ExisteListaPrecioRegistradaValidationRule : IValidationRule
    {
        protected readonly string _idLista;
        protected readonly IUnitOfWork _uow;

        public ExisteListaPrecioRegistradaValidationRule(IUnitOfWork uow, string valueIdLista)
        {
            this._idLista = valueIdLista;
            this._uow = uow;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            if (_uow.ListaPrecioRepository.ExisteListaPrecio(int.Parse(this._idLista)))
                errors.Add(new ValidationError("General_Sec0_Error_ListaPrecioExiste"));

            return errors;
        }
    }
}
