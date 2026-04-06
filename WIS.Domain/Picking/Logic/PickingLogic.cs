using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Security;

namespace WIS.Domain.Picking.Logic
{
    public class PickingLogic
    {

        protected IIdentityService _identity { get; set; }

        protected ITrackingService _trackingService;

        public PickingLogic(IIdentityService identity, ITrackingService trackingService)
        {
            _identity = identity;
            _trackingService = trackingService;
        }

        public virtual void PickingAdministrativo(IUnitOfWork uow, long nuTransaccion, string descripcion, List<StockPicking> stocksPicking, List<LpnDetalle> detallesLpns, ref string idPedido, int ruta, int empresa, string cliente, string predio, string tpPedido, string tpExpedicion, ref int? nroPreparacion, bool crearPreparacionCarga, DateTime? fechaEntrega)
        {
            bool crearPedido = false;
            var pedido = uow.PedidoRepository.GetPedido(empresa, cliente, idPedido);

            if (pedido == null)
            {
                CreatePedido(uow, ref idPedido, empresa, cliente, predio, tpPedido, tpExpedicion, fechaEntrega, ruta, out crearPedido, out pedido);
            }

            bool isEmpresaDocumental = uow.EmpresaRepository.IsEmpresaDocumental(empresa);

            uow.PreparacionRepository.PuedePrepararse(isEmpresaDocumental, stocksPicking, detallesLpns,predio);

            GetBulkContextPedido(pedido, nuTransaccion, stocksPicking, detallesLpns, out List<DetallePedido> detallesPedido, out List<DetallePedidoLpn> detallesPedidoLpn);

            GetBulkContextPreparacion(uow, ref nroPreparacion,  descripcion, pedido, nuTransaccion, detallesPedido, stocksPicking, detallesLpns, isEmpresaDocumental, out Preparacion preparacion, out Carga carga, out List<DocumentoPreparacionReserva> documentoReserva, out List<DetallePreparacion> detallePreparacion, out List<DetallePreparacionLpn> detallePreparacionPedido);

            uow.PreparacionRepository.GenerarPedidoAndDetalles(uow, pedido, crearPedido, detallesPedido, detallesPedidoLpn, uow.GetTransactionNumber());

            uow.PreparacionRepository.GenerarPreparacionAndReserva(uow, crearPreparacionCarga, pedido, preparacion, carga, documentoReserva, detallePreparacion, detallePreparacionPedido);

            uow.PreparacionRepository.RemoveTablasTemporalesPickingAdministrativo();

            uow.SaveChanges();

            var agente = uow.AgenteRepository.GetAgente(empresa, pedido.Cliente);
            _trackingService.SincronizarPedido(uow, pedido, agente, false);
            uow.PedidoRepository.UpdatePedido(pedido);

            uow.SaveChanges();

        }

        public virtual void CreatePedido(IUnitOfWork uow, ref string idPedido, int empresa, string cliente, string predio, string tpPedido, string tpExpedicion, DateTime? fechaEntrega, int ruta, out bool crearPedido, out Pedido pedido)
        {
            pedido = new Pedido();
            pedido.Id = string.IsNullOrEmpty(idPedido) ? Convert.ToString(uow.PedidoRepository.GetNextNuPedidoManual()) : idPedido;
            pedido.Tipo = tpPedido;
            pedido.Cliente = cliente;
            pedido.Agrupacion = Agrupacion.Pedido;
            pedido.Empresa = empresa;
            pedido.FechaEntrega = fechaEntrega;
            pedido.Predio = predio;
            pedido.TipoExpedicionId = tpExpedicion;
            pedido.FechaAlta = DateTime.Now;
            pedido.Estado = SituacionDb.PedidoAbierto;
            pedido.ManualId = "N";
            pedido.CondicionLiberacion = CondicionLiberacionDb.SinCondicion;
            pedido.ComparteContenedorPicking = $"{pedido.Id}#{pedido.Empresa}#{pedido.Cliente}";
            pedido.NuCarga = null;
            pedido.Origen = "PRE052";
            pedido.Actividad = EstadoPedidoDb.Activo;
            pedido.Transaccion = uow.GetTransactionNumber();
            pedido.SincronizacionRealizadaId = "N";
            pedido.Ruta = ruta;
            pedido.ConfiguracionExpedicion = uow.PedidoRepository.GetConfiguracionExpedicion(tpExpedicion);
            crearPedido = true;
            idPedido = pedido.Id;
        }

