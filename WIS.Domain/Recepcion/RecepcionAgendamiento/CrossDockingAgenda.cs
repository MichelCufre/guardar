using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.Picking;
using WIS.Domain.Recepcion.Enums;

namespace WIS.Domain.Recepcion
{
    public class CrossDockingAgenda
    {
        protected readonly IUnitOfWork _uow;
        protected Agenda _agenda;

        protected AtenderPedidoCrossDockingBulkOperationContext _operationContext;

        public CrossDockingAgenda(IUnitOfWork uow, Agenda agenda)
        {
            this._uow = uow;
            this._agenda = agenda;
            this._operationContext = new AtenderPedidoCrossDockingBulkOperationContext();
        }

        #region AtenderPedidos

        public virtual void AtenderPedidoCrossDocking(IEnumerable<Pedido> pedidos)
        {
            var detallesPedidos = _uow.PedidoRepository.GetDetallePedidos(pedidos);

            foreach (var pedido in pedidos)
            {
                var detallesLoteEspecifico = _agenda.Detalles.Where(d => d.Identificador != ManejoIdentificadorDb.IdentificadorAuto);

                foreach (var detalleAgenda in detallesLoteEspecifico)
                {
                    decimal cantCrossDock = detalleAgenda.CantidadCrossDocking;
                    decimal cantDisponible = 0;

                    detalleAgenda.NumeroTransaccion = _uow.GetTransactionNumber();

                    var detallesPedido = detallesPedidos
                        .Where(w => w.Id == pedido.Id
                            && w.Cliente == pedido.Cliente
                            && w.Empresa == pedido.Empresa
                            && w.Producto == detalleAgenda.CodigoProducto
                            && w.Faixa == detalleAgenda.Faixa
                            && (w.Identificador == detalleAgenda.Identificador || !w.EspecificaIdentificador)
                            && w.HasSaldo())
                        .OrderByDescending(w => w.Identificador);

                    foreach (var detallePedido in detallesPedido)
                    {
                        decimal cantSeparar = (detallePedido.Cantidad ?? 0) - (detallePedido.CantidadLiberada ?? 0) - (detallePedido.CantidadAnulada ?? 0);
                        cantDisponible = detalleAgenda.CantidadAgendada - cantCrossDock;

                        if (cantDisponible > 0)
                        {
                            if (cantDisponible > cantSeparar)
                                cantDisponible = cantSeparar;

                            cantCrossDock = cantCrossDock + cantDisponible;

                            ProcesoCrossDockingTemp(detallePedido, detalleAgenda, cantDisponible);

                            AtenderDetallePedidoCrossDocking(detallesPedidos, detalleAgenda, detallePedido, cantDisponible);
                        }
                    }

                    if (cantCrossDock > 0)
                        ProcesoDetalleAgenda(detalleAgenda, cantCrossDock);
                }
            }

            _uow.CrossDockingRepository.ProcesarPedidosCrossDocking(_operationContext);
        }

        public virtual void ProcesoCrossDockingTemp(DetallePedido detallePedido, AgendaDetalle detalleAgenda, decimal cantDisponible)
        {
            var lineaCrossDockTemp = _operationContext.NewCrossDockingTemp
                .FirstOrDefault(dct => dct.NU_AGENDA == detalleAgenda.IdAgenda
                    && dct.NU_PEDIDO == detallePedido.Id
                    && dct.CD_CLIENTE == detallePedido.Cliente
                    && dct.CD_EMPRESA == detalleAgenda.IdEmpresa
                    && dct.CD_PRODUTO == detalleAgenda.CodigoProducto
                    && dct.CD_FAIXA == detalleAgenda.Faixa
                    && dct.NU_IDENTIFICADOR == detalleAgenda.Identificador
                    && dct.ID_ESPECIFICA_IDENTIFICADOR == detallePedido.EspecificaIdentificador);

            if (lineaCrossDockTemp != null)
            {
                lineaCrossDockTemp.QT_PRODUTO = lineaCrossDockTemp.QT_PRODUTO + cantDisponible;
            }
            else
            {
                var newCrossDockingTemp = new CrossDockingTemp
                {
                    NU_AGENDA = detalleAgenda.IdAgenda,
                    CD_CLIENTE = detallePedido.Cliente,
                    NU_PEDIDO = detallePedido.Id,
                    CD_PRODUTO = detalleAgenda.CodigoProducto,
                    CD_FAIXA = detalleAgenda.Faixa,
                    NU_IDENTIFICADOR = detalleAgenda.Identificador,
                    ID_ESPECIFICA_IDENTIFICADOR = detallePedido.EspecificaIdentificador,
                    CD_EMPRESA = detalleAgenda.IdEmpresa,
                    QT_PRODUTO = cantDisponible
                };

                _operationContext.NewCrossDockingTemp.Add(newCrossDockingTemp);
            }
        }

