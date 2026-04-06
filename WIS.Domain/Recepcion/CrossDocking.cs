using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Documento;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.Picking;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent.Execution.Configuration;

namespace WIS.Domain.Recepcion
{
    public class CrossDocking : ICrossDocking
    {
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        public int Agenda { get; set; }
        public int Preparacion { get; set; }
        public int Usuario { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string Estado { get; set; }
        public string Tipo { get; set; }
        public string IdTipo { get; set; }

        public List<LineaCrossDocking> Lineas { get; set; }

        public IniciarCrossDockingBulkOperationContext _operationContext;

        public CrossDocking()
        {
            Lineas = new List<LineaCrossDocking>();
            _operationContext = new IniciarCrossDockingBulkOperationContext();
        }

        public virtual bool CanEdit()
        {
            return !this.Lineas.Any();
        }

        public static bool PuedeCrearCrossDock(IUnitOfWork uow, Agenda agenda)
        {
            return !uow.CrossDockingRepository.AnyAgendaEnCrossDockActivo(agenda.Id)
                && uow.CrossDockingRepository.HayTiposCrossDockDisponibles(agenda.Estado)
                && uow.AgendaRepository.AnyDetalleLoteNoAuto(agenda.Id);
        }

        public virtual bool PuedeFinalizarCrossDock()
        {
            return true;
        }

        #region Agregar y Quitar Pedidos

        public virtual IEnumerable<Pedido> GetPedidosToAdd(IUnitOfWork uow, Agenda agenda, CrossDockingSeleccionTipo tipoSeleccion, GridMenuItemActionContext context, List<string> gridKeys, IFilterInterpreter filterInterpreter)
        {
            var selection = context.Selection.GetSelection(gridKeys, keys =>
            {
                return new Pedido
                {
                    Id = keys["NU_PEDIDO"],
                    Cliente = keys["CD_CLIENTE"],
                    Empresa = int.Parse(keys["CD_EMPRESA"])
                };
            });

            if (context.Selection.AllSelected)
            {
                var dbQuery = new PedidosCrossDockQuery(agenda, tipoSeleccion);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(filterInterpreter, context.Filters);

                return dbQuery.GetPedidos().Except(selection);
            }

            return selection;
        }

        public virtual void AddPedidos(IUnitOfWork uow, IEnumerable<Pedido> pedidos)
        {
            var nuOrdenLiberacion = uow.CrossDockingRepository.GetLastNumeroOrdenLiberacion(this.Preparacion);

            var keys = pedidos.Select(k => new Pedido
            {
                Id = k.Id,
                Empresa = k.Empresa,
                Cliente = k.Cliente,
                PreparacionProgramada = Preparacion,
                NumeroOrdenLiberacion = nuOrdenLiberacion,
                FechaModificacion = DateTime.Now,
                Transaccion = uow.GetTransactionNumber(),
            }).ToList();

            uow.CrossDockingRepository.MarcarPedidos(keys);

            if (!uow.CrossDockingRepository.ValidarPedidos(Preparacion, out DetallePedido detalleInvalido))
                throw new ValidationFailedException("REC200_frm1_error_PedidoConDetallesConProductoConDosCriterios", [detalleInvalido.Id, detalleInvalido.Empresa.ToString(), detalleInvalido.Cliente, detalleInvalido.Producto]);

            uow.SaveChanges();
        }

        public virtual void RemovePedidos(IUnitOfWork uow, List<Pedido> pedidos)
        {
            var keys = pedidos.Select(k => new Pedido
            {
                Id = k.Id,
                Empresa = k.Empresa,
                Cliente = k.Cliente,
                PreparacionProgramada = null,
                NumeroOrdenLiberacion = null,
                FechaModificacion = DateTime.Now,
                Transaccion = uow.GetTransactionNumber(),
            }).ToList();

            uow.CrossDockingRepository.DesmarcarPedidos(keys);

            uow.SaveChanges();
        }

        public virtual void AddPreparacion(IUnitOfWork uow, int empresa, string predio)
        {
            var onda = uow.CrossDockingRepository.GetOndaCrossDocking();

            var preparacion = new Preparacion
            {
                Usuario = this.Usuario,
                Predio = predio,
                Descripcion = $"Preparacion creada para crossdocking de agenda {this.Agenda}",
                Situacion = SituacionDb.PreparacionIniciada,
                FechaInicio = DateTime.Now,
                Empresa = empresa,
                Tipo = TipoPreparacionDb.CrossDocking,
                Transaccion = uow.GetTransactionNumber(),
                Onda = onda?.Id,
                Agrupacion = Agrupacion.Pedido,
                AceptaMercaderiaAveriada = false,
                PermitePickVencido = false,
                ValidarProductoProveedor = false,
            };

            uow.PreparacionRepository.AddPreparacion(preparacion);

            this.Preparacion = preparacion.Id;
        }

        public virtual void RemovePreparacion(IUnitOfWork uow)
        {
            uow.CrossDockingRepository.EliminarPreparacionCrossDocking(Preparacion, Agenda);
        }

        #endregion

        #region Iniciar CrossDocking

        public virtual void Iniciar(IUnitOfWork uow, Agenda agenda, bool consumirOtrosDocumentos)
        {
            this.Estado = EstadoCrossDockingDb.Iniciado;
            uow.CrossDockingRepository.UpdateCrossDocking(this);

            var pedidos = uow.PedidoRepository.GetPedidosPreparacionProgramada(this.Preparacion);

            ProcessLineasAgenda(uow, agenda, pedidos, consumirOtrosDocumentos);

            var pedidosQuitar = pedidos.Where(w => !this.Lineas.Any(a => a.Pedido == w.Id && a.Cliente == w.Cliente && a.Empresa == w.Empresa)).ToList();

            RemovePedidos(uow, pedidosQuitar);

            uow.CrossDockingRepository.ProcesarInicioCrossDockingDF(_operationContext);
        }

        protected virtual void ProcessLineasAgenda(IUnitOfWork uow, Agenda agenda, List<Pedido> pedidos, bool consumirOtrosDocumentos)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var cargas = new List<Carga>();
            var detallesDisponibles = agenda.Detalles.Where(d => d.CantidadRecibida > 0).ToList();

            logger.Debug($"REC200 - Agenda : {agenda.Id} - Cantidad lineas: {detallesDisponibles?.Count()} ");

            var crossDockingDocumental = new CrossDockingDocumental();

            if (uow.EmpresaRepository.IsEmpresaDocumental(agenda.IdEmpresa))
            {
                crossDockingDocumental.ManejaDocumental = true;
                crossDockingDocumental.DocumentoOriginal = uow.DocumentoRepository.GetIngresoPorAgenda(agenda.Id);
                crossDockingDocumental.DocumentosDisponibles = uow.DocumentoRepository.GetDocumentosDisponibles(agenda.Id, consumirOtrosDocumentos);
                crossDockingDocumental.ConsumirOtrosDocumentos = consumirOtrosDocumentos;
            }

            foreach (var detalleAgenda in detallesDisponibles)
            {
                decimal cantidadCrossDocking = 0;
                var detDisponibleCrossDock = uow.CrossDockingRepository.GetDetalleDisponible(detalleAgenda.IdAgenda, detalleAgenda.IdEmpresa, detalleAgenda.CodigoProducto, detalleAgenda.Faixa, detalleAgenda.Identificador);

                detalleAgenda.NumeroTransaccion = nuTransaccion;

                if (detDisponibleCrossDock == null)
                    continue;

                var pedidosConProductoLinea = pedidos.Where(w => w.Lineas.Count() > 0 && w.Lineas.Any(a => a.Producto == detalleAgenda.CodigoProducto && a.Empresa == detalleAgenda.IdEmpresa)).ToList();

                ProcessPedidos(uow, pedidosConProductoLinea, cargas, detalleAgenda, detDisponibleCrossDock, ref cantidadCrossDocking, ref crossDockingDocumental);

                if (cantidadCrossDocking > 0)
                {
                    detalleAgenda.CantidadCrossDocking += cantidadCrossDocking;
                    detalleAgenda.FechaModificacion = DateTime.Now;
                }

                _operationContext.UpdateDetalleAgenda.Add(detalleAgenda);
            }

            _operationContext.UpdateDocumentoLineaDesafectada.AddRange(crossDockingDocumental.LineasAModificar);

            _operationContext.NewDocumentoPreparacionReserva.AddRange(crossDockingDocumental.NuevasReservas);
        }

