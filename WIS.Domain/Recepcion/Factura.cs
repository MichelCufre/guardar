using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Interfaces;
using WIS.TrafficOfficer;

namespace WIS.Domain.Recepcion
{
    public class Factura
    {
        public int Id { get; set; }
        public string Predio { get; set; }
        public string Serie { get; set; }
        public string NumeroFactura { get; set; }
        public int? Agenda { get; set; }
        public string TipoFactura { get; set; }
        public int IdEmpresa { get; set; }
        public DateTime? FechaEmision { get; set; }
        public decimal? TotalDigitado { get; set; }
        public string TipoAgente { get; set; }
        public string CodigoInternoCliente { get; set; }
        public string CodigoAgente { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string CodigoMoneda { get; set; }
        public string Anexo1 { get; set; }
        public string Anexo2 { get; set; }
        public string Anexo3 { get; set; }
        public string Observacion { get; set; }
        public string IdOrigen { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public short? Situacion { get; set; }
        public string Estado { get; set; }
        public string Referencia { get; set; }
        public string Remito { get; set; }
        public long? NumeroTransaccion { get; set; }
        public long? NumeroTransaccionDelete { get; set; }
        public decimal? IvaBase { get; set; }
        public decimal? IvaMinimo { get; set; }

        public List<FacturaDetalle> Detalles { get; set; }

        public Factura()
        {
            this.Detalles = new List<FacturaDetalle>();
        }

        public virtual bool CancelarFactura(IUnitOfWork uow, string aplicacion, int userId, ITrafficOfficerService concurrencyControl, int idFactura)
        {
            TrafficOfficerTransaction transaction = concurrencyControl.CreateTransaccion();

            try
            {
                uow.CreateTransactionNumber("Factura - DesvincularDeAgenda");

                var factura = uow.FacturaRepository.GetFacturaCabezal(idFactura);

                if (factura.Agenda == null && factura.Estado == EstadoFacturaDb.Pendiente)
                {
                    factura.Estado = EstadoFacturaDb.Cancelada;
                    factura.Situacion = SituacionDb.Inactivo;
                    factura.FechaModificacion = DateTime.Now;
                    factura.NumeroTransaccion = uow.GetTransactionNumber();

                    uow.FacturaRepository.AddUpdateIdAgendaFactura(factura, updateDetalles: false);
                    uow.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                concurrencyControl.DeleteTransaccion(transaction);
            }
        }

        public virtual bool DesvincularDeAgenda(IUnitOfWork uow, string aplicacion, int userId, ITrafficOfficerService concurrencyControl, int idFactura)
        {
            TrafficOfficerTransaction transaction = concurrencyControl.CreateTransaccion();
            bool desValid = false;

            try
            {
                uow.CreateTransactionNumber("Factura - DesvincularDeAgenda");

                Factura factura = uow.FacturaRepository.GetFactura(idFactura);

                if (factura.Agenda.HasValue)
                {
                    var agenda = uow.AgendaRepository.GetAgenda(factura.Agenda.Value);

                    if (agenda != null && uow.FacturaRepository.EsPosibleDesvincularFactura(agenda.Id, factura.Id))
                    {
                        factura.Agenda = null;
                        factura.FechaModificacion = DateTime.Now;
                        factura.NumeroTransaccion = uow.GetTransactionNumber();

                        uow.FacturaRepository.UpdateFactura(factura); 
                        uow.SaveChanges();

                        desValid = true;
                    }
                }
            }
            finally
            {
                concurrencyControl.DeleteTransaccion(transaction);
            }

            return desValid;
        }

        public virtual void CrearDetallesFactura()
        {
            this.Detalles = CreacionDetalleStrategy.CrearDetallesFactura();
        }

        public virtual void SetCrearDetalleStrategy(ICrearDetallesFacturaStrategy strategy)
        {
            CreacionDetalleStrategy = strategy;
        }

        protected ICrearDetallesFacturaStrategy CreacionDetalleStrategy { get; set; }

        public virtual void SetCrearDetalleStrategy()
        {
            this.SetCrearDetalleStrategy(new CrearDetalleFacturaStrategy());
        }
    }
}
