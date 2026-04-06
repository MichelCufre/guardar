using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Automation;
using WIS.AutomationManager.Interfaces;
using WIS.Domain.Automatismo;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.Automatismo.Enums;
using WIS.Domain.Automatismo.Interfaces;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Picking;

namespace WIS.AutomationManager.Services
{
    public class AutomatismoAutoStoreNotificationService : AutomatismoNotificationService, IAutomatismoNotificationService
    {
        protected ILogger _logger;

        public AutomatismoAutoStoreNotificationService(
            IAutomatismoWmsApiClientService wmsApiClientService,
            IAutomatismoInterpreterClientService interpreterService,
            ILogger logger) : base(wmsApiClientService, interpreterService)
        {
            _logger = logger;
        }

        public override AutomatismoResponse NotificarProductos(IAutomatismo automatismo, ProductosAutomatismoRequest request)
        {
            return this.CallInterpreterSendProductos(automatismo, request);
        }

        public override AutomatismoResponse NotificarCodigosBarras(IAutomatismo automatismo, CodigosBarrasAutomatismoRequest request)
        {
            return this.CallInterpreterSendCodigosBarras(automatismo, request);
        }

        public override AutomatismoResponse NotificarEntrada(IAutomatismo automatismo, EntradaStockAutomatismoRequest request)
        {
            return this.CallInterpreterSendEntrada(automatismo, request);
        }

        public override AutomatismoResponse NotificarSalida(IAutomatismo automatismo, SalidaStockAutomatismoRequest request)
        {
            return this.CallInterpreterSendSalida(automatismo, request);
        }