        protected virtual void ProcessPedidos(IUnitOfWork uow, List<Pedido> pedidos, List<Carga> cargas, AgendaDetalle lineaAgenda, DetalleDisponibleCrossDocking detDisponibleCrossDock, ref decimal cantidadCrossDocking, ref CrossDockingDocumental crossDockingDocumental)
        {
            logger.Debug($"REC200 - Cantidad pedidos: {pedidos?.Count()} ");

            foreach (var pedido in pedidos.OrderBy(d => d.NumeroOrdenLiberacion))
            {
                var cantidadDisponible = detDisponibleCrossDock.CantidadDisponible ?? 0 - cantidadCrossDocking;

                if (cantidadDisponible > 0)
                    ProcessLineasPedido(uow, pedido, cargas, lineaAgenda, detDisponibleCrossDock, ref cantidadCrossDocking, ref crossDockingDocumental);
                else
                {
                    logger.Debug("REC200 - Recorriendo Pedidos - No mas cantidad disponible");
                    break;
                }
            }
        }

        protected virtual void ProcessLineasPedido(IUnitOfWork uow, Pedido pedido, List<Carga> cargas, AgendaDetalle detalleAgenda, DetalleDisponibleCrossDocking detDisponibleCrossDock, ref decimal cantidadCrossDocking, ref CrossDockingDocumental crossDockingDoc)
        {
            foreach (var detallePedido in pedido.Lineas.OrderByDescending(d => d.EspecificaIdentificador))
            {
                var cantidadDisponible = (detDisponibleCrossDock.CantidadDisponible ?? 0) - cantidadCrossDocking;

                if (cantidadDisponible > 0)
                {
                    decimal saldo = detallePedido.GetSaldo();

                    if (detallePedido.Producto == detalleAgenda.CodigoProducto && detallePedido.Faixa == detalleAgenda.Faixa && pedido.Empresa == detalleAgenda.IdEmpresa && (detallePedido.Identificador == detalleAgenda.Identificador || !detallePedido.EspecificaIdentificador) && saldo > 0)
                    {
                        if (cantidadDisponible > saldo)
                            cantidadDisponible = saldo;

                        cantidadDisponible = DisponibilidadDocumental(crossDockingDoc, detallePedido, cantidadDisponible);
                        cantidadCrossDocking += cantidadDisponible;

                        logger.Debug($"REC200 - LineaCrossDocking - Empresa: {pedido.Empresa} // Pedido : {pedido.Id} //  Cliente : {pedido.Cliente} //    Producto : {detallePedido.Producto} //  Lote: {detallePedido.Identificador} // EspLote: {detallePedido.EspecificaIdentificador}  // Cantidad: {cantidadDisponible}");

                        var lineaCrossDocking = this.Lineas
                            .FirstOrDefault(d => d.Pedido == pedido.Id
                                && d.Cliente == pedido.Cliente
                                && d.Empresa == pedido.Empresa
                                && d.Producto == detallePedido.Producto
                                && d.Faixa == detallePedido.Faixa
                                && d.Identificador == detallePedido.Identificador
                                && d.EspecificaIdentificador == detallePedido.EspecificaIdentificador);

                        if (lineaCrossDocking == null)
                            lineaCrossDocking = this.AddLinea(uow, cargas, pedido, detallePedido, cantidadDisponible, detalleAgenda.NumeroTransaccion);
                        else
                        {
                            uow.SaveChanges();
                            logger.Debug("REC200 - LineaCrossDocking - Update");

                            lineaCrossDocking.Cantidad += cantidadDisponible;
                            lineaCrossDocking.NroTransaccion = detalleAgenda.NumeroTransaccion;

                            ProcesarDetalleCrossDocking(lineaCrossDocking);
                        }

                        detallePedido.CantidadLiberada += cantidadDisponible;
                        detallePedido.Transaccion = uow.GetTransactionNumber();
                        detallePedido.FechaModificacion = DateTime.Now;

                        ProcesarDetallePedido(detallePedido);

                        if (crossDockingDoc.ManejaDocumental)
                            IniciarCrossDockingDocumental(uow, detallePedido, cantidadDisponible, ref crossDockingDoc);
                    }
                }
                else
                {
                    logger.Debug("REC200 - Recorriendo Detalle - No mas cantidad disponible");
                    break;
                }
            }
        }

