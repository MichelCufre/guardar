using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Validation;

namespace WIS.Application.Validation.Rules.Stock
{
    public class TraspasoEmpresasPreparacionPendienteValidationRule : IValidationRule
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _nuPreparacion;

        public TraspasoEmpresasPreparacionPendienteValidationRule(IUnitOfWork uow, int nuPreparacion)
        {
            this._uow = uow;
            this._nuPreparacion = nuPreparacion;
        }

        public virtual List<IValidationError> Validate()
        {
            var errors = new List<IValidationError>();

            var datosPreparacion = this._uow.TraspasoEmpresasRepository.GetPreparacionPendienteValida(this._nuPreparacion);
            if (datosPreparacion == null)
            {
                errors.Add(new ValidationError("STO820_Sec0_Error_PreparacionIncompatible"));
                return errors;
            }
            else if (datosPreparacion.CantidadLoteAuto > 0)
            {
                errors.Add(new ValidationError("STO820_Sec0_Error_PreparacionConLoteAuto"));
                return errors;
            }
            else if (datosPreparacion.CantidadDetallePickingPorAtributo > 0)
            {
                errors.Add(new ValidationError("STO820_Sec0_Error_PreparacionConCantidadDetallePickingPorAtributo"));
                return errors;
            }
            else if (datosPreparacion.CantidadDetallleLpn != datosPreparacion.CantidadDetallePickingPorLpn)
            {
                errors.Add(new ValidationError("STO820_Sec0_Error_PreparacionConLpnNoReservadoTotalmente"));
                return errors;
            }
            else if (datosPreparacion.CantidadPickingMayorSuelto > 0)
            {
                errors.Add(new ValidationError("STO820_Sec0_Error_PreparacionConCantidadPickingMayorSuelto"));
                return errors;
            }
            else if (datosPreparacion.CantidadPickingReabastecimientoUbicacion > 0)
            {
                errors.Add(new ValidationError("STO820_Sec0_Error_PreparacionConCantidadPickingReabasteciemiento"));
                return errors;
            }
            else if (datosPreparacion.CantidadPreparada > 0)
            {
                errors.Add(new ValidationError("STO820_Sec0_Error_PreparacionConCantidadPreparada"));
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

            return errors;
        }
    }
}