        public virtual void ProcesoDetalleAgenda(AgendaDetalle detalleAgenda, decimal cantCrossDock)
        {
            detalleAgenda.CantidadCrossDocking = cantCrossDock;
            detalleAgenda.NumeroTransaccion = _uow.GetTransactionNumber();
            detalleAgenda.FechaModificacion = DateTime.Now;

            var detAgendaTemp = _operationContext.UpdateDetalleAgenda
                .FirstOrDefault(s => s.Agenda == detalleAgenda.Agenda
                    && s.CodigoProducto == detalleAgenda.CodigoProducto
                    && s.Identificador == detalleAgenda.Identificador
                    && s.Faixa == detalleAgenda.Faixa
                    && s.IdEmpresa == detalleAgenda.IdEmpresa);

            if (detAgendaTemp != null)
                detAgendaTemp = detalleAgenda;
            else
                _operationContext.UpdateDetalleAgenda.Add(detalleAgenda);
        }

        public virtual void AtenderDetallePedidoCrossDocking(IEnumerable<DetallePedido> detallesPedidos, AgendaDetalle detAgenda, DetallePedido detPedido, decimal cantidad)
        {
            var detalleModificar = detallesPedidos
                .FirstOrDefault(w => w.Id == detPedido.Id
                    && w.Cliente == detPedido.Cliente
                    && w.Empresa == detPedido.Empresa
                    && w.Producto == detAgenda.CodigoProducto
                    && w.Faixa == detAgenda.Faixa
                    && w.Identificador == detAgenda.Identificador);

            if (detPedido.EspecificaIdentificador && detalleModificar != null)
            {
                detalleModificar.CantidadLiberada = (detalleModificar.CantidadLiberada ?? 0) + cantidad;
                detalleModificar.FechaModificacion = DateTime.Now;
                detalleModificar.Transaccion = _uow.GetTransactionNumber();

                ProcesarDetallePedido(detalleModificar);
            }
            else
            {
                if (detalleModificar != null)
                {
                    detalleModificar.CantidadLiberada = (detalleModificar.CantidadLiberada ?? 0) + cantidad;
                    detalleModificar.FechaModificacion = DateTime.Now;
                    detalleModificar.Transaccion = _uow.GetTransactionNumber();

                    ProcesarDetallePedido(detalleModificar);
                }
                else
                {
                    var nuevaLinea = new DetallePedido
                    {
                        Id = detPedido.Id,
                        Cliente = detPedido.Cliente,
                        Empresa = detPedido.Empresa,
                        Producto = detAgenda.CodigoProducto,
                        Faixa = detAgenda.Faixa,
                        Identificador = detAgenda.Identificador,
                        Cantidad = cantidad,
                        CantidadAnulada = 0,
                        CantidadLiberada = cantidad,
                        Agrupacion = detPedido.Agrupacion,
                        EspecificaIdentificador = detPedido.EspecificaIdentificador,
                        FechaAlta = DateTime.Now,
                        Transaccion = _uow.GetTransactionNumber()
                    };

                    ProcesarDetallePedido(nuevaLinea, isUpdate: false);

                    var detalleAuto = detallesPedidos
                        .FirstOrDefault(w => w.Id == detPedido.Id
                            && w.Cliente == detPedido.Cliente
                            && w.Empresa == detPedido.Empresa
                            && w.Producto == detAgenda.CodigoProducto
                            && w.Faixa == detAgenda.Faixa
                            && w.Identificador == ManejoIdentificadorDb.IdentificadorAuto);

                    if (detalleAuto != null)
                    {
                        detalleAuto.Cantidad = (detalleAuto.Cantidad ?? 0) - cantidad;
                        detalleAuto.FechaModificacion = DateTime.Now;
                        detalleAuto.Transaccion = _uow.GetTransactionNumber();

                        ProcesarDetallePedido(detalleAuto);
                    }
                }
            }
        }