        protected virtual LineaCrossDocking AddLinea(IUnitOfWork uow, List<Carga> cargas, Pedido pedido, DetallePedido detalle, decimal cantidadDisponible, long? nuTransaccion)
        {
            long cargaId;
            if (pedido.NuCarga != null)
                cargaId = (long)pedido.NuCarga;
            else
            {
                var carga = cargas.FirstOrDefault(d => d.Ruta == pedido.Ruta);

                if (carga == null)
                    carga = this.AddCarga(uow, cargas, (short)pedido.Ruta);

                cargaId = carga.Id;
            }

            var lineaCrossDocking = new LineaCrossDocking
            {
                Agenda = this.Agenda,
                Preparacion = this.Preparacion,
                Pedido = pedido.Id,
                Cliente = pedido.Cliente,
                Empresa = pedido.Empresa,
                Cantidad = cantidadDisponible,
                Carga = cargaId,
                Producto = detalle.Producto,
                Identificador = detalle.Identificador,
                EspecificaIdentificador = detalle.EspecificaIdentificador,
                Faixa = detalle.Faixa,
                CantidadPreparada = 0,
                PreparacionPickeada = this.Preparacion,
                NroTransaccion = nuTransaccion,
                FechaAlta = DateTime.Now,
            };

            this.Lineas.Add(lineaCrossDocking);

            _operationContext.NewDetalleCrossDocking.Add(lineaCrossDocking);

            return lineaCrossDocking;
        }