        public override ValidationsResult ProcesarConfirmacionSalida(IUnitOfWork uow, IAutomatismo automatismo, PickingRequest request)
        {
            ValidationsResult response = null;
            var nuEjecucionSalida = int.Parse(request.IdRequest);

            if (request.EstadoSalida == AutomatismoEstadoSalidaGalys.CierreDeBulto
                && request.Detalles != null
                && request.Detalles.Count > 0)
            {

                var posicionPicking = automatismo.GetPosiciones(AutomatismoPosicionesTipoDb.POS_PICKING).FirstOrDefault();
                var posicionSalida = automatismo.GetPosiciones(AutomatismoPosicionesTipoDb.POS_SALIDA).FirstOrDefault();
                var tipoContenedor = automatismo.GetCaracteristicaByCodigo(CaracteristicasAutomatismoDb.TIPO_CONTENEDOR)?.Valor;

                var ejecucionSalida = uow.AutomatismoEjecucionRepository.GetAutomatismoEjecucionWithData(nuEjecucionSalida);
                var ejecucionSalidaData = ejecucionSalida.AutomatismoData.OrderByDescending(o => o.Id).FirstOrDefault();
                var salidaNotificada = JsonConvert.DeserializeObject<SalidaStockAutomatismoRequest>(ejecucionSalidaData.RequestData);
                var primerDetalleSalidaNotificada = salidaNotificada.Detalles.FirstOrDefault();

                var prep = uow.PreparacionRepository.GetPreparacionPorNumero(salidaNotificada.Detalles.FirstOrDefault().Preparacion);

                foreach (var det in request.Detalles)
                {
                    det.Agrupacion = prep.Agrupacion;

                    det.TipoContenedor = tipoContenedor;
                    det.Ubicacion = posicionPicking.IdUbicacion;
                    det.UbicacionContenedor = posicionSalida.IdUbicacion;
                    det.Preparacion = prep.Id;

                    switch (prep.Agrupacion)
                    {
                        case Agrupacion.Pedido:
                            det.Pedido = primerDetalleSalidaNotificada.Pedido;
                            det.TipoAgente = primerDetalleSalidaNotificada.TipoAgente;
                            det.CodigoAgente = primerDetalleSalidaNotificada.CodigoAgente;
                            break;
                        case Agrupacion.Cliente:
                            det.TipoAgente = primerDetalleSalidaNotificada.TipoAgente;
                            det.CodigoAgente = primerDetalleSalidaNotificada.CodigoAgente;
                            break;
                        case Agrupacion.Ruta:
                            det.Carga = primerDetalleSalidaNotificada.Carga;
                            det.ComparteContenedorPicking = primerDetalleSalidaNotificada.ComparteContenedorPicking;
                            break;
                        case Agrupacion.Onda:
                            det.ComparteContenedorPicking = primerDetalleSalidaNotificada.ComparteContenedorPicking;
                            break;
                    }
                }

                request.EstadoDetalle = EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO;
                request.DsReferencia = automatismo.Codigo;
                request.Preparacion = prep.Id;
                request.IdRequest = null;

                return this.CallWMSSendSalidas(automatismo, request);

            }

            else if ((request.EstadoSalida == AutomatismoEstadoSalidaGalys.OrdenFinalizada
                      || request.EstadoSalida == AutomatismoEstadoSalidaGalys.OrdenCancelada)
                      && request.DetallesFinalizados != null
                      && request.DetallesFinalizados.Count > 0)
            {

                if (request.DetallesFinalizados.Any(w => w.CantidadSolicitada > w.CantidadPreparada))
                {
                    var anulaciones = new AnularPickingPedidoPendienteRequest()
                    {
                        Empresa = request.Empresa,
                        DsReferencia = request.DsReferencia,
                        Archivo = request.Archivo,
                        Usuario = request.Usuario
                    };

                    var ejecucionSalida = uow.AutomatismoEjecucionRepository.GetAutomatismoEjecucionWithData(nuEjecucionSalida);
                    var ejecucionSalidaData = ejecucionSalida.AutomatismoData.OrderByDescending(o => o.Id).FirstOrDefault();
                    var salidaNotificada = JsonConvert.DeserializeObject<SalidaStockAutomatismoRequest>(ejecucionSalidaData.RequestData);
                    var primerDetalleSalidaNotificada = salidaNotificada.Detalles.FirstOrDefault();

                    var prep = uow.PreparacionRepository.GetPreparacionPorNumero(salidaNotificada.Detalles.FirstOrDefault().Preparacion);

                    var anulacion = new AnulacionPedidoPendienteRequest()
                    {
                        AgrupacionPreparacion = prep.Agrupacion,
                        Preparacion = prep.Id,
                        EstadoPicking = EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO
                    };

                    switch (prep.Agrupacion)
                    {
                        case Agrupacion.Pedido:
                            anulacion.Pedido = primerDetalleSalidaNotificada.Pedido;
                            anulacion.TipoAgente = primerDetalleSalidaNotificada.TipoAgente;
                            anulacion.CodigoAgente = primerDetalleSalidaNotificada.CodigoAgente;
                            break;
                        case Agrupacion.Cliente:
                            anulacion.TipoAgente = primerDetalleSalidaNotificada.TipoAgente;
                            anulacion.CodigoAgente = primerDetalleSalidaNotificada.CodigoAgente;
                            break;
                        case Agrupacion.Ruta:
                            anulacion.Carga = primerDetalleSalidaNotificada.Carga;
                            anulacion.ComparteContenedorPicking = primerDetalleSalidaNotificada.ComparteContenedorPicking;
                            break;
                        case Agrupacion.Onda:
                            anulacion.ComparteContenedorPicking = primerDetalleSalidaNotificada.ComparteContenedorPicking;
                            break;
                    }

                    anulaciones.Detalles.Add(anulacion);

                    List<DetallePreparacion> detalles = new List<DetallePreparacion>();

                    foreach (var det in request.DetallesFinalizados.Where(w => w.CantidadSolicitada > w.CantidadPreparada))
                    {
                        anulacion.ProductosAnular.Add(new AnulacionPedidoPendienteDetalleRequest
                        {
                            CodigoProducto = det.CodigoProducto,
                            CantidadAnular = det.CantidadSolicitada - det.CantidadPreparada,
                            Identificador = "*" //No llega desde Galys
                        });
                    }

                    if (anulaciones.Detalles != null && anulaciones.Detalles.Count > 0)
                    {
                        response = this.CallWMSSendAnularPendiente(automatismo, anulaciones);
                    }
                }



                if (response?.IsValid() ?? true)
                {
                    var ejecucionSalida = uow.AutomatismoEjecucionRepository.GetAutomatismoEjecucionWithData(nuEjecucionSalida);
                    ejecucionSalida.FechaModificacion = DateTime.Now;
                    ejecucionSalida.Estado = EstadoEjecucion.PROCESADO_FIN;
                    ejecucionSalida.Transaccion = uow.GetTransactionNumber();
                    uow.AutomatismoEjecucionRepository.Update(ejecucionSalida);
                    uow.SaveChanges();

                    //  uow.AutomatismoEjecucionRepository.RemoveConfirmacionAutomatismoEntrada(request.IdRequest);
                    //  uow.SaveChanges();

                }

            }

            return response;
        }