        public virtual void GetBulkContextPedido(Pedido pedido, long nuTransaccion, List<StockPicking> stocksPicking, List<LpnDetalle> detallesLpns, out List<DetallePedido> detallesPedido, out List<DetallePedidoLpn> detallesPedidoLpn)
        {

            detallesPedido = new List<DetallePedido>();
            detallesPedidoLpn = new List<DetallePedidoLpn>();

            foreach (var stock in stocksPicking.GroupBy(x => new { x.Producto, x.Identificador, x.Faixa })
                .Select(x => new LpnDetalle() { CodigoProducto = x.Key.Producto, Lote = x.Key.Identificador, Faixa = x.Key.Faixa, Cantidad = x.Sum(d => d.Cantidad) }))
            {
                detallesPedido.Add(new DetallePedido()
                {
                    Id = pedido.Id,
                    Empresa = pedido.Empresa,
                    Cliente = pedido.Cliente,
                    Faixa = stock.Faixa,
                    Producto = stock.CodigoProducto,
                    FechaAlta = DateTime.Now,
                    FechaModificacion = null,
                    Identificador = stock.Lote,
                    EspecificaIdentificadorId = "S",
                    Cantidad = stock.Cantidad,
                    CantidadLiberada = 0,
                    Agrupacion = Agrupacion.Pedido,
                    CantidadAnulada = 0,
                    Transaccion = nuTransaccion,
                });
            }

            foreach (var detalleLpn in detallesLpns.GroupBy(x => new { x.CodigoProducto, x.Lote, x.Faixa })
                .Select(x => new LpnDetalle() { CodigoProducto = x.Key.CodigoProducto, Lote = x.Key.Lote, Faixa = x.Key.Faixa, Cantidad = x.Sum(d => d.Cantidad) }))
            {
                var detallePedido = detallesPedido.FirstOrDefault(x => x.Producto == detalleLpn.CodigoProducto && x.Identificador == detalleLpn.Lote && x.Faixa == detalleLpn.Faixa);

                if (detallePedido == null)
                {
                    detallesPedido.Add(new DetallePedido()
                    {
                        Id = pedido.Id,
                        Empresa = pedido.Empresa,
                        Cliente = pedido.Cliente,
                        Faixa = detalleLpn.Faixa,
                        Producto = detalleLpn.CodigoProducto,
                        FechaAlta = DateTime.Now,
                        FechaModificacion = null,
                        Identificador = detalleLpn.Lote,
                        EspecificaIdentificadorId = "S",
                        Cantidad = detalleLpn.Cantidad,
                        Agrupacion = Agrupacion.Pedido,
                        CantidadLiberada = 0,
                        CantidadAnulada = 0,
                        Transaccion = nuTransaccion,
                    });
                }
                else
                {
                    detallesPedido.Remove(detallePedido);
                    detallePedido.Cantidad = detallePedido.Cantidad + detalleLpn.Cantidad;
                    detallesPedido.Add(detallePedido);
                }

            }

            foreach (var detalleLpn in detallesLpns
                .GroupBy(x => new { x.CodigoProducto, x.Lote, x.Faixa, x.NumeroLPN, x.Tipo, x.IdExterno })
                .Select(x => new LpnDetalle() { CodigoProducto = x.Key.CodigoProducto, Lote = x.Key.Lote, Faixa = x.Key.Faixa, NumeroLPN = x.Key.NumeroLPN, Cantidad = x.Sum(d => d.Cantidad), Tipo = x.Key.Tipo, IdExterno = x.Key.IdExterno, }))
            {
                detallesPedidoLpn.Add(new DetallePedidoLpn()
                {
                    Pedido = pedido.Id,
                    Empresa = pedido.Empresa,
                    Cliente = pedido.Cliente,
                    Faixa = detalleLpn.Faixa,
                    Producto = detalleLpn.CodigoProducto,
                    FechaAlta = DateTime.Now,
                    FechaModificacion = null,
                    Identificador = detalleLpn.Lote,
                    IdEspecificaIdentificador = "S",
                    IdLpnExterno = detalleLpn.IdExterno,
                    Tipo = detalleLpn.Tipo,
                    CantidadPedida = detalleLpn.Cantidad,
                    CantidadLiberada = 0,
                    CantidadAnulada = 0,
                    Transaccion = nuTransaccion,
                    NumeroLpn = detalleLpn.NumeroLPN
                });
            }
        }

