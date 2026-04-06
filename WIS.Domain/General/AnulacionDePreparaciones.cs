using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Picking;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Domain.Validation;
using WIS.Exceptions;
using WIS.Security;

namespace WIS.Domain.General
{
    public class AnulacionDePreparaciones
    {
        protected readonly ILogger<AnulacionService> _logger;
        protected readonly IUnitOfWork _uow;
        protected readonly IDapper _dapper;
        protected readonly ITaskQueueService _taskQueue;
        protected readonly IIdentityService _identity;
        protected readonly ITrackingService _trackingService;

        protected bool _manejaInterfaz;

        public AnulacionDePreparaciones(IUnitOfWork uow,
            ILogger<AnulacionService> logger,
            IDapper dapper,
            ITaskQueueService taskQueue,
            IIdentityService identity,
            ITrackingService trackingService)
        {
            _uow = uow;
            _logger = logger;
            _dapper = dapper;
            _taskQueue = taskQueue;
            _identity = identity;
            _trackingService = trackingService;
        }

        public virtual void IniciarProcesoAnulacion()
        {
            try
            {
                var keys = new List<string>();
                var keysProduccion = new List<string>();
                var cargas = new List<long>();

                _logger.LogDebug($"IniciarProcesoAnulacion");

                using (var connection = this._dapper.GetDbConnection())
                {
                    connection.Open();

                    long? nuTransaccion = null;

                    _manejaInterfaz = (_uow.ParametroRepository.GetParametro(ParamManager.ENVIO_INT_ANULACION).Result ?? "N") != "S" ? false : true;

                    if (_uow.EmpresaRepository.AnyEmpresaDocumentalActiva())
                    {
                        _logger.LogDebug($"Actualización de estado de anulaciones de empresas documentales");
                        _uow.AnulacionRepository.ActualizarAnulacionesDocumentales(connection, ref nuTransaccion);

                        _logger.LogDebug($"Anulación de preparaciones de empresas documentales");
                        _uow.AnulacionRepository.AnularPreparacionesDocumentales(connection, ref nuTransaccion);
                    }

                    _logger.LogDebug($"Generación de detalles de anulaciones");
                    _uow.AnulacionRepository.GenerarDetallesDeAnulacion(connection);

                    var anulaciones = _uow.AnulacionRepository.GetAnulacionesPendientes(connection);

                    if (anulaciones.Count > 0)
                    {
                        foreach (var a in anulaciones)
                        {
                            _logger.LogDebug($"Procesar Anulacion Nro: {a.NroAnulacionPreparacion}");

                            var detalles = _uow.AnulacionRepository.GetDetallesPendientes(a.NroAnulacionPreparacion, a.Preparacion, connection);
                            var errores = ProcesarAnulacion(a.NroAnulacionPreparacion, a.Preparacion, detalles, ref nuTransaccion, connection, keys, keysProduccion, cargas);

                            if (errores != null && errores.Count > 0)
                            {
                                _uow.AnulacionRepository.GuardarErrores(a.NroAnulacionPreparacion, errores, connection);
                                _uow.AnulacionRepository.UpdateEstadoAnulacion(a.NroAnulacionPreparacion, EstadoAnulacion.FinalizadaError, connection);
                            }
                            else
                                _uow.AnulacionRepository.UpdateEstadoAnulacion(a.NroAnulacionPreparacion, EstadoAnulacion.FinalizadaOk, connection);
                        }
                    }
                }

                if (_taskQueue.IsEnabled() && keysProduccion.Count > 0)
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.ConfirmacionProduccion, keysProduccion);

                if (_taskQueue.IsEnabled() && keys.Count > 0)
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.PedidosAnulados, keys);