        protected virtual Carga AddCarga(IUnitOfWork uow, List<Carga> cargas, short ruta)
        {
            var carga = new Carga
            {
                Descripcion = "Carga creada para cross-docking",
                Preparacion = this.Preparacion,
                Ruta = (short)ruta,
                FechaAlta = DateTime.Now
            };

            uow.CargaRepository.AddCarga(carga);

            cargas.Add(carga);

            return carga;
        }

        public virtual void ProcesarDetalleCrossDocking(LineaCrossDocking detalle)
        {
            var detalleEnMemoria = _operationContext.NewDetalleCrossDocking
                .FirstOrDefault(dct => dct.Agenda == detalle.Agenda
                    && dct.Pedido == detalle.Pedido
                    && dct.Cliente == detalle.Cliente
                    && dct.Empresa == detalle.Empresa
                    && dct.Producto == detalle.Producto
                    && dct.Faixa == detalle.Faixa
                    && dct.Identificador == detalle.Identificador
                    && dct.EspecificaIdentificador == detalle.EspecificaIdentificador);

            if (detalleEnMemoria != null)
                detalleEnMemoria = detalle;
        }

        public virtual void ProcesarDetallePedido(DetallePedido detalle)
        {
            var detalleEnMemoria = _operationContext.UpdateDetallePedido
                .FirstOrDefault(s => s.Id == detalle.Id
                    && s.Producto == detalle.Producto
                    && s.Identificador == detalle.Identificador
                    && s.Faixa == detalle.Faixa
                    && s.Empresa == detalle.Empresa);

            if (detalleEnMemoria != null)
                detalleEnMemoria = detalle;
            else
                _operationContext.UpdateDetallePedido.Add(detalle);

        }

        public virtual void Liberar(IUnitOfWork uow, Agenda agenda, List<Carga> cargas)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Finalizar CrossDocking