        public virtual void GetBulkContextPreparacion(IUnitOfWork uow, ref int? nroPreparacion, string descripcion, Pedido pedido, long nuTransaccion, List<DetallePedido> detallesPedido, List<StockPicking> stocksPicking, List<LpnDetalle> detallesLpns, bool isEmpresaDocumental, out Preparacion preparacion, out Carga carga, out List<DocumentoPreparacionReserva> documentosReservas, out List<DetallePreparacion> detallesPreparacion, out List<DetallePreparacionLpn> detallePreparacionPedido)
        {
            ProcesarPreparacion(uow, ref nroPreparacion, descripcion, pedido, out preparacion, out carga);

            documentosReservas = new List<DocumentoPreparacionReserva>();
            detallesPreparacion = new List<DetallePreparacion>();
            detallePreparacionPedido = new List<DetallePreparacionLpn>();

            ProcesarDetallesPreparacion(uow, pedido, preparacion, carga, stocksPicking, detallesLpns, detallesPreparacion, detallePreparacionPedido);

            if (isEmpresaDocumental)
            {
                ProcesarReservaDocumental(uow, pedido.Predio, detallesPedido, preparacion.Id, documentosReservas);
            }

        }

        public virtual void ProcesarPreparacion(IUnitOfWork uow, ref int? nroPreparacion, string descripcion, Pedido pedido, out Preparacion preparacion, out Carga carga)
        {
            if (nroPreparacion == null)
            {
                var ondaRuta = uow.RutaRepository.GetRuta(short.Parse((pedido.Ruta ?? 1).ToString()));
                nroPreparacion = uow.PreparacionRepository.GetNextNumeroPreparacion();
                preparacion = new Preparacion()
                {
                    Id = nroPreparacion ?? 0,
                    Descripcion = descripcion,
                    FechaInicio = DateTime.Now,
                    Usuario = _identity.UserId,
                    Tipo = "N",
                    Situacion = SituacionDb.HabilitadoParaPickear,
                    FlAceptaMercaderiaAveriada = "S",
                    Onda = ondaRuta.OndaId,
                    Predio = pedido.Predio,
                    Empresa = pedido.Empresa,
                    CantidadRechazo = 0,
                    Agrupacion = Agrupacion.Pedido,
                    FlPermitePickVencido = "S",
                    CodigoContenedorValidado = "TPOPED",
                };

                var nroCarga = uow.PreparacionRepository.GetNextNumeroCarga();
                carga = new Carga()
                {
                    Id = nroCarga,
                    Descripcion = $"Generada por la preparación Administrativa: {(nroPreparacion ?? 0).ToString()}",
                    Preparacion = nroPreparacion,
                    FechaAlta = DateTime.Now,
                    Ruta = short.Parse((pedido.Ruta ?? 1).ToString())
                };

            }
            else
            {
                preparacion = uow.PreparacionRepository.GetPreparacionPorNumero(nroPreparacion ?? 0);
                carga = uow.CargaRepository.GetCarga(preparacion.Id);
            }

        }