        public virtual void ProcesarDetallePedido(DetallePedido detallePedido, bool isUpdate = true)
        {
            DetallePedido detalleEnMemoria = null;

            if (isUpdate)
            {
                detalleEnMemoria = this._operationContext.UpdateDetallePedido
                    .FirstOrDefault(s => s.Id == detallePedido.Id
                        && s.Cliente == detallePedido.Cliente
                        && s.Empresa == detallePedido.Empresa
                        && s.Producto == detallePedido.Producto
                        && s.Faixa == detallePedido.Faixa
                        && s.Identificador == detallePedido.Identificador);
            }
            else
            {
                detalleEnMemoria = this._operationContext.NewDetallePedido
                    .FirstOrDefault(s => s.Id == detallePedido.Id
                        && s.Cliente == detallePedido.Cliente
                        && s.Empresa == detallePedido.Empresa
                        && s.Producto == detallePedido.Producto
                        && s.Faixa == detallePedido.Faixa
                        && s.Identificador == detallePedido.Identificador);
            }


            if (detalleEnMemoria != null)
            {
                detalleEnMemoria = detallePedido;
            }
            else if (isUpdate)
                _operationContext.UpdateDetallePedido.Add(detallePedido);
            else
                _operationContext.NewDetallePedido.Add(detallePedido);
        }

        #endregion

        #region DesatenderPedidos

        public virtual void DesatenderPedidosCrossDocking(bool tomarEnCuentaLineas)
        {
            var empresaDocumental = new Dictionary<int, bool>();
            ICrossDocking crossDock = _uow.CrossDockingRepository.GetCrossDockingActivoByAgenda(_agenda.Id);

            if (crossDock != null)
            {
                List<Pedido> pedidosAtendidos = _uow.PedidoRepository.GetPedidosConPendienteCrossDocking(crossDock.Agenda, crossDock.Preparacion);

                foreach (var pedido in pedidosAtendidos)
                {
                    if (!empresaDocumental.ContainsKey(pedido.Empresa))
                    {
                        var manejaDocumental = _uow.ParametroRepository.GetParameter(ParamManager.MANEJO_DOCUMENTAL, new Dictionary<string, string> { [ParamManager.PARAM_EMPR] = $"{ParamManager.PARAM_EMPR}_{pedido.Empresa}" }) == "S";
                        empresaDocumental[pedido.Empresa] = manejaDocumental;
                    }

                    this.DesatenderPedidoCrossDocking(crossDock, pedido, tomarEnCuentaLineas, empresaDocumental[pedido.Empresa]);
                }


                if (_agenda.Estado == EstadoAgenda.Cerrada || _agenda.Estado == EstadoAgenda.Cancelada)
                {
                    var preparacion = _uow.PreparacionRepository.GetPreparacionPorNumero(crossDock.Preparacion);

                    if (preparacion != null)
                    {
                        preparacion.Situacion = SituacionDb.PreparacionFinalizada;
                        preparacion.Transaccion = _uow.GetTransactionNumber();
                        preparacion.FechaFin = DateTime.Now;

                        _uow.PreparacionRepository.UpdatePreparacion(preparacion);
                    }

                    crossDock.Estado = EstadoCrossDockingDb.Finalizado;
                    _uow.CrossDockingRepository.UpdateCrossDocking(crossDock);
                }
            }
        }