        public virtual void FinalizarCrossDocking(IUnitOfWork uow, int numeroAgenda, int empresa)
        {
            bool manejaDocumental = uow.EmpresaRepository.IsEmpresaDocumental(empresa);
            ICrossDocking crossDock = uow.CrossDockingRepository.GetCrossDockingActivoByAgenda(numeroAgenda);

            var lineasCrossDocking = uow.CrossDockingRepository.GetLineasCrossDockingParaFinalizar(numeroAgenda, out int nuPreparacion);
            var lineasAFinalizar = new List<LineaReservaCrossDocking>();

            foreach (var linea in lineasCrossDocking.Where(w => w.Cantidad > w.CantidadPreparada))
            {
                var qtDiferencia = linea.Cantidad - linea.CantidadPreparada;

                string espLote = linea.EspecificaIdentificador ? "S" : "N";
                DetallePedido detallePedido = uow.PedidoRepository.GetDetallePedido(linea.Pedido, linea.Empresa, linea.Cliente, linea.Producto, linea.Identificador, linea.Faixa, espLote);

                detallePedido.CantidadLiberada -= qtDiferencia;

                if (detallePedido.CantidadLiberada < 0)
                    detallePedido.CantidadLiberada = 0;

                linea.Cantidad -= qtDiferencia;

                if (linea.Cantidad < 0)
                    linea.Cantidad = 0;

                linea.NroTransaccion = uow.GetTransactionNumber();

                uow.PedidoRepository.UpdateDetallePedido(detallePedido);
                uow.CrossDockingRepository.UpdateDetalleCrossDocking(linea);

                if (manejaDocumental)
                {
                    string espIdentificador = linea.EspecificaIdentificador ? "S" : "N";

                    var det = lineasAFinalizar.FirstOrDefault(l => l.Producto == linea.Producto
                    && l.Faixa == linea.Faixa
                    && l.Identificidaor == linea.Identificador
                    && l.EspecificaIdentificidaor == espIdentificador);

                    if (det != null)
                        det.Cantidad += qtDiferencia;
                    else
                    {
                        lineasAFinalizar.Add(new LineaReservaCrossDocking
                        {
                            Producto = linea.Producto,
                            Faixa = linea.Faixa,
                            Empresa = linea.Empresa,
                            Identificidaor = linea.Identificador,
                            EspecificaIdentificidaor = espIdentificador,
                            Cantidad = qtDiferencia
                        });
                    }
                }
            }

            var pedidos = uow.PedidoRepository.GetPedidosPreparacionProgramada(nuPreparacion);
            foreach (var pedidoOrigin in pedidos)
            {
                pedidoOrigin.DesbloquearLiberacion();
                pedidoOrigin.Transaccion = uow.GetTransactionNumber();
                uow.PedidoRepository.UpdatePedido(pedidoOrigin);
            }

            crossDock.Estado = EstadoCrossDockingDb.Finalizado;

            uow.CrossDockingRepository.UpdateCrossDocking(crossDock);

            if (manejaDocumental)
                FinalizarCrossDockingDocumental(uow, empresa, nuPreparacion, lineasAFinalizar);

            var preparacion = uow.PreparacionRepository.GetPreparacionPorNumero(nuPreparacion);

            if (preparacion != null)
            {
                preparacion.Situacion = SituacionDb.PreparacionFinalizada;
                preparacion.Transaccion = uow.GetTransactionNumber();
                preparacion.FechaFin = DateTime.Now;

                uow.PreparacionRepository.UpdatePreparacion(preparacion);
            }

            uow.SaveChanges();
        }

        #endregion

        #region Documental
        public virtual void IniciarCrossDockingDocumental(IUnitOfWork uow, DetallePedido detalle, decimal cantidadDisponible, ref CrossDockingDocumental crossDockingDoc)
        {
            var saldoAReservar = cantidadDisponible;
            var nuDocOriginal = crossDockingDoc.DocumentoOriginal.Numero;
            var tpDocOriginal = crossDockingDoc.DocumentoOriginal.Tipo;

            if (detalle.EspecificaIdentificador)
                ProcesarLoteEspecificoDocumental(detalle, crossDockingDoc, saldoAReservar, nuDocOriginal, tpDocOriginal);
            else
                ProcesarLoteAutoDocumental(detalle, crossDockingDoc, saldoAReservar, nuDocOriginal, tpDocOriginal);
        }

        public virtual void ProcesarLoteEspecificoDocumental(DetallePedido detalle, CrossDockingDocumental crossDockingDoc, decimal saldoAReservar, string nuDocOriginal, string tpDocOriginal)
        {
            var lineaOriginal = crossDockingDoc.DocumentosDisponibles[detalle.Producto][detalle.Identificador][$"{nuDocOriginal}@{tpDocOriginal}"];

            if (lineaOriginal != null && lineaOriginal.LineaModificada.GetCantidadDisponible() > 0)
                saldoAReservar = ProcesoProductoLote(detalle, crossDockingDoc, lineaOriginal, detalle.Identificador, saldoAReservar);

            if (saldoAReservar > 0 && crossDockingDoc.ConsumirOtrosDocumentos)
            {
                var lineasDisponibles = crossDockingDoc.DocumentosDisponibles[detalle.Producto][detalle.Identificador].Values.Where(d => d.LineaModificada.GetCantidadDisponible() > 0);
                foreach (var linea in lineasDisponibles)
                {
                    saldoAReservar = ProcesoProductoLote(detalle, crossDockingDoc, linea, detalle.Identificador, saldoAReservar);
                }
            }
        }
        