        public virtual void ProcesarDetallesPreparacion(IUnitOfWork uow, Pedido pedido, Preparacion preparacion, Carga carga, List<StockPicking> stocksPicking, List<LpnDetalle> detallesLpns, List<DetallePreparacion> detallesPreparacion, List<DetallePreparacionLpn> detallePreparacionPedido)
        {
            int sec = 0;
            foreach (var stock in stocksPicking)
            {
                detallesPreparacion.Add(new DetallePreparacion()
                {
                    NumeroPreparacion = preparacion.Id,
                    Pedido = pedido.Id,
                    Cliente = pedido.Cliente,
                    Producto = stock.Producto,
                    Empresa = stock.Empresa,
                    Lote = stock.Identificador,
                    Faixa = stock.Faixa,
                    Ubicacion = stock.Ubicacion,
                    Cantidad = stock.Cantidad,
                    CantidadPreparada = null,
                    Agrupacion = Agrupacion.Pedido,
                    Estado = EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE,
                    Transaccion = uow.GetTransactionNumber(),
                    FechaAlta = DateTime.Now,
                    Usuario = _identity.UserId,
                    Carga = carga.Id,
                    EspecificaLote = "S",
                    NumeroSecuencia = sec
                });

                sec++;
            }
            var cantidadDetallesLpns = detallesLpns.Count();
            var idsDetPickinLpn = uow.PreparacionRepository.GetNewIdDetallePickingLpn(detallesLpns.Count());

            foreach (var detalleLpn in detallesLpns)
            {
                var idDetallePickingLpn = idsDetPickinLpn.FirstOrDefault();
                idsDetPickinLpn.Remove(idDetallePickingLpn);

                detallesPreparacion.Add(new DetallePreparacion()
                {
                    NumeroPreparacion = preparacion.Id,
                    Pedido = pedido.Id,
                    Cliente = pedido.Cliente,
                    Producto = detalleLpn.CodigoProducto,
                    Empresa = detalleLpn.Empresa,
                    Lote = detalleLpn.Lote,
                    Faixa = detalleLpn.Faixa,
                    Ubicacion = detalleLpn.Ubicacion,
                    Cantidad = detalleLpn.Cantidad,
                    CantidadPreparada = null,
                    Agrupacion = Agrupacion.Pedido,
                    Estado = EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE,
                    Transaccion = uow.GetTransactionNumber(),
                    FechaAlta = DateTime.Now,
                    Usuario = _identity.UserId,
                    EspecificaLote = "S",
                    Carga = carga.Id,
                    NumeroSecuencia = sec,
                    IdDetallePickingLpn = idDetallePickingLpn,
                });


                detallePreparacionPedido.Add(new DetallePreparacionLpn()
                {
                    NroPreparacion = preparacion.Id,
                    IdDetallePickingLpn = idDetallePickingLpn,
                    IdDetalleLpn = detalleLpn.Id,
                    NroLpn = detalleLpn.NumeroLPN,
                    Empresa = detalleLpn.Empresa,
                    Faixa = detalleLpn.Faixa,
                    Lote = detalleLpn.Lote,
                    FechaAlta = DateTime.Now,
                    Producto = detalleLpn.CodigoProducto,
                    IdExternoLpn = detalleLpn.IdExterno,
                    CantidadReservada = detalleLpn.Cantidad,
                    Ubicacion = detalleLpn.Ubicacion,
                    Transaccion = uow.GetTransactionNumber(),
                    TipoLpn = detalleLpn.Tipo,
                    Pedido = pedido.Id,
                    Cliente = pedido.Cliente,
                });

                sec++;
            }
        }

        public virtual void ProcesarReservaDocumental(IUnitOfWork uow,string predio, List<DetallePedido> detallesPedido, int preparacion, List<DocumentoPreparacionReserva> documentosReservas)
        {
            var detallesDocumental = uow.PreparacionRepository.GetDetallesDocumentoCandidatosByDetallePedido(predio);
            foreach (var detallePedido in detallesPedido)
            {
                var cantidadRestantateReservar = detallePedido.Cantidad ?? 0;
                foreach (var detalleDocumento in detallesDocumental.Where(x => x.Producto == detallePedido.Producto && x.Empresa == detallePedido.Empresa && x.Faixa == detallePedido.Faixa && x.Identificador == detallePedido.Identificador))
                {
                    if (cantidadRestantateReservar <= 0)
                        break;

                    decimal cantidadReservar = 0;
                    if (cantidadRestantateReservar <= detalleDocumento.CantidadDisponible)
                    {
                        cantidadReservar = cantidadRestantateReservar;
                        cantidadRestantateReservar = 0;
                    }
                    else
                    {
                        cantidadReservar = detalleDocumento.CantidadDisponible;
                        cantidadRestantateReservar = cantidadRestantateReservar - cantidadReservar;
                    }

                    documentosReservas.Add(new DocumentoPreparacionReserva()
                    {
                        NroDocumento = detalleDocumento.Documento,
                        TipoDocumento = detalleDocumento.TipoDocumento,
                        Producto = detalleDocumento.Producto,
                        Identificador = detalleDocumento.Identificador,
                        NroIdentificadorPicking = detalleDocumento.Identificador,
                        Faixa = detalleDocumento.Faixa,
                        Empresa = detalleDocumento.Empresa,
                        CantidadProducto = cantidadReservar,
                        CantidadAnular = 0,
                        CantidadPreparada = 0,
                        Preparacion = preparacion,
                        FechaAlta = DateTime.Now,
                        EspecificaIdentificadorId = detallePedido.EspecificaIdentificadorId,
                        Auditoria = string.Format("{0}${1}${2}", _identity.Application, _identity.UserId, uow.GetTransactionNumber())
                    });

                }
            }
        }

    }
}