                if (_trackingService.TrackingHabilitado())
                    _trackingService.RegularizarBultosAnulados(_uow, cargas);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public virtual List<string> ProcesarAnulacion(int nroAnulacionPreparacion, int nroPreparacion, List<AnulacionPreparacionDetalle> detalles, ref long? nuTransaccion, DbConnection connection, List<string> keys, List<string> keysProduccion, List<long> cargas)
        {
            var errores = new List<Error>();
            _logger.LogDebug($"Proceso detalles para la Anulación Nro: {nroAnulacionPreparacion} y Preparación: {nroPreparacion}");

            foreach (var det in detalles)
            {
                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        if (det.TipoArmadoEgreso == TipoArmadoEgreso.Empaque && det.FacturaAutoCompletar == "S")
                            throw new ValidationFailedException("General_msg_Error_AnulacionTpArmadoEgresoNoValido", [TipoArmadoEgreso.Empaque, det.NumeroPreparacion.ToString(), det.Empresa.ToString(), det.Cliente]);

                        if (!nuTransaccion.HasValue)
                            nuTransaccion = _uow.TransaccionRepository.CreateTransaction($"Proceso de anulación", connection, tran: tran, app: _identity.Application, userId: _identity.UserId).Result;

                        ProcesarDetalle(connection, tran, det, nuTransaccion.Value, keys, keysProduccion);

                        tran.Commit();

                        if (!cargas.Contains((long)det.Carga))
                            cargas.Add((long)det.Carga);
                    }
                    catch (ValidationFailedException ex)
                    {
                        this._logger.LogError(ex, ex.Message);

                        errores.Add(new Error(ex));

                        tran.Rollback();
                    }
                }
            }