        public virtual void DesatenderPedidoCrossDocking(ICrossDocking crossDock, Pedido pedido, bool tomarEnCuentaLineas, bool procesarDocumental)
        {
            var nuTransaccion = _uow.GetTransactionNumber();

            IDocumentoIngreso docIngreso = null;
            var lineasModificadas = new List<DocumentoLinea>();
            var reservasModificadas = new List<DocumentoPreparacionReserva>();
            var reservasEliminadas = new List<DocumentoPreparacionReserva>();

            if (procesarDocumental)
                docIngreso = _uow.DocumentoRepository.GetIngresoPorAgenda(_agenda.Id);

            foreach (var detPed in pedido.Lineas)
            {
                CrossDockingTemp fila = _uow.CrossDockingRepository.GetCrossDockTemp(pedido.Id, pedido.Empresa, pedido.Cliente, detPed.Identificador, detPed.Producto, _agenda.Id, detPed.Faixa, detPed.EspecificaIdentificador);
                LineaCrossDocking linea = (tomarEnCuentaLineas) ? _uow.CrossDockingRepository.GetLineaCrossDocking(_agenda.Id, crossDock.Preparacion, pedido.Cliente, detPed.Producto, pedido.Id, detPed.Faixa, detPed.Identificador, pedido.Empresa, crossDock.Preparacion) : null;

                if (fila != null || linea != null)
                {
                    var cantidadRestar = fila?.QT_PRODUTO ?? (linea.Cantidad - linea.CantidadPreparada);

                    detPed.CantidadLiberada = detPed.CantidadLiberada - cantidadRestar;
                    detPed.Transaccion = nuTransaccion;

                    List<AgendaDetalle> detalleage = _agenda.Detalles.Where(w => w.IdEmpresa == pedido.Empresa && w.Faixa == detPed.Faixa && w.CodigoProducto == detPed.Producto).ToList();

                    foreach (var detAge in detalleage)
                    {
                        detAge.NumeroTransaccion = nuTransaccion;

                        if (cantidadRestar > detAge.CantidadCrossDocking)
                        {
                            detAge.CantidadCrossDocking = 0;
                        }
                        else
                        {
                            detAge.CantidadCrossDocking = detAge.CantidadCrossDocking - (cantidadRestar);
                        }

                        _uow.AgendaRepository.UpdateAgendaDetalle(detAge);
                    }

                    _uow.PedidoRepository.UpdateDetallePedido(detPed);

                    if (docIngreso != null && cantidadRestar > 0)
                    {
                        var lineaIngreso = lineasModificadas.FirstOrDefault(l => l.Producto == detPed.Producto
                                        && l.Faixa == detPed.Faixa
                                        && l.Identificador == detPed.Identificador);

                        if (lineaIngreso == null)
                        {
                            lineaIngreso = docIngreso.Lineas.FirstOrDefault(d => d.Producto == detPed.Producto
                                            && d.Faixa == detPed.Faixa
                                            && d.Identificador == detPed.Identificador);

                            lineasModificadas.Add(lineaIngreso);
                        }

                        if (lineaIngreso != null)
                        {
                            lineaIngreso.CantidadReservada = (lineaIngreso.CantidadReservada ?? 0) - cantidadRestar;
                            lineaIngreso.FechaModificacion = DateTime.Now;
                        }

                        var docPrepReserva = reservasModificadas.FirstOrDefault(l => l.NroDocumento == docIngreso.Numero
                        && l.TipoDocumento == docIngreso.Tipo
                        && l.Preparacion == crossDock.Preparacion
                        && l.Empresa == docIngreso.Empresa
                        && l.Producto == detPed.Producto
                        && l.Faixa == detPed.Faixa
                        && l.NroIdentificadorPicking == fila.NU_IDENTIFICADOR);

                        var reservaEliminada = reservasEliminadas.FirstOrDefault(l => l.NroDocumento == docIngreso.Numero
                        && l.TipoDocumento == docIngreso.Tipo
                        && l.Preparacion == crossDock.Preparacion
                        && l.Empresa == docIngreso.Empresa
                        && l.Producto == detPed.Producto
                        && l.Faixa == detPed.Faixa
                        && l.NroIdentificadorPicking == fila.NU_IDENTIFICADOR);

                        if (reservaEliminada == null)
                        {
                            if (docPrepReserva == null)
                            {
                                docPrepReserva = _uow.DocumentoRepository.GetPreparacionReserva(docIngreso.Numero, docIngreso.Tipo,
                                                   crossDock.Preparacion, pedido.Empresa, detPed.Producto, detPed.Faixa, detPed.Identificador);

                                if (docPrepReserva != null)
                                {
                                    reservasModificadas.Add(docPrepReserva);
                                }
                            }

                            if (docPrepReserva != null)
                            {
                                docPrepReserva.CantidadProducto = (docPrepReserva.CantidadProducto ?? 0) - cantidadRestar;
                                docPrepReserva.FechaModificacion = DateTime.Now;

                                if (docPrepReserva.CantidadProducto <= 0)
                                {
                                    reservasModificadas.Remove(docPrepReserva);
                                    reservasEliminadas.Add(docPrepReserva);
                                }
                            }
                        }
                    }

                    if (fila != null)
                        _uow.CrossDockingRepository.EliminarTemporal(fila);
                }
            }

            foreach (var linea in lineasModificadas)
            {
                _uow.DocumentoRepository.UpdateLineaDocumento(linea, docIngreso, nuTransaccion);
            }

            foreach (var reserva in reservasModificadas)
            {
                reserva.NumeroTransaccion = nuTransaccion;
                reserva.NumeroTransaccionDelete = null;
                _uow.DocumentoRepository.UpdateDocumentoPreparacionReserva(reserva);
            }

            foreach (var reserva in reservasEliminadas)
            {
                reserva.NumeroTransaccion = nuTransaccion;
                reserva.NumeroTransaccionDelete = nuTransaccion;
                _uow.DocumentoRepository.UpdateDocumentoPreparacionReserva(reserva);
                _uow.SaveChanges();
                _uow.DocumentoRepository.RemoveDocumentoPreparacionReserva(reserva);
            }

            pedido.DesbloquearLiberacion();
            pedido.Transaccion = nuTransaccion;
            pedido.FechaModificacion = DateTime.Now;
            _uow.PedidoRepository.UpdatePedido(pedido);

            _uow.SaveChanges();
        }

        #endregion

    }
}