        public virtual void ProcesarLoteAutoDocumental(DetallePedido detalle, CrossDockingDocumental crossDockingDoc, decimal saldoAReservar, string nuDocOriginal, string tpDocOriginal)
        {
            var lotesDocOriginal = crossDockingDoc.DocumentoOriginal.Lineas.Where(d => d.Producto == detalle.Producto && d.GetCantidadDisponible() > 0).Select(d => d.Identificador);

            foreach (var lote in lotesDocOriginal)
            {
                if (saldoAReservar <= 0)
                    break;

                var lineaOriginal = crossDockingDoc.DocumentosDisponibles[detalle.Producto][lote][$"{nuDocOriginal}@{tpDocOriginal}"];

                if (lineaOriginal != null && lineaOriginal.LineaModificada.GetCantidadDisponible() > 0)
                    saldoAReservar = ProcesoProductoLote(detalle, crossDockingDoc, lineaOriginal, lineaOriginal.LineaModificada.Identificador, saldoAReservar);
            }

            if (saldoAReservar > 0 && crossDockingDoc.ConsumirOtrosDocumentos)
            {
                var lineasDisponibles = crossDockingDoc.DocumentosDisponibles[detalle.Producto].Values
                .Select(d => d.Values).SelectMany(x => x)
                .Where(d => d.LineaModificada.GetCantidadDisponible() > 0).ToList();

                foreach (var linea in lineasDisponibles)
                {
                    saldoAReservar = ProcesoProductoLote(detalle, crossDockingDoc, linea, linea.LineaModificada.Identificador, saldoAReservar);
                }
            }
        }
        
        public virtual decimal ProcesoProductoLote(DetallePedido detalle, CrossDockingDocumental crossDockingDoc, DocumentoLineaDesafectada linea, string loteFinal, decimal saldoAReservar)
        {
            decimal cantAReservar = 0;
            var saldolinea = linea.LineaModificada.GetCantidadDisponible();

            if (saldoAReservar > saldolinea)
                cantAReservar = saldolinea;
            else
                cantAReservar = saldoAReservar;

            linea.LineaModificada.CantidadReservada = (linea.LineaModificada.CantidadReservada ?? 0) + cantAReservar;

            var lineaAModificar = GetLineaAModificar(detalle, crossDockingDoc, linea, loteFinal);
            if (lineaAModificar != null)
                lineaAModificar.LineaModificada.CantidadReservada = (lineaAModificar.LineaModificada.CantidadReservada ?? 0) + cantAReservar;
            else
                crossDockingDoc.LineasAModificar.Add(linea);

            var nuevaReserva = GetNuevaPreparacionReserva(detalle, crossDockingDoc, linea, loteFinal);
            if (nuevaReserva != null)
                nuevaReserva.CantidadProducto = (nuevaReserva.CantidadProducto ?? 0) + cantAReservar;
            else
            {
                nuevaReserva = CrearLineaReserva(detalle, linea, cantAReservar, loteFinal);
                crossDockingDoc.NuevasReservas.Add(nuevaReserva);
            }

            saldoAReservar -= cantAReservar;
            crossDockingDoc.DocumentosDisponibles[detalle.Producto][loteFinal][$"{linea.NroDocumento}@{linea.TipoDocumento}"] = linea;
            return saldoAReservar;
        }
        
        public virtual decimal DisponibilidadDocumental(CrossDockingDocumental crossDockingDoc, DetallePedido detalle, decimal cantidadDisponible)
        {
            if (crossDockingDoc.ManejaDocumental)
            {
                decimal cantDocumental = 0;
                if (detalle.EspecificaIdentificador)
                    cantDocumental = crossDockingDoc.DocumentosDisponibles[detalle.Producto][detalle.Identificador].Values.Sum(d => d.LineaModificada.GetCantidadDisponible());
                else
                    cantDocumental = crossDockingDoc.DocumentosDisponibles[detalle.Producto].Values.Sum(l => l.Values.Sum(d => d.LineaModificada.GetCantidadDisponible()));

                cantidadDisponible = Math.Min(cantidadDisponible, cantDocumental);
            }

            return cantidadDisponible;
        }
        