            return Translator.Translate(_uow, errores, _identity.UserId);
        }

        public virtual void ProcesarDetalle(DbConnection connection, DbTransaction tran, AnulacionPreparacionDetalle detalleAnulacion, long nuTransaccion, List<string> keys, List<string> keysProduccion)
        {
            _logger.LogDebug($"Datos del detalle, Empresa: {detalleAnulacion.Empresa} - Cliente: {detalleAnulacion.Cliente} - Ubicación: {detalleAnulacion.Ubicacion} - Pedido: {detalleAnulacion.Pedido} -Producto: {detalleAnulacion.Producto} - Lote: {detalleAnulacion.Lote} - Faixa: {detalleAnulacion.Faixa} - NumeroSecuencia: {detalleAnulacion.NumeroSecuencia}");

            var estadosAnulacion = new List<string>() { EstadoDetallePreparacion.ESTADO_ANULACION_PENDIENTE, EstadoDetallePreparacion.ESTADO_ANULACION_EJECUTADA_DOC };

            if (estadosAnulacion.Contains(detalleAnulacion.EstadoDetPicking) && detalleAnulacion.Cantidad > 0)
            {
                DetallePreparacionLpn detPickingLpn = null;
                if (detalleAnulacion.IdDetallePickingLpn != null)
                    detPickingLpn = _uow.AnulacionRepository.GetDetallePreparacionLpn(detalleAnulacion.NumeroPreparacion, detalleAnulacion.IdDetallePickingLpn.Value, connection, tran);

                ProcesoPicking(connection, tran, detalleAnulacion, detPickingLpn, nuTransaccion);
                ProcesoStock(connection, tran, detalleAnulacion, nuTransaccion);
                ProcesoPedido(connection, tran, detalleAnulacion, detPickingLpn, nuTransaccion, keys, keysProduccion);
                ProcesoClienteCamion(connection, tran, detalleAnulacion);
            }
        }

        #region Picking
        public virtual void ProcesoPicking(DbConnection connection, DbTransaction tran, AnulacionPreparacionDetalle detalleAnulacion, DetallePreparacionLpn detPickingLpn, long nuTransaccion)
        {
            //Actualizo el detalle de picking
            detalleAnulacion.CantidadPreparada = 0;
            detalleAnulacion.EstadoDetPicking = EstadoDetallePreparacion.ESTADO_ANULACION_EJECUTADA;
            detalleAnulacion.Transaccion = nuTransaccion;

            _uow.AnulacionRepository.UpdateDetPicking(detalleAnulacion, connection, tran);

            if (detPickingLpn != null)
            {
                if (detPickingLpn.IdDetalleLpn != null)
                {
                    var updDetalleLpn = new LpnDetalle()
                    {
                        Id = detPickingLpn.IdDetalleLpn.Value,
                        NumeroLPN = detPickingLpn.NroLpn.Value,
                        Empresa = detPickingLpn.Empresa.Value,
                        CodigoProducto = detPickingLpn.Producto,
                        Faixa = detPickingLpn.Faixa.Value,
                        Lote = detPickingLpn.Lote,
                        Cantidad = detPickingLpn.CantidadReservada,
                        NumeroTransaccion = nuTransaccion
                    };

                    _uow.AnulacionRepository.UpdateDetalleLpn(updDetalleLpn, connection, tran);
                }

                detPickingLpn.CantidadReservada = 0;
                detPickingLpn.Transaccion = nuTransaccion;
                detPickingLpn.FechaModificacion = DateTime.Now;

                _uow.AnulacionRepository.UpdateDetallePreparacionLpn(detPickingLpn, connection, tran);
            }
        }

        public virtual void ProcesoClienteCamion(DbConnection connection, DbTransaction tran, AnulacionPreparacionDetalle detalleAnulacion)
        {
            var pendDetPicking = _uow.AnulacionRepository.PendienteDetPicking(detalleAnulacion, connection, tran);
            var pedido = _uow.PedidoRepository.GetPedido(detalleAnulacion.Empresa, detalleAnulacion.Cliente, detalleAnulacion.Pedido);

            if (!pendDetPicking && pedido.NuCarga == null)
            {
                var clienteCamion = _uow.AnulacionRepository.GetClienteCamion(detalleAnulacion, connection, tran);
                if (clienteCamion != null)
                {
                    _uow.AnulacionRepository.DeleteClienteCamion(clienteCamion, connection, tran);
                    _trackingService.CambiarEstadoSincronizacion(_uow, clienteCamion.Camion, false);
                }
            }
        }
        #endregion

        #region Stock

        public virtual void ProcesoStock(DbConnection connection, DbTransaction tran, AnulacionPreparacionDetalle detalleAnulacion, long nuTransaccion)
        {
            if (detalleAnulacion.Lote != ManejoIdentificadorDb.IdentificadorAuto)
            {
                var stock = ComprueboStock(connection, tran, detalleAnulacion, nuTransaccion);

                stock.ReservaSalida -= detalleAnulacion.Cantidad;
                stock.NumeroTransaccion = nuTransaccion;

                _uow.AnulacionRepository.UpdateStock(stock, tran, connection);
            }
            else
                ProcesoStockAuto(connection, tran, detalleAnulacion, nuTransaccion);
        }

        public virtual void ProcesoStockAuto(DbConnection connection, DbTransaction tran, AnulacionPreparacionDetalle detalleAnulacion, long nuTransaccion)
        {
            decimal? auxCantidad = 0;
            decimal? auxCantidadAnular = detalleAnulacion.Cantidad;
            var stocks = _uow.StockRepository.GetStock(detalleAnulacion.Ubicacion, detalleAnulacion.Empresa, detalleAnulacion.Producto, detalleAnulacion.Faixa, connection, tran);

            foreach (var stock in stocks)
            {
                decimal reservaStock = stock.ReservaSalida ?? 0;
                var cantProdPrepPendiente = _uow.AnulacionRepository.GetCantidadProducto(stock, tran, connection);

                var nuevaReservaStock = reservaStock - cantProdPrepPendiente;

                if (auxCantidadAnular > nuevaReservaStock)
                    auxCantidad = nuevaReservaStock;
                else
                    auxCantidad = auxCantidadAnular;

                if (auxCantidad > 0)
                {
                    var resta = nuevaReservaStock - auxCantidad;
                    var aux = resta <= 0 ? 0 : resta;

                    stock.ReservaSalida = aux;
                    stock.NumeroTransaccion = nuTransaccion;

                    _uow.AnulacionRepository.UpdateStock(stock, tran, connection);

                    auxCantidadAnular = auxCantidadAnular - auxCantidad;
                }

                if (auxCantidadAnular == 0)
                    break;
            }
        }

        public virtual Stock ComprueboStock(DbConnection connection, DbTransaction tran, AnulacionPreparacionDetalle detalleAnulacion, long nuTransaccion)
        {
            var stock = _uow.StockRepository.GetStock(detalleAnulacion.Ubicacion, detalleAnulacion.Empresa, detalleAnulacion.Producto, detalleAnulacion.Faixa, detalleAnulacion.Lote, connection, tran);
            if (stock == null)
            {
                //Genero linea de stock solo con reserva
                var s = new Stock()
                {
                    Ubicacion = detalleAnulacion.Ubicacion,
                    Empresa = detalleAnulacion.Empresa,
                    Producto = detalleAnulacion.Producto,
                    Faixa = detalleAnulacion.Faixa,
                    Identificador = detalleAnulacion.Lote,
                    Cantidad = 0,
                    ReservaSalida = detalleAnulacion.Cantidad,
                    CantidadTransitoEntrada = 0,
                    Averia = "N",
                    Inventario = "R",
                    ControlCalidad = EstadoControlCalidad.Controlado,
                    NumeroTransaccion = nuTransaccion,
                };

                _uow.AnulacionRepository.AddStock(s, tran, connection);
                stock = s;
            }
            else if (stock.ReservaSalida == 0)
            {
                throw new ValidationFailedException("General_msg_Error_AnulacionStockSinReserva",
                    [detalleAnulacion.NumeroPreparacion.ToString(), detalleAnulacion.Ubicacion, detalleAnulacion.Empresa.ToString(), detalleAnulacion.Producto, detalleAnulacion.Faixa.ToString(), detalleAnulacion.Lote]);
            }

            return stock;
        }

        #endregion

        #region Pedido
        public virtual void ProcesoPedido(DbConnection connection, DbTransaction tran, AnulacionPreparacionDetalle detalleAnulacion, DetallePreparacionLpn detPickingLpn, long nuTransaccion, List<string> keys, List<string> keysProduccion)
        {
            var modalidadAnulacionLpn = GetModalidadAnulacionLpn(detPickingLpn);

            if (detalleAnulacion.TipoAnulacion == TipoAnulacion.Preparacion)
            {
                if (detalleAnulacion.Lote != ManejoIdentificadorDb.IdentificadorAuto && detalleAnulacion.EspecificaLote == "N")
                {
                    var detPedidoAuto = _uow.AnulacionRepository.GetDetalleAUTO(detalleAnulacion, connection, tran);
                    if (detPedidoAuto != null)
                    {
                        detPedidoAuto.Cantidad = (detPedidoAuto.Cantidad ?? 0) + detalleAnulacion.Cantidad;
                        detPedidoAuto.CantidadOriginal = (detPedidoAuto.CantidadOriginal ?? 0) + detalleAnulacion.Cantidad;
                        detPedidoAuto.Transaccion = nuTransaccion;

                        _uow.AnulacionRepository.UpdateDetPedido(detPedidoAuto, tran, connection);
                    }

                    ProcesoLpnAuto(detalleAnulacion, detPickingLpn, modalidadAnulacionLpn, nuTransaccion, connection, tran);//Respetar orden ejecución por FK

                    var detalles = _uow.AnulacionRepository.GetDetallesAsociados(detalleAnulacion, connection, tran);

                    if (detalles != null && detalles.Count > 0)
                    {
                        detalles.ForEach(d => d.TransaccionDelete = d.Transaccion = nuTransaccion);

                        _uow.AnulacionRepository.UpdateDetPedido(detalles, tran, connection);
                        _uow.AnulacionRepository.DeleteDetPedido(detalles, tran, connection);
                    }
                    else
                    {
                        var detPedido = _uow.AnulacionRepository.GetDetalle(detalleAnulacion, connection, tran);
                        if (detPedido != null)
                        {
                            detPedido.Transaccion = nuTransaccion;
                            detPedido.Cantidad = (detPedido.Cantidad ?? 0) - detalleAnulacion.Cantidad;
                            detPedido.CantidadLiberada = (detPedido.CantidadLiberada ?? 0) - detalleAnulacion.Cantidad;
                            detPedido.CantidadOriginal = (detPedido.CantidadOriginal ?? 0) - detalleAnulacion.Cantidad;

                            _uow.AnulacionRepository.UpdateDetPedido(detPedido, tran, connection);
                        }
                    }

                }
                else
                {
                    var detPedido = _uow.AnulacionRepository.GetDetalle(detalleAnulacion, connection, tran);
                    if (detPedido != null)
                    {
                        detPedido.CantidadLiberada = (detPedido.CantidadLiberada ?? 0) - detalleAnulacion.Cantidad;
                        detPedido.Transaccion = nuTransaccion;

                        _uow.AnulacionRepository.UpdateDetPedido(detPedido, tran, connection);

                        ProcesoLpn(detalleAnulacion, detPickingLpn, modalidadAnulacionLpn, nuTransaccion, connection, tran, idLogPedidoAnulado: null);
                    }
                }
            }
            else
            {
                var detPedido = _uow.AnulacionRepository.GetDetalle(detalleAnulacion, connection, tran);
                if (detPedido != null)
                {
                    detPedido.CantidadLiberada = (detPedido.CantidadLiberada ?? 0) - detalleAnulacion.Cantidad;
                    detPedido.CantidadAnulada = (detPedido.CantidadAnulada ?? 0) + detalleAnulacion.Cantidad;
                    detPedido.Transaccion = nuTransaccion;

                    _uow.AnulacionRepository.UpdateDetPedido(detPedido, tran, connection);

                    var logPedidoAnulado = LogPedidoAnulado(connection, tran, detalleAnulacion, keys);

                    ProcesoLpn(detalleAnulacion, detPickingLpn, modalidadAnulacionLpn, nuTransaccion, connection, tran, logPedidoAnulado.Id);

                    var pedido = _uow.AnulacionRepository.GetPedido(detalleAnulacion, connection, tran);

                    AnulacionDePreparaciones.FinalizarProduccion(_uow, pedido, out bool isProduccionFinalizada);

                    if (isProduccionFinalizada && !keysProduccion.Any(x => x == pedido.IngresoProduccion))
                        keysProduccion.Add(pedido.IngresoProduccion);

                }
            }
        }

        public virtual PedidoAnulado LogPedidoAnulado(DbConnection connection, DbTransaction tran, AnulacionPreparacionDetalle detalle, List<string> keys)
        {
            var idLogPedido = _uow.AnulacionRepository.GetNextIdLogPedidoAnulado(connection, tran);

            long? manejaInt = null;
            if (_manejaInterfaz)
            {
                manejaInt = -1;
                var keyPedido = $"{detalle.Pedido}#{detalle.Cliente}#{detalle.Empresa}";
                if (!keys.Contains(keyPedido))
                    keys.Add(keyPedido);
            }

            var logPedido = new PedidoAnulado()
            {
                Id = idLogPedido,
                Pedido = detalle.Pedido,
                Empresa = detalle.Empresa,
                Cliente = detalle.Cliente,
                Producto = detalle.Producto,
                Embalaje = detalle.Faixa,
                Identificador = detalle.Lote,
                EspecificaIdentificadorId = detalle.EspecificaLote,
                CantidadAnulada = detalle.Cantidad,
                Motivo = "Liberación anuladada",
                Funcionario = detalle.UserIdAnulacion,
                Aplicacion = _identity.Application,
                InterfazEjecucion = manejaInt
            };

            _uow.AnulacionRepository.AddLogPedidoAnulado(logPedido, tran, connection);

            return logPedido;
        }

        public virtual string GetModalidadAnulacionLpn(DetallePreparacionLpn detPickingLpn)
        {
            if (detPickingLpn != null)
            {
                if (detPickingLpn.IdDetalleLpn != null)
                    return TipoAnulacionLpn.PedidoLpn;
                else if (detPickingLpn.IdDetalleLpn == null && detPickingLpn.IdConfiguracion != null && !string.IsNullOrEmpty(detPickingLpn.Atributos) && !string.IsNullOrEmpty(detPickingLpn.TipoLpn) && !string.IsNullOrEmpty(detPickingLpn.IdExternoLpn))
                    return TipoAnulacionLpn.PedidoLpnAtributo;
                else if (detPickingLpn.IdDetalleLpn == null && detPickingLpn.IdConfiguracion != null && !string.IsNullOrEmpty(detPickingLpn.Atributos) && string.IsNullOrEmpty(detPickingLpn.IdExternoLpn))
                    return TipoAnulacionLpn.PedidoAtributo;
            }

            return null;
        }

        public virtual void ProcesoLpn(AnulacionPreparacionDetalle detalleAnulacion, DetallePreparacionLpn detPickingLpn, string modalidadAnulacionLpn, long nuTransaccion, DbConnection connection, DbTransaction tran, long? idLogPedidoAnulado)
        {
            if (!string.IsNullOrEmpty(modalidadAnulacionLpn))
            {
                var bajarCantAnulada = detalleAnulacion.TipoAnulacion == TipoAnulacion.PreparacionPedido;

                switch (modalidadAnulacionLpn)
                {
                    case TipoAnulacionLpn.PedidoLpn:
                    case TipoAnulacionLpn.PedidoLpnAtributo:

                        var detPedidoLpn = _uow.AnulacionRepository.GetDetallePedidoLpn(detalleAnulacion, detPickingLpn, connection, tran, auto: false);
                        if (detPedidoLpn != null)
                        {
                            detPedidoLpn.CantidadLiberada = (detPedidoLpn.CantidadLiberada ?? 0) - detalleAnulacion.Cantidad;
                            detPedidoLpn.Transaccion = nuTransaccion;
                            detPedidoLpn.FechaModificacion = DateTime.Now;

                            if (bajarCantAnulada)
                                detPedidoLpn.CantidadAnulada = (detPedidoLpn.CantidadAnulada ?? 0) + detalleAnulacion.Cantidad;

                            _uow.AnulacionRepository.UpdateDetallePedidoLpn(detPedidoLpn, connection, tran);

                            if (bajarCantAnulada && idLogPedidoAnulado.HasValue)
                            {
                                var pedidoAnuladoLpn = new PedidoAnuladoLpn
                                {
                                    Id = _uow.AnulacionRepository.GetNextIdLogPedidoAnuladoLpn(connection, tran),
                                    IdLogPedidoAnulado = idLogPedidoAnulado.Value,
                                    TipoOperacion = modalidadAnulacionLpn,
                                    IdExternoLpn = detPedidoLpn.IdLpnExterno,
                                    TipoLpn = detPedidoLpn.Tipo,
                                    CantidadAnulada = detalleAnulacion.Cantidad,
                                    FechaInsercion = DateTime.Now,
                                };

                                _uow.AnulacionRepository.AddLogPedidoAnuladoLpn(pedidoAnuladoLpn, connection, tran);

                            }

                            if (modalidadAnulacionLpn == TipoAnulacionLpn.PedidoLpnAtributo)
                            {
                                var detPedidoLpnAtributo = _uow.AnulacionRepository.GetDetallePedidoLpnAtributo(detalleAnulacion, detPickingLpn, connection, tran, auto: false);

                                if (detPedidoLpnAtributo != null)
                                {
                                    detPedidoLpnAtributo.CantidadLiberada = (detPedidoLpnAtributo.CantidadLiberada ?? 0) - detalleAnulacion.Cantidad;
                                    detPedidoLpnAtributo.Transaccion = nuTransaccion;
                                    detPedidoLpnAtributo.FechaModificacion = DateTime.Now;

                                    if (bajarCantAnulada)
                                        detPedidoLpnAtributo.CantidadAnulada = (detPedidoLpnAtributo.CantidadAnulada ?? 0) + detalleAnulacion.Cantidad;

                                    _uow.AnulacionRepository.UpdateDetallePedidoLpnAtributo(detPedidoLpnAtributo, connection, tran);

                                    if (bajarCantAnulada && idLogPedidoAnulado.HasValue)
                                    {
                                        var pedidoAnuladoLpn = new PedidoAnuladoLpn
                                        {
                                            Id = _uow.AnulacionRepository.GetNextIdLogPedidoAnuladoLpn(connection, tran),
                                            IdLogPedidoAnulado = idLogPedidoAnulado.Value,
                                            TipoOperacion = modalidadAnulacionLpn,
                                            IdExternoLpn = detPedidoLpnAtributo.IdLpnExterno,
                                            TipoLpn = detPedidoLpnAtributo.Tipo,
                                            IdConfiguracion = detPedidoLpnAtributo.IdConfiguracion,
                                            CantidadAnulada = detalleAnulacion.Cantidad,
                                            FechaInsercion = DateTime.Now,
                                        };

                                        _uow.AnulacionRepository.AddLogPedidoAnuladoLpn(pedidoAnuladoLpn, connection, tran);
                                    }
                                }
                            }
                        }
                        break;
                    case TipoAnulacionLpn.PedidoAtributo:

                        var detPedidoAtributo = _uow.AnulacionRepository.GetDetallePedidoAtributo(detalleAnulacion, detPickingLpn.IdConfiguracion.Value, connection, tran, auto: false);

                        if (detPedidoAtributo != null)
                        {
                            detPedidoAtributo.CantidadLiberada = (detPedidoAtributo.CantidadLiberada ?? 0) - detalleAnulacion.Cantidad;
                            detPedidoAtributo.Transaccion = nuTransaccion;
                            detPedidoAtributo.FechaModificacion = DateTime.Now;

                            if (bajarCantAnulada)
                                detPedidoAtributo.CantidadAnulada = (detPedidoAtributo.CantidadAnulada ?? 0) + detalleAnulacion.Cantidad;

                            _uow.AnulacionRepository.UpdateDetallePedidoAtributo(detPedidoAtributo, connection, tran);

                            if (bajarCantAnulada && idLogPedidoAnulado.HasValue)
                            {
                                var pedidoAnuladoLpn = new PedidoAnuladoLpn
                                {
                                    Id = _uow.AnulacionRepository.GetNextIdLogPedidoAnuladoLpn(connection, tran),
                                    IdLogPedidoAnulado = idLogPedidoAnulado.Value,
                                    TipoOperacion = modalidadAnulacionLpn,
                                    IdConfiguracion = detPedidoAtributo.IdConfiguracion,
                                    CantidadAnulada = detalleAnulacion.Cantidad,
                                    FechaInsercion = DateTime.Now,
                                };

                                _uow.AnulacionRepository.AddLogPedidoAnuladoLpn(pedidoAnuladoLpn, connection, tran);
                            }
                        }
                        break;
                }
            }
        }

        public virtual void ProcesoLpnAuto(AnulacionPreparacionDetalle detalleAnulacion, DetallePreparacionLpn detPickingLpn, string modalidadAnulacionLpn, long nuTransaccion, DbConnection connection, DbTransaction tran)
        {
            if (!string.IsNullOrEmpty(modalidadAnulacionLpn))
            {
                switch (modalidadAnulacionLpn)
                {
                    case TipoAnulacionLpn.PedidoLpn:
                    case TipoAnulacionLpn.PedidoLpnAtributo:

                        var detPedidoLpnAuto = _uow.AnulacionRepository.GetDetallePedidoLpn(detalleAnulacion, detPickingLpn, connection, tran, auto: true);
                        if (detPedidoLpnAuto != null)
                        {
                            detPedidoLpnAuto.CantidadPedida = (detPedidoLpnAuto.CantidadPedida ?? 0) + detalleAnulacion.Cantidad;
                            detPedidoLpnAuto.Transaccion = nuTransaccion;
                            detPedidoLpnAuto.FechaModificacion = DateTime.Now;

                            _uow.AnulacionRepository.UpdateDetallePedidoLpn(detPedidoLpnAuto, connection, tran);

                            if (modalidadAnulacionLpn == TipoAnulacionLpn.PedidoLpnAtributo)
                            {
                                var detPedidoLpnAtributoAuto = _uow.AnulacionRepository.GetDetallePedidoLpnAtributo(detalleAnulacion, detPickingLpn, connection, tran, auto: true);
                                if (detPedidoLpnAtributoAuto != null)
                                {
                                    detPedidoLpnAtributoAuto.CantidadPedida += detalleAnulacion.Cantidad;
                                    detPedidoLpnAtributoAuto.Transaccion = nuTransaccion;
                                    detPedidoLpnAtributoAuto.FechaModificacion = DateTime.Now;

                                    _uow.AnulacionRepository.UpdateDetallePedidoLpnAtributo(detPedidoLpnAtributoAuto, connection, tran);
                                }
                            }
                        }

                        if (modalidadAnulacionLpn == TipoAnulacionLpn.PedidoLpnAtributo)
                        {
                            var detPedidoLpnAtributo = _uow.AnulacionRepository.GetDetallePedidoLpnAtributo(detalleAnulacion, detPickingLpn, connection, tran, auto: false);
                            if (detPedidoLpnAtributo != null)
                            {
                                detPedidoLpnAtributo.CantidadPedida -= detalleAnulacion.Cantidad;
                                detPedidoLpnAtributo.CantidadLiberada = (detPedidoLpnAtributo.CantidadLiberada ?? 0) - detalleAnulacion.Cantidad;
                                detPedidoLpnAtributo.FechaModificacion = DateTime.Now;
                                detPedidoLpnAtributo.Transaccion = nuTransaccion;
                                detPedidoLpnAtributo.TransaccionDelete = nuTransaccion;

                                if (detPedidoLpnAtributo.CantidadPedida > 0)
                                    _uow.AnulacionRepository.UpdateDetallePedidoLpnAtributo(detPedidoLpnAtributo, connection, tran);
                                else
                                    _uow.AnulacionRepository.DeleteDetallePedidoLpnAtributo(detPedidoLpnAtributo, connection, tran);
                            }
                        }

                        var detPedidoLpn = _uow.AnulacionRepository.GetDetallePedidoLpn(detalleAnulacion, detPickingLpn, connection, tran, auto: false);
                        if (detPedidoLpn != null)
                        {
                            detPedidoLpn.CantidadPedida = (detPedidoLpn.CantidadPedida ?? 0) - detalleAnulacion.Cantidad;
                            detPedidoLpn.CantidadLiberada = (detPedidoLpn.CantidadLiberada ?? 0) - detalleAnulacion.Cantidad;
                            detPedidoLpn.FechaModificacion = DateTime.Now;
                            detPedidoLpn.Transaccion = nuTransaccion;
                            detPedidoLpn.TransaccionDelete = nuTransaccion;

                            if (detPedidoLpn.CantidadPedida > 0)
                                _uow.AnulacionRepository.UpdateDetallePedidoLpn(detPedidoLpn, connection, tran);
                            else
                                _uow.AnulacionRepository.DeleteDetallePedidoLpn(detPedidoLpn, connection, tran);
                        }

                        break;
                    case TipoAnulacionLpn.PedidoAtributo:

                        var detPedidoAtributoAuto = _uow.AnulacionRepository.GetDetallePedidoAtributo(detalleAnulacion, detPickingLpn.IdConfiguracion.Value, connection, tran, auto: true);

                        detPedidoAtributoAuto.CantidadPedida += detalleAnulacion.Cantidad;
                        detPedidoAtributoAuto.Transaccion = nuTransaccion;
                        detPedidoAtributoAuto.FechaModificacion = DateTime.Now;

                        _uow.AnulacionRepository.UpdateDetallePedidoAtributo(detPedidoAtributoAuto, connection, tran);

                        var detPedidoAtributo = _uow.AnulacionRepository.GetDetallePedidoAtributo(detalleAnulacion, detPickingLpn.IdConfiguracion.Value, connection, tran, auto: false);
                        if (detPedidoAtributo != null)
                        {
                            detPedidoAtributo.CantidadPedida -= detalleAnulacion.Cantidad;
                            detPedidoAtributo.CantidadLiberada = (detPedidoAtributo.CantidadLiberada ?? 0) - detalleAnulacion.Cantidad;
                            detPedidoAtributo.FechaModificacion = DateTime.Now;
                            detPedidoAtributo.Transaccion = nuTransaccion;
                            detPedidoAtributo.TransaccionDelete = nuTransaccion;

                            if (detPedidoAtributo.CantidadPedida > 0)
                                _uow.AnulacionRepository.UpdateDetallePedidoAtributo(detPedidoAtributo, connection, tran);
                            else
                                _uow.AnulacionRepository.DeleteDetallePedidoAtributo(detPedidoAtributo, connection, tran);
                        }

                        break;
                }
            }
        }

        #endregion

        #region Produccion

        public static void FinalizarProduccion(IUnitOfWork uow, Pedido pedido, out bool isProduccionFinalizada)
        {
            isProduccionFinalizada = false;
            if (pedido.IngresoProduccion != null)
            {
                var ingreso = uow.IngresoProduccionRepository.GetIngresoById(pedido.IngresoProduccion);
                if (ingreso.Tipo == TipoIngresoProduccion.Colector)
                {
                    uow.SaveChanges();

                    if (!uow.PreparacionRepository.AnyPendientesEnsamblarProduccion(pedido.IngresoProduccion) &&
                        !uow.PreparacionRepository.AnyPendientesContenedorEnsamblarProduccion(pedido.IngresoProduccion) &&
                        !uow.PreparacionRepository.AnyPedidoProduccionFinalizado(pedido.IngresoProduccion) && ingreso.Situacion != SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_FINAL)
                    {
                        ingreso.Situacion = SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_FINAL;
                        ingreso.NuTransaccion = uow.GetTransactionNumber();

                        uow.IngresoProduccionRepository.UpdateIngresoProduccion(ingreso);

                        uow.SaveChanges();

                        isProduccionFinalizada = true;
                    }
                }
            }
        }

        #endregion
    }
}