        public override ValidationsResult ProcesarConfirmacionEntrada(IUnitOfWork uow, IAutomatismo automatismo, TransferenciaStockRequest request, EntradaStockAutomatismoRequest entradaNotificada, AutomatismoConfirmacionEntradaServiceContext context)
        {
            ValidationsResult response = null;

            var requestTransferencia = AgruparTransferencias(request);

            if (request.Transferencias.Count > 0)
                response = this.CallWMSSendMovimiento(automatismo, requestTransferencia);

            if (response?.IsValid() ?? true)
            {
                var interfazEntrada = context.GetAutomatismoEjecucionEntrada();
                interfazEntrada.FechaModificacion = DateTime.Now;
                interfazEntrada.Estado = EstadoEjecucion.PROCESADO_FIN;
                interfazEntrada.Transaccion = uow.GetTransactionNumber();

                uow.AutomatismoEjecucionRepository.Update(interfazEntrada);
                uow.SaveChanges();
            }

            return response;
        }

        public TransferenciaStockRequest AgruparTransferencias(TransferenciaStockRequest request)
        {
            var requestAgrupada = new TransferenciaStockRequest();
            requestAgrupada.Empresa = request.Empresa;
            requestAgrupada.DsReferencia = request.DsReferencia;
            requestAgrupada.Archivo = request.Archivo;
            requestAgrupada.IdEntrada = request.IdEntrada;
            requestAgrupada.Usuario = request.Usuario;
            if (request.Transferencias != null && request.Transferencias.Count > 0)
            {
                var transferencias = request.Transferencias
                    .GroupBy(d => new
                    {
                        d.CodigoProducto,
                        d.Identificador,
                        d.Ubicacion,
                        d.UbicacionDestino,
                    })
                    .Select(g => new TransferenciaRequest
                    {
                        CodigoProducto = g.Key.CodigoProducto,
                        Identificador = g.Key.Identificador,
                        Ubicacion = g.Key.Ubicacion,
                        UbicacionDestino = g.Key.UbicacionDestino,
                        Cantidad = g.Sum(x => x.Cantidad)
                    })
                    .ToList();

                requestAgrupada.Transferencias.Clear();
                requestAgrupada.Transferencias.AddRange(transferencias);
            }

            return requestAgrupada;
        }

        public override ValidationsResult ProcesarNotificacionAjustes(IUnitOfWork uow, IAutomatismo automatismo, AjustesDeStockRequest request)
        {
            var response = this.CallWMSSendAjustes(automatismo, request);

            if (!response.HasError())
            {
                var posicionSalida = automatismo.GetPosiciones(AutomatismoPosicionesTipoDb.POS_SALIDA).FirstOrDefault();
                var motivo = (automatismo.GetCaracteristicaByCodigo(AutomatismoDb.CARACTERISTICA_MOTIVO_AJUSTE_VACIADO_UBICACION)?.Valor) ?? CaracteristicasAutomatismoDb.CD_MOTIVO_AJUSTE_SALIDA_MANUAL;

                request.Ajustes = request.Ajustes
                    .Where(w => w.Cantidad < 0 && w.MotivoAjuste == motivo)
                    .ToList();

                foreach (var ajuste in request.Ajustes)
                {
                    ajuste.Ubicacion = posicionSalida.IdUbicacion;
                    ajuste.Cantidad = System.Math.Abs(ajuste.Cantidad);
                }

                if (request.Ajustes.Count > 0)
                {
                    response = this.CallWMSSendAjustes(automatismo, request);
                }
            }

            return response;
        }

        public override ValidationsResult ProcesarConfirmacionMovimiento(IUnitOfWork uow, IAutomatismo automatismo, TransferenciaStockRequest request, AutomatismoConfirmacionMovimientoServiceContext context)
        {
            var response = this.CallWMSSendMovimiento(automatismo, request);


            if (response?.IsValid() ?? false)
            {
                var ejecucion = response.SuccessMessage;
                var loginName = context.GetUsuario().Username;
                uow.AutomatismoEjecucionRepository.AddConfAutomatismoEntrada(long.Parse(ejecucion), loginName, request);
                uow.SaveChanges();
            }


            return response;
        }
    }
}