        public virtual DocumentoLineaDesafectada GetLineaAModificar(DetallePedido detalle, CrossDockingDocumental crossDockingDoc, DocumentoLineaDesafectada linea, string loteFinal)
        {
            return crossDockingDoc.LineasAModificar
                .FirstOrDefault(l => l.NroDocumento == linea.NroDocumento
                    && l.TipoDocumento == linea.TipoDocumento
                    && l.LineaModificada.Empresa == detalle.Empresa
                    && l.LineaModificada.Producto == detalle.Producto
                    && l.LineaModificada.Identificador == loteFinal
                    && l.LineaModificada.Faixa == detalle.Faixa);
        }
        
        public virtual DocumentoPreparacionReserva GetNuevaPreparacionReserva(DetallePedido detalle, CrossDockingDocumental crossDockingDoc, DocumentoLineaDesafectada linea, string loteFinal)
        {
            return crossDockingDoc.NuevasReservas
                .FirstOrDefault(l => l.NroDocumento == linea.NroDocumento
                    && l.TipoDocumento == linea.TipoDocumento
                    && l.Preparacion == this.Preparacion
                    && l.Empresa == detalle.Empresa
                    && l.Producto == detalle.Producto
                    && l.Faixa == detalle.Faixa
                    && l.Identificador == loteFinal
                    && l.NroIdentificadorPicking == loteFinal);
        }
        
        public virtual DocumentoPreparacionReserva CrearLineaReserva(DetallePedido detalle, DocumentoLineaDesafectada linea, decimal cantAReservar, string loteFinal)
        {
            return new DocumentoPreparacionReserva
            {
                NroDocumento = linea.NroDocumento,
                TipoDocumento = linea.TipoDocumento,
                Preparacion = this.Preparacion,
                Empresa = detalle.Empresa,
                Producto = detalle.Producto,
                Faixa = detalle.Faixa,
                Identificador = loteFinal,
                CantidadProducto = cantAReservar,
                CantidadPreparada = null,
                CantidadAnular = null,
                NroIdentificadorPicking = loteFinal,
                EspecificaIdentificador = detalle.EspecificaIdentificador
            };
        }

        public virtual void FinalizarCrossDockingDocumental(IUnitOfWork uow, int empresa, int preparacion, List<LineaReservaCrossDocking> lineasAFinalizar)
        {
            var detallesADesreservar = BajarReservaPreparacion(uow, empresa, preparacion, lineasAFinalizar);
            BajaReservaDetalleDocumentos(uow, detallesADesreservar);
        }

        public virtual List<LineaReservaCrossDocking> BajarReservaPreparacion(IUnitOfWork uow, int empresa, int preparacion, List<LineaReservaCrossDocking> lineasAFinalizar)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var detallesDesreservar = new List<LineaReservaCrossDocking>();
            var reservas = uow.DocumentoRepository.GetPreparacionReservas(preparacion, empresa).Where(r => r.CantidadDisponible() > 0).ToList();

            ProcesoReservaLoteEspecifico(lineasAFinalizar, detallesDesreservar, reservas);
            ProcesoReservaLotesAuto(lineasAFinalizar, detallesDesreservar, reservas);

            var reservasModificadas = reservas.Where(d => (detallesDesreservar
                .Any(r => d.NroDocumento == r.NroDocumento
                    && d.TipoDocumento == r.TipoDocumento
                    && d.Empresa == r.Empresa
                    && d.Producto == r.Producto
                    && d.Faixa == r.Faixa
                    && d.Identificador == r.Identificidaor)));

            foreach (var reserva in reservasModificadas)
            {
                reserva.NumeroTransaccion = nuTransaccion;

                if ((reserva.CantidadProducto ?? 0) > 0)
                {
                    reserva.NumeroTransaccionDelete = null;
                    uow.DocumentoRepository.UpdateDocumentoPreparacionReserva(reserva);
                }
                else
                {
                    reserva.NumeroTransaccionDelete = nuTransaccion;
                    uow.DocumentoRepository.UpdateDocumentoPreparacionReserva(reserva);
                    uow.SaveChanges();
                    uow.DocumentoRepository.RemoveDocumentoPreparacionReserva(reserva);
                }
            }

            return detallesDesreservar;
        }

