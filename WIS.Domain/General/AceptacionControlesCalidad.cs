using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;

namespace WIS.Domain.General
{
    public class AceptacionControlesCalidad
    {
        protected readonly List<int> _controles;
        protected readonly IUnitOfWork _uow;
        protected readonly int _userId;

        public AceptacionControlesCalidad(IUnitOfWork uow, List<int> idControles, int userId)
        {
            this._uow = uow;
            this._controles = idControles;
            this._userId = userId;
        }

        public AceptacionControlesCalidad(IUnitOfWork uow, int userId)
        {
            this._uow = uow;
            this._userId = userId;
        }

        public virtual void AceptarControlesUbicacion(List<ControlDeCalidadPendiente> controles = null)
        {
            controles ??= this._uow.ControlDeCalidadRepository.GetControles(_controles);

            foreach (var control in controles)
            {
                control.Aceptar(_userId);

                _uow.ControlDeCalidadRepository.UpdateControlPendiente(control);
                _uow.SaveChanges();

                if (control.NroLPN != null && control.IdLpnDet != null && !_uow.ControlDeCalidadRepository.AnyControlDeCalidadPendienteDetalleLpn(control.NroLPN.Value, control.IdLpnDet.Value))
                {
                    var detLpn = _uow.ManejoLpnRepository.GetDetalleLpnByIdDetalle(control.NroLPN.Value, control.IdLpnDet.Value);

                    if (detLpn != null)
                    {
                        detLpn.IdCtrlCalidad = EstadoControlCalidad.Controlado;
                        detLpn.NumeroTransaccion = _uow.GetTransactionNumber();
                        _uow.ManejoLpnRepository.UpdateDetalleLpn(detLpn);
                    }
                }
                else if (!_uow.ControlDeCalidadRepository.AnyControlPendiente(control.Stock, control.Id))
                {
                    control.Stock.SetControlado();
                    control.Stock.NumeroTransaccion = _uow.GetTransactionNumber();
                    control.Stock.FechaModificacion = DateTime.Now;
                    _uow.ControlDeCalidadRepository.UpdateControlCalidadStock(control.Stock);
                }
                _uow.SaveChanges();
            }
        }

        public virtual void CargarObjetosBulkAceptarControlesUbicacion(
            List<ControlDeCalidadPendiente> controlesNuevos,
            List<ControlDeCalidadPendiente> controles,
            List<ControlDeCalidadPendiente> toApproveDisponibles,
            IControlCalidadServiceContext context,
            List<LpnDetalle> toUpdateDetallesLpn,
            List<Stock> toUpdateStock)
        {
            List<ControlDeCalidadPendiente> toApproveFinales = toApproveDisponibles.Where(r => !controles.Any(a => a.Id == r.Id)).ToList();

            foreach (var control in controles)
            {
                control.Aceptar(this._userId);

                if (control.NroLPN != null && control.IdLpnDet != null)
                {
                    if (!controlesNuevos.Any(a => a.Ubicacion == control.Stock.Ubicacion && a.Producto == control.Stock.Producto && a.Identificador == control.Stock.Identificador && a.Empresa == control.Stock.Empresa && a.Faixa == control.Stock.Faixa && a.NroLPN == control.NroLPN && a.IdLpnDet == control.IdLpnDet && !a.Aceptado) && !toApproveFinales.Any(a => a.Ubicacion == control.Stock.Ubicacion && a.Producto == control.Stock.Producto && a.Identificador == control.Stock.Identificador && a.Empresa == control.Stock.Empresa && a.Faixa == control.Stock.Faixa && a.NroLPN == control.NroLPN && a.IdLpnDet == control.IdLpnDet && !a.Aceptado))
                    {
                        var detLpn = context.GetDetalleLpn((long)control.NroLPN, (int)control.IdLpnDet);

                        if (detLpn != null && detLpn.IdCtrlCalidad != EstadoControlCalidad.Controlado)
                        {
                            detLpn.IdCtrlCalidad = EstadoControlCalidad.Controlado;
                            detLpn.NumeroTransaccion = _uow.GetTransactionNumber();

                            toUpdateDetallesLpn.Add(detLpn);
                        }
                    }
                }
                else
                {
                    if (!controlesNuevos.Any(a => a.Ubicacion == control.Stock.Ubicacion && a.Producto == control.Stock.Producto && a.Identificador == control.Stock.Identificador && a.Empresa == control.Stock.Empresa && a.Faixa == control.Stock.Faixa && a.NroLPN == null && !a.Aceptado) && !toApproveFinales.Any(a => a.Ubicacion == control.Stock.Ubicacion && a.Producto == control.Stock.Producto && a.Identificador == control.Stock.Identificador && a.Empresa == control.Stock.Empresa && a.Faixa == control.Stock.Faixa && a.NroLPN == null && !a.Aceptado) && control.Stock.ControlCalidad != EstadoControlCalidad.Controlado)
                    {
                        control.Stock.SetControlado();
                        control.Stock.NumeroTransaccion = _uow.GetTransactionNumber();
                        toUpdateStock.Add(control.Stock);
                    }
                }
            }
        }

        public virtual void AceptarControlesEtiqueta(List<ControlDeCalidadPendiente> controles = null)
        {
            controles ??= this._uow.ControlDeCalidadRepository.GetControles(this._controles);

            foreach (var control in controles)
            {
                control.Aceptar(this._userId);
                this._uow.ControlDeCalidadRepository.UpdateControlPendiente(control);
            }
        }

        public virtual void CargarObjetosBulkAceptarControlesEtiqueta(List<ControlDeCalidadPendiente> controles)
        {
            foreach (var control in controles)
            {
                control.Aceptar(this._userId);
            }
        }
    }
}
