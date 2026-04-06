using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Stock
{
    public class TraspasoEmpresasPreparacionPreparadoValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _nuPreparacion;

        public TraspasoEmpresasPreparacionPreparadoValidationRule(IUnitOfWork uow, int nuPreparacion)
        {
            this._uow = uow;
            this._nuPreparacion = nuPreparacion;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();
            var datosPreparacion = this._uow.TraspasoEmpresasRepository.GetPreparacionPreparadoValida(this._nuPreparacion);
            if (datosPreparacion == null)
            {
                errors.Add(new ValidationError("STO820_Sec0_Error_PreparacionIncompatible"));
                return errors;
            }
            else if (datosPreparacion.CantidadSaldoSinTrabajar > 0)
            {
                errors.Add(new ValidationError("STO820_Sec0_Error_PreparacionConCantidadSaldoSinTrabajar"));
                return errors;
            }
            else if (datosPreparacion.CantidadPedidoNoTraspaso > 0)
            {
                errors.Add(new ValidationError("STO820_Sec0_Error_PreparacionCantidadPedidoNoTraspaso"));
                return errors;
            }
            else if (datosPreparacion.CantidadPendiente > 0)
            {
                errors.Add(new ValidationError("STO820_Sec0_Error_PreparacionCantidadPendiente"));
                return errors;
            }

            return errors;
        }
    }
}