        public virtual void BajaReservaDetalleDocumentos(IUnitOfWork uow, List<LineaReservaCrossDocking> detallesADesreservar)
        {
            var nuTransaccion = uow.GetTransactionNumber();

            foreach (var d in detallesADesreservar)
            {
                var detalle = uow.DocumentoRepository.GetDetalleDocumento(d.Producto, d.Faixa, d.Identificidaor, d.Empresa, d.NroDocumento, d.TipoDocumento);

                if (d.Cantidad >= (detalle.CantidadReservada ?? 0))
                    detalle.CantidadReservada = 0;
                else
                    detalle.CantidadReservada = (detalle.CantidadReservada ?? 0) - d.Cantidad;

                uow.DocumentoRepository.UpdateDetailWithoutDocument(d.NroDocumento, d.TipoDocumento, detalle, nuTransaccion);
            }
        }

        public virtual void ProcesoReservaLoteEspecifico(List<LineaReservaCrossDocking> lineasAFinalizar, List<LineaReservaCrossDocking> detallesADesreservar, List<DocumentoPreparacionReserva> reservas)
        {
            foreach (var detalle in lineasAFinalizar.Where(l => l.Identificidaor != ManejoIdentificadorDb.IdentificadorAuto))
            {
                var saldoADesreservar = detalle.Cantidad;
                foreach (var reserva in reservas.Where(r => r.Producto == detalle.Producto && r.Faixa == detalle.Faixa && r.NroIdentificadorPicking == detalle.Identificidaor))
                {
                    if (saldoADesreservar == 0)
                        break;

                    BajarReservaDocumental(detallesADesreservar, reserva, ref saldoADesreservar);
                }
            }
        }

        public virtual void ProcesoReservaLotesAuto(List<LineaReservaCrossDocking> lineasAFinalizar, List<LineaReservaCrossDocking> detallesADesreservar, List<DocumentoPreparacionReserva> reservas)
        {
            foreach (var detalle in lineasAFinalizar.Where(l => l.Identificidaor == ManejoIdentificadorDb.IdentificadorAuto))
            {
                var saldoADesreservar = detalle.Cantidad;
                foreach (var reserva in reservas.Where(r => r.Producto == detalle.Producto && r.Faixa == detalle.Faixa && r.CantidadDisponible() > 0).OrderByDescending(r => r.CantidadDisponible()))
                {
                    if (saldoADesreservar == 0)
                        break;

                    BajarReservaDocumental(detallesADesreservar, reserva, ref saldoADesreservar);
                }
            }
        }

        public virtual void BajarReservaDocumental(List<LineaReservaCrossDocking> detallesADesreservar, DocumentoPreparacionReserva reserva, ref decimal saldoADesreservar)
        {
            decimal cantidad = 0;
            var saldoLinea = (reserva.CantidadProducto ?? 0) - (reserva.CantidadPreparada ?? 0);

            if (saldoLinea >= saldoADesreservar)
            {
                cantidad = saldoADesreservar;
                reserva.CantidadProducto = (reserva.CantidadProducto ?? 0) - saldoADesreservar;
                saldoADesreservar = 0;
            }
            else
            {
                cantidad = saldoLinea;
                reserva.CantidadProducto = (reserva.CantidadProducto ?? 0) - saldoLinea;
                saldoADesreservar -= saldoLinea;
            }

            var detDoc = detallesADesreservar.FirstOrDefault(l => l.NroDocumento == reserva.NroDocumento
            && l.TipoDocumento == reserva.TipoDocumento
            && l.Empresa == reserva.Empresa
            && l.Producto == reserva.Producto
            && l.Faixa == reserva.Faixa
            && l.Identificidaor == reserva.Identificador);

            if (detDoc != null)
                detDoc.Cantidad += cantidad;
            else
            {
                detallesADesreservar.Add(new LineaReservaCrossDocking
                {
                    NroDocumento = reserva.NroDocumento,
                    TipoDocumento = reserva.TipoDocumento,
                    Empresa = reserva.Empresa,
                    Producto = reserva.Producto,
                    Faixa = reserva.Faixa,
                    Identificidaor = reserva.Identificador,
                    EspecificaIdentificidaor = reserva.EspecificaIdentificador ? "S" : "N",
                    Cantidad = cantidad
                });
            }
        }

        #endregion
    }
}
