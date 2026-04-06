using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Impresiones;
using WIS.Domain.Interfaces;
using WIS.Domain.Picking;
using WIS.Domain.Recepcion;
using WIS.Domain.Security;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Tracking;
using WIS.Domain.Tracking.Models;
using WIS.Exceptions;
using WIS.Extension;
using WIS.Security;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WIS.Domain.Services
{
    public class TrackingService : ITrackingService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ILogger<TrackingService> _logger;
        protected readonly IAPITrackingService _apiTrackingService;
        protected readonly IBarcodeService _barcodeService;
        protected readonly IIdentityService _identity;

        public bool _trackingHablitado;
        public Dictionary<string, string> _config;
        protected readonly TrackingMapper _trackingMapper;

        public TrackingService(IUnitOfWorkFactory uowFactory, ILogger<TrackingService> logger, IAPITrackingService apiTrackingService, IIdentityService identity, IBarcodeService barcodeService)
        {
            _logger = logger;
            _uowFactory = uowFactory;
            _trackingMapper = new TrackingMapper();
            _apiTrackingService = apiTrackingService;
            _config = _apiTrackingService.GetConfig();
            _trackingHablitado = _apiTrackingService.TrackingHabilitado();
            _identity = identity;
            _barcodeService = barcodeService;
        }

        #region SincronizarTrackingProcess

        public virtual void Sync()
        {
            this._logger.LogDebug($"Iniciando proceso");

            if (_trackingHablitado)
            {
                DateTime? fechaInicial = null;
                if (DateTime.TryParse(_config["fechaInicial"], _identity.GetFormatProvider(), DateTimeStyles.None, out DateTime parsedValue))
                    fechaInicial = parsedValue;

                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    uow.CreateTransactionNumber("Sincronizar Tracking");

                    ProcesoPredios(uow);
                    ProcesoAgentes(uow, fechaInicial);
                    ProcesoPedidos(uow, fechaInicial);
                    ProcesoCierrePedido(uow, fechaInicial);
                }
            }

            this._logger.LogDebug($"Proceso finalizado");
        }

        public virtual void ProcesoPredios(IUnitOfWork uow)
        {
            this._logger.LogDebug($"Inicio sincronización de predios");
            var predios = uow.PredioRepository.GetPrediosSinSincronizar();

            foreach (var predio in predios)
            {
                SincronizarPredio(predio);
                uow.PredioRepository.UpdatePredio(predio);
                uow.SaveChanges();

                _logger.LogDebug($"Predio {predio.Numero} sincronizado.");
            }
            _logger.LogDebug($"Sincronización de predios terminada.");

        }
        public virtual void ProcesoAgentes(IUnitOfWork uow, DateTime? fechaInicial)
        {
            this._logger.LogDebug($"Inicio sincronización de agentes");
            var agentes = uow.AgenteRepository.GetAgentesNoSincronizados(fechaInicial);

            foreach (var agente in agentes)
            {
                try
                {
                    SincronizarAgente(agente, true);
                    uow.AgenteRepository.UpdateAgente(agente);
                    uow.SaveChanges();

                    this._logger.LogDebug($"Agente sincronizado - CD_CLIENTE: {agente.CodigoInterno} - CD_EMPRESA: {agente.Empresa}");
                }
                catch (ValidationFailedException ex)
                {
                    this._logger.LogDebug(ex, $"Error al sincronizar Agente - CD_CLIENTE: {agente.CodigoInterno} - CD_EMPRESA: {agente.Empresa}");
                    this._logger.LogError(ex, $"Error al sincronizar Agente - CD_CLIENTE: {agente.CodigoInterno} - CD_EMPRESA: {agente.Empresa}");
                }
                catch (Exception ex)
                {
                    this._logger.LogDebug(ex, $"Error al sincronizar Agente - CD_CLIENTE: {agente.CodigoInterno} - CD_EMPRESA: {agente.Empresa}");
                    this._logger.LogError(ex, $"Error al sincronizar Agente - CD_CLIENTE: {agente.CodigoInterno} - CD_EMPRESA: {agente.Empresa}");
                }
            }
            this._logger.LogDebug($"Sincronización de agentes terminada.");
        }

        public virtual void ProcesoPedidos(IUnitOfWork uow, DateTime? fechaInicial)
        {
            this._logger.LogDebug($"Inicio sincronización de pedidos");
            var pedidos = uow.TrackingRepository.GetPedidosNoSincronizados(fechaInicial);

            foreach (var ped in pedidos)
            {
                try
                {
                    Pedido pedido = uow.PedidoRepository.GetPedido(ped.Empresa, ped.CodigoCliente, ped.Pedido);
                    Agente agente = uow.AgenteRepository.GetAgenteConRelaciones(ped.Empresa, ped.CodigoCliente);

                    SincronizarPedido(uow, pedido, agente, true);

                    pedido.Transaccion = uow.GetTransactionNumber();

                    uow.PedidoRepository.UpdatePedido(pedido);
                    uow.SaveChanges();

                    this._logger.LogDebug($"Pedido sincronizado - Pedido: {ped.Pedido} - Cliente: {ped.CodigoCliente} - Empresa: {ped.Empresa}");
                }
                catch (Exception ex)
                {
                    this._logger.LogDebug(ex, $"Error al sincronizar - Pedido: {ped.Pedido} - Cliente: {ped.CodigoCliente} - Empresa: {ped.Empresa}");
                    this._logger.LogError(ex, $"Error al sincronizar - Pedido: {ped.Pedido} - Cliente: {ped.CodigoCliente} - Empresa: {ped.Empresa}");
                }
            }
            this._logger.LogDebug($"Sincronización de pedidos terminada.");
        }

        public virtual void ProcesoCierrePedido(IUnitOfWork uow, DateTime? fechaInicial)
        {
            this._logger.LogDebug($"Inicio cierre de pedidos");
            var peds = uow.TrackingRepository.GetPedidosNoFinalizados(fechaInicial);

            foreach (var ped in peds)
            {
                try
                {
                    CerrarPedido(ped);
                    this._logger.LogDebug($"Pedido cerrado - Pedido: {ped.Pedido} - Cliente: {ped.CodigoCliente} - Empresa: {ped.Empresa}");
                }
                catch (Exception ex)
                {
                    this._logger.LogDebug(ex, $"Error al cerrar - Pedido: {ped.Pedido} - Cliente: {ped.CodigoCliente} - Empresa: {ped.Empresa}");
                    this._logger.LogError(ex, $"Error al cerrar - Pedido: {ped.Pedido} - Cliente: {ped.CodigoCliente} - Empresa: {ped.Empresa}");
                }
            }
            this._logger.LogDebug($"Finaliza cierre de pedidos.");
        }

        public virtual void CerrarPedido(PedidoNoFinalizado pedido)
        {
            var result = this._apiTrackingService.CerrarPedido(_trackingMapper.MapToRequest(pedido));
            if (result.HasError())
            {
                string error = result.Errors.FirstOrDefault().Mensaje; ;
                GuardarErrores(_uowFactory, $"SincronizarTrackingProcess CerrarPedido - NU_PEDIDO: {pedido.Pedido} - CD_CLIENTE: {pedido.CodigoCliente} - CD_EMPRESA: {pedido.Empresa}", error);
                throw new Exception(error);
            }
        }
        #endregion

        #region SincronizacionInicial
        public virtual void SincronizacionInicial(IUnitOfWork uow)
        {
            #region - Predios -

            _logger.LogDebug($"Inicio Predios");
            var predios = uow.PredioRepository.GetPrediosSinSincronizar();

            foreach (var predio in predios)
            {
                SincronizarPredio(predio);
                uow.PredioRepository.UpdatePredio(predio);
                uow.SaveChanges();
                _logger.LogDebug($"Predio {predio.Numero} sincronizado.");
            }
            _logger.LogDebug($"Fin Predios");

            #endregion

            #region - Agentes -

            _logger.LogDebug($"Inicio Agentes");
            var agentes = uow.AgenteRepository.GetAgentesNoSincronizados();

            foreach (var agente in agentes)
            {
                SincronizarAgente(agente, true);
                uow.AgenteRepository.UpdateAgente(agente);
                uow.SaveChanges();
                _logger.LogDebug($"Agente sincronizado - CD_CLIENTE: {agente.CodigoInterno} - CD_EMPRESA: {agente.Empresa}");
            }
            _logger.LogDebug($"Fin Agentes");

            #endregion

            #region - Tipos de Vehículos

            //Tipo de Vehiculos
            _logger.LogDebug($"Inicio Tipo de Vehiculos");
            var tipos = uow.TipoVehiculoRepository.GetTiposNoSincronizados();

            foreach (var tipo in tipos)
            {
                SincronizarTipoVehiculo(tipo, true);
                uow.TipoVehiculoRepository.Update(tipo);
                uow.SaveChanges();
                _logger.LogDebug($"Tipo de vehículo {tipo.Id} sincronizado.");
            }
            _logger.LogDebug($"Fin Tipo de Vehiculos");

            #endregion

            #region - Vehículos

            //Vehiculos
            _logger.LogDebug($"Inicio Vehiculos");
            var vehiculos = uow.VehiculoRepository.GetVehiculosNoSincronizados();

            foreach (var vehiculo in vehiculos)
            {
                _logger.LogDebug($"Agrego vehiculo {vehiculo.Id}");

                SincronizarVehiculo(uow, vehiculo, true);
                uow.VehiculoRepository.Update(vehiculo);
                uow.SaveChanges();
            }
            _logger.LogDebug($"Fin Vehiculos");
            #endregion

            #region - Rutas -

            //Rutas por defecto necesarias en tracking
            _logger.LogDebug($"Inicio Rutas");
            string zonaSG = "S/G";//Sin geolocalizar
            string zonaRP = "200";//Resto del pais

            var seqRuta = uow.RutaRepository.GetUltimaRuta() + 1;

            if (!uow.RutaRepository.AnyRutaZona(zonaSG))
            {
                Ruta ruta = new Ruta()
                {
                    Id = (short)seqRuta,
                    Descripcion = "Zona Sin Geolocalizar",
                    ControlaOrdenDeCarga = false,
                    Estado = EstadoRutaDeEntrega.Activo,
                    Zona = zonaSG
                };
                _logger.LogDebug($"Agrego ruta zona S/G");
                uow.RutaRepository.AddRuta(ruta);
                seqRuta++;
            }

            if (!uow.RutaRepository.AnyRutaZona(zonaRP))
            {
                Ruta ruta = new Ruta()
                {
                    Id = (short)seqRuta,
                    Descripcion = "Resto del país",
                    ControlaOrdenDeCarga = false,
                    Estado = EstadoRutaDeEntrega.Activo,
                    Zona = zonaRP
                };
                _logger.LogDebug($"Agrego ruta zona 200");
                uow.RutaRepository.AddRuta(ruta);
            }
            _logger.LogDebug($"Fin Rutas");
            #endregion
        }

        public virtual void SincronizacionInicialPedidos(IUnitOfWork uow, bool bloquear = false)
        {
            _logger.LogDebug($"Injicio SincronizacionInicialPedidos");

            var pedidosNoPlanificados = uow.TrackingRepository.GetPedidosNoPlanificados();

            foreach (var datos in pedidosNoPlanificados)
            {
                Pedido pedido = uow.PedidoRepository.GetPedido(datos.Empresa, datos.CodigoCliente, datos.Pedido);
                Agente agente = uow.AgenteRepository.GetAgenteConRelaciones(datos.Empresa, datos.CodigoCliente);

                SincronizarPedido(uow, pedido, agente, bloquear);

                pedido.Transaccion = uow.GetTransactionNumber();

                uow.PedidoRepository.UpdatePedido(pedido);
                uow.SaveChanges();
            }

            _logger.LogDebug($"Fin SincronizacionInicialPedidos");
        }
        #endregion

        #region SincronizarEgreso
        public virtual void SincronizarEgreso(IUnitOfWork uow, Camion camion, bool confirmarViaje)
        {
            if (camion.IsTrackingHabilitado)
            {
                //Flujo normal
                this._logger.LogDebug($"SincronizarTracking - Camión : {camion.Id}");

                var pedidosSinSincronizar = uow.TrackingRepository.GetPedidosSinSincronizar(camion, true);
                string puntoEntregaPredio = uow.PredioRepository.GetPuntoEntregaPredio(camion.Predio);

                if (pedidosSinSincronizar != null && pedidosSinSincronizar.Count > 0)
                {
                    this._logger.LogDebug($"Sincronizacion de pedidos sin sincronizar");
                    SincronizacionDePedidos(uow, pedidosSinSincronizar);
                    uow.SaveChanges();
                }

                SincronizarViaje(uow, camion, puntoEntregaPredio);
                uow.CamionRepository.UpdateCamion(camion);
                uow.SaveChanges();

                if (confirmarViaje)
                {
                    this._logger.LogDebug($"Confirmacion de viaje camión : {camion.Id}");
                    ConfirmarViaje(uow, camion, puntoEntregaPredio);
                    camion.ConfirmacionViajeRealizada = true;
                    uow.CamionRepository.UpdateCamion(camion);
                    uow.SaveChanges();
                }

                if (camion.IsSincronizacionRealizada)
                {
                    uow.CargaCamionRepository.MarcarCargasSincronizadas(camion.Id);
                    uow.SaveChanges();
                }
            }
            else
            {
                if (confirmarViaje)
                {
                    //Se verifica que existan pedidos totalmente planificados para cerrarlos en tracking.
                    this._logger.LogDebug($"Checkeo de pedidos planificados para sus cierre - Camión : {camion.Id}");
                    CerrarPedidosPlanificados(uow, camion);
                    uow.SaveChanges();
                }
            }
        }

        public virtual void SincronizarViaje(IUnitOfWork uow, Camion camion, string puntoEntregaPredio)
        {
            short parada = 1;
            camion.IsSincronizacionRealizada = false;

            var viajeTeorico = _trackingMapper.MapToViajeTeoricoRequest(camion, puntoEntregaPredio);
            var puntosEntregaCarga = uow.TrackingRepository.GetPuntosEntregaCarga(camion.Id);

            var puntos = puntosEntregaCarga.GroupBy(d => d.PuntoEntrega).Select(s => new { puntoEntrega = s.Key, nuPrioridad = s.Min(e => e.NuPrioridadCarga ?? 999999999999) }).ToList(); //Entiendo que el 999999999 no es lo mejor

            var contenedoresEnviados = new List<string>();
            foreach (var punto in puntos.OrderBy(d => d.nuPrioridad))
            {
                var detalle = GetViajeDetalle(viajeTeorico, punto.puntoEntrega, ref parada);
                MapearDetalle(uow, camion, detalle, punto.puntoEntrega, puntosEntregaCarga, contenedoresEnviados);
                viajeTeorico.Detalles.Add(detalle);
            }

            var result = this._apiTrackingService.CrearViaje(viajeTeorico);
            if (result.HasError())
            {
                GuardarErrores(_uowFactory, $"SincronizarViaje - CD_CAMION: {camion.Id}", result.Errors.FirstOrDefault().Mensaje);
                throw new Exception($"Tracking - {result.Errors.FirstOrDefault().Mensaje}");
            }
            else
                camion.IsSincronizacionRealizada = true;
        }

        public virtual void SincronizacionDePedidos(IUnitOfWork uow, List<PuntoEntregaCarga> pedidoSinSincronizar)
        {
            foreach (var ped in pedidoSinSincronizar)
            {
                Pedido pedido = uow.PedidoRepository.GetPedido(ped.Empresa, ped.Cliente, ped.Pedido);
                Agente agente = uow.AgenteRepository.GetAgenteConRelaciones(ped.Empresa, ped.Cliente);

                SincronizarPedido(uow, pedido, agente, true);

                pedido.Transaccion = uow.GetTransactionNumber();

                uow.PedidoRepository.UpdatePedido(pedido);
            }
        }

        public virtual ViajeDetalleTeoricoRequest GetViajeDetalle(ViajeTeoricoRequest viajeTeorico, string cdPuntoEntrega, ref short parada)
        {
            var detalle = viajeTeorico.Detalles.Where(d => d.CodigoPuntoDeEntrega == cdPuntoEntrega).FirstOrDefault();
            if (detalle == null)
            {
                detalle = new ViajeDetalleTeoricoRequest
                {
                    CodigoPuntoDeEntrega = cdPuntoEntrega,
                    NumeroParada = parada,
                };
                parada++;
            }
            return detalle;
        }
        public virtual void MapearDetalle(IUnitOfWork uow, Camion camion, ViajeDetalleTeoricoRequest detalle, string puntoEntrega, List<PuntoEntregaCarga> entregasCarga, List<string> contenedoresEnviados)
        {
            foreach (var entrega in entregasCarga.Where(c => c.PuntoEntrega == puntoEntrega))
            {
                Pedido pedido = uow.PedidoRepository.GetPedido(entrega.Empresa, entrega.Cliente, entrega.Pedido);
                Agente agente = uow.AgenteRepository.GetAgente(entrega.Empresa, entrega.Cliente);

                detalle.ReferenciaPedidos.Add(new ViajeDetalleReferenciaPedidoTeoricoRequest
                {
                    CodigoAgrupacion = pedido.ComparteContenedorEntrega ?? $"{agente.Tipo}-{agente.Codigo}",
                    Numero = pedido.Id,
                    CodigoEmpresa = pedido.Empresa,
                    CodigoAgente = agente.Codigo,
                    TipoAgente = agente.Tipo
                });

                var contenedores = new List<ContenedorEntrega>();

                if (camion.IsCerrado())
                    contenedores = uow.TrackingRepository.GetContenedoresExp(camion.Id, pedido.Empresa, pedido.Cliente, pedido.Id);
                else
                    contenedores = uow.TrackingRepository.GetContenedores(camion.Id, pedido.Empresa, pedido.Cliente, pedido.Id);

                foreach (var cont in contenedores)
                {
                    string nuObjeto = $"{cont.IdExterno}#{cont.TipoContenedor}#{cont.IdExternoTracking}";
                    if (!contenedoresEnviados.Contains(nuObjeto))
                    {
                        detalle.DetalleObjetos.Add(new ViajeDetalleObjetoTeoricoRequest
                        {
                            Numero = nuObjeto,
                            Descripcion = cont.Descripcion,
                            Tipo = _config["tpCont"] ?? cont.TpObjetoTracking,
                            CodigoBarras = cont.CodigoBarras,
                            Cantidad = 1,
                            Peso = cont.PesoTotal,
                            Volumen = cont.Volumen ?? 1,
                            Alto = cont.Alto,
                            Largo = cont.Largo,
                            Profundidad = cont.Produndidad,
                            TipoContenedor = cont.TipoContenedor,
                        });

                        contenedoresEnviados.Add(nuObjeto);
                    }
                }
            }
        }

        public virtual void ConfirmarViaje(IUnitOfWork uow, Camion camion, string puntoEntregaPredio)
        {
            var puntosEntregaCarga = uow.TrackingRepository.GetPuntosEntregaCarga(camion.Id);
            var viajeReal = _trackingMapper.MapToViajeRealRequest(camion, puntoEntregaPredio);

            foreach (var entrega in puntosEntregaCarga.OrderBy(d => d.PuntoEntrega))
            {
                Pedido pedido = uow.PedidoRepository.GetPedido(entrega.Empresa, entrega.Cliente, entrega.Pedido);
                Agente agente = uow.AgenteRepository.GetAgente(entrega.Empresa, entrega.Cliente);

                viajeReal.ReferenciaPedidos.Add(new ViajeReferenciaPedidoRealRequest
                {
                    CodigoAgrupacion = pedido.ComparteContenedorEntrega ?? $"{agente.Tipo}-{agente.Codigo}",
                    Numero = pedido.Id,
                    CodigoEmpresa = pedido.Empresa,
                    CodigoAgente = agente.Codigo,
                    TipoAgente = agente.Tipo,
                    TodoPlanificado = uow.PedidoRepository.AnyPedidoTodoPlanificado(pedido.Id, pedido.Empresa, pedido.Cliente)
                });
            }

            var result = this._apiTrackingService.ConfirmarViaje(viajeReal);
            if (result.HasError())
            {
                GuardarErrores(_uowFactory, $"ConfirmarViaje - CD_CAMION: {camion.Id}", result.Errors.FirstOrDefault().Mensaje);
                throw new Exception($"Tracking - {result.Errors.FirstOrDefault().Mensaje}");
            }

        }
        public virtual void CerrarPedidosPlanificados(IUnitOfWork uow, Camion camion)
        {
            var pedidosPlanificados = uow.TrackingRepository.GetPedidosPlanificadosCamion(camion.Id);
            foreach (var p in pedidosPlanificados)
            {
                var result = this._apiTrackingService.CerrarPedido(_trackingMapper.MapToRequest(p));
                if (result.HasError())
                    throw new Exception($"Tracking - {result.Errors.FirstOrDefault().Mensaje}");
            }
        }

        public virtual List<ValidacionCamionResultado> ValidarSincronizacion(IUnitOfWork uow, Camion camion)
        {
            List<ValidacionCamionResultado> resultadoValidacion = null;

            //ValidarPedidosNoManejanTracking
            string msg = "Pedidos que no manejan tracking:";
            var keyPedidosConProblemas = uow.TrackingRepository.GetPedidoNoManejaTracking(camion);

            var result = new ValidacionCamionResultado(msg, keyPedidosConProblemas);
            if (result != null && result.Datos.Any())
            {
                if (resultadoValidacion == null)
                    resultadoValidacion = new List<ValidacionCamionResultado>();
                resultadoValidacion.Add(result);
            }
            //Posibilidad de agregar más validaciones si se requiere.

            return resultadoValidacion;
        }
        #endregion

        #region SincronizarPlanificacion
        public virtual void SincronizarPlanificacion(IUnitOfWork uow, Camion camion)
        {
            camion.IsSincronizacionRealizada = false;
            this._logger.LogDebug($"Sincronizar Planificación - Camión : {camion.Id}");

            if (camion.IsTrackingHabilitado)
            {
                var pedidosSinSincronizar = uow.TrackingRepository.GetPedidosSinSincronizar(camion, true);

                if (pedidosSinSincronizar != null && pedidosSinSincronizar.Count > 0)
                {
                    this._logger.LogDebug($"Sincronizacion de pedidos sin sincronizar");
                    SincronizacionDePedidos(uow, pedidosSinSincronizar);
                    uow.SaveChanges();
                }

                var planificacion = MapearPlanificacion(uow, camion);
                if (planificacion != null && planificacion.Count > 0)
                {
                    var result = this._apiTrackingService.EnviarTareaConAcciones(planificacion);
                    if (result.HasError())
                    {
                        GuardarErrores(_uowFactory, $"SincronizarPlanificacion - CD_CAMION: {camion.Id}", result.Errors.FirstOrDefault().Mensaje);
                        throw new Exception($"Tracking - {result.Errors.FirstOrDefault().Mensaje}");
                    }
                    else
                    {
                        camion.IsSincronizacionRealizada = true;
                        uow.CamionRepository.UpdateCamion(camion);
                        uow.SaveChanges();
                    }

                    if (camion.IsSincronizacionRealizada)
                    {
                        uow.CargaCamionRepository.MarcarCargasSincronizadas(camion.Id);
                        uow.SaveChanges();
                    }
                }
            }
        }
        public virtual List<PlanificacionRequest> MapearPlanificacion(IUnitOfWork uow, Camion camion)
        {
            var result = new List<PlanificacionRequest>();

            string puntoEntregaPredio = uow.PredioRepository.GetPuntoEntregaPredio(camion.Predio);
            var cargas = uow.TrackingRepository.GetContenedoresPlanificacion(camion.Id);

            var puntosEntrega = cargas.GroupBy(d => new { d.PuntoEntrega, d.ComparteContenedorEntrega })
                .Select(s => new { PuntoEntrega = s.Key.PuntoEntrega, ComparteContenedorEntrega = s.Key.ComparteContenedorEntrega, OrdenDeCarga = s.Min(e => e.OrdenDeCarga) });

            foreach (var punto in puntosEntrega.OrderBy(d => d.OrdenDeCarga))
            {
                var planificacion = new PlanificacionRequest()
                {
                    CodigoPuntoDeEntrega = punto.PuntoEntrega,
                    CodigoPuntoEntregaDeposito = puntoEntregaPredio,
                    CodigoAgrupacionTarea = punto.ComparteContenedorEntrega,
                    Descripcion = "Entrega",
                    SistemaCreacion = "WMS WIS"
                };

                var contenedores = cargas.Where(c => c.PuntoEntrega == punto.PuntoEntrega && c.ComparteContenedorEntrega == punto.ComparteContenedorEntrega).ToList();
                foreach (var contenedor in contenedores)
                {
                    var cantidadBulto = 1;
                    string cdBarras = null;
                    string tpBulto = _config["tpContFicticio"];
                    string nuObjeto = contenedor.Numero.ToString();

                    if (contenedor.TipoBulto == TipoBultoContenedor.Real)
                    {
                        nuObjeto = $"{contenedor.IdExterno}#{contenedor.TipoContenedor}#{contenedor.IdExternoTracking}";
                        tpBulto = _config["tpCont"];
                        cdBarras = contenedor.CodigoBarras;
                    }

                    var entrega = new EntregaRequest()
                    {
                        CodigoEmpresa = contenedor.Empresa,
                        CodigoCliente = contenedor.CodigoAgente,
                        TipoCliente = contenedor.TipoAgente,
                        Numero = nuObjeto,
                        Descripcion = contenedor.DescContenedor,
                        Tipo = tpBulto,
                        CodigoBarras = cdBarras,
                        Cantidad = cantidadBulto,
                        Volumen = contenedor.VolumenTotal,
                        Peso = contenedor.PesoTotal,
                        Alto = contenedor.Alto,
                        Largo = contenedor.Largo,
                        Profundidad = contenedor.Produndidad,
                        TipoContenedor = contenedor.TipoContenedor,
                    };

                    if (planificacion.Entregas == null)
                        planificacion.Entregas = new List<EntregaRequest>();

                    planificacion.Entregas.Add(entrega);
                }
                result.Add(planificacion);
            }

            return result;
        }
        #endregion

        #region SincronizarDevolucion
        public virtual void SincronizarDevolucion(IUnitOfWork uow, Agenda agenda, string puntoDeEntrega)
        {
            agenda.SincronizacionRealizada = false;
            this._logger.LogDebug($"Sincronizar Devolución - Agenda: {agenda.Id}");

            var planificacion = MapearDevolucion(uow, agenda, puntoDeEntrega);
            if (planificacion != null && planificacion.Count > 0)
            {
                var result = this._apiTrackingService.EnviarTareaConAcciones(planificacion);
                if (!result.HasError())
                    agenda.SincronizacionRealizada = true;
                else
                {
                    GuardarErrores(_uowFactory, $"Sincronizar devolución - NU_AGENDA: {agenda.Id}", result.Errors.FirstOrDefault().Mensaje);
                    throw new Exception($"Tracking - {result.Errors.FirstOrDefault().Mensaje}");
                }
            }
        }

        public virtual List<PlanificacionRequest> MapearDevolucion(IUnitOfWork uow, Agenda agenda, string puntoDeEntrega)
        {
            var result = new List<PlanificacionRequest>();

            string puntoEntregaPredio = uow.PredioRepository.GetPuntoEntregaPredio(agenda.Predio);
            var datos = uow.TrackingRepository.GetDevolucionANotificar(agenda.Id);

            var planificacion = new PlanificacionRequest()
            {
                CodigoPuntoDeEntrega = puntoDeEntrega,
                CodigoPuntoEntregaDeposito = puntoEntregaPredio,
                CodigoAgrupacionTarea = $"{datos.TipoAgente}-{datos.CodigoAgente}",
                Descripcion = "Devolución",
                Telefono = datos.Telefono,
                Prometida = datos.FechaPrometida.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                SistemaCreacion = "WMS WIS"
            };

            var cantidadBulto = 1;
            string cdBarras = null;
            string tpBulto = _config["tpContFicticio"];

            if (datos.TipoBulto == TipoBultoContenedor.Real)
            {
                tpBulto = _config["tpCont"];
                cdBarras = _barcodeService.GenerateBarcode(datos.Numero.ToString(), datos.TipoContenedor);
            }

            var devolucion = new DevolucionRequest()
            {
                CodigoEmpresa = datos.Empresa,
                CodigoCliente = datos.CodigoAgente,
                TipoCliente = datos.TipoAgente,
                Numero = datos.Numero.ToString(),
                Descripcion = _trackingMapper.NullIfEmpty(datos.DescripcionContenedor) ?? datos.Agenda.ToString(),
                Tipo = tpBulto,
                CodigoBarras = cdBarras,
                Cantidad = cantidadBulto,
                Volumen = datos.VolumenTotal,
                Peso = datos.PesoTotal,
                Alto = datos.Alto,
                Largo = datos.Largo,
                Profundidad = datos.Produndidad,
                TipoReferencia = datos.TipoReferencia,
                CodigoReferencia = _trackingMapper.NullIfEmpty(datos.CodigoReferencia) ?? datos.Agenda.ToString(),

            };

            var detalles = uow.TrackingRepository.GetDetallesDevolucion(agenda.Id);

            devolucion.DetallesDevolucion = new List<DetalleDevolucionRequest>();

            int cdExterno = 1;
            foreach (var d in detalles)
            {
                var detalle = new DetalleDevolucionRequest()
                {
                    CodigoExterno = cdExterno.ToString(),
                    TipoLinea = d.TipoLinea,
                    CodigoBarras = d.CodigoBarras,
                    Descripcion = d.DescipcionProducto,
                    Lote = d.Identificador,
                    Cantidad = d.CantidadAgendada ?? 0,
                    Fecha = d.FechaVencimiento,
                    Dato1 = d.Anexo1,
                    Dato2 = d.Anexo2,
                    Dato3 = d.Anexo3,
                    Dato4 = d.Anexo4
                };

                devolucion.DetallesDevolucion.Add(detalle);
                cdExterno++;
            }

            planificacion.Devoluciones.Add(devolucion);
            result.Add(planificacion);

            return result;
        }
        #endregion

        #region Mesa de Empaque

        public virtual void RegularizarEgresosMesaEmpaque(IUnitOfWork uow, EgresoContenedorTracking datosOrigen, EgresoContenedorTracking datosDestino)
        {
            if (_trackingHablitado)
            {
                if (datosOrigen.Egreso != null && datosOrigen.Egreso.IsTrackingHabilitado)
                {
                    CambiarEstadoSincronizacion(uow, datosOrigen.Egreso, false);

                    var planificacion = datosOrigen.Egreso.TipoArmadoEgreso == TipoArmadoEgreso.Planificacion;
                    if (planificacion)
                    {
                        var datosObjeto = uow.TrackingRepository.GetPlanificacionCamion(datosOrigen.Contenedor.Numero);
                        if (datosObjeto == null || datosOrigen.Baja)
                        {
                            datosObjeto = new PlanificacionCamion()
                            {
                                TipoBulto = TipoBultoContenedor.Real,
                                Numero = datosOrigen.Contenedor.Numero,
                                IdExterno = datosOrigen.Contenedor.IdExterno,
                                CodigoBarras = datosOrigen.Contenedor.CodigoBarras,
                                TipoContenedor = datosOrigen.Contenedor.TipoContenedor,
                                DescContenedor = datosOrigen.Contenedor.Numero.ToString(),
                                IdExternoTracking = datosOrigen.Contenedor.IdExternoTracking,
                            };
                        }

                        var request = _trackingMapper.MapToRequest(datosObjeto, planificacion, datosOrigen.Baja, _config);
                        var result = this._apiTrackingService.ModificarObjeto(request, out ModificarObjetosResponse response);

                        if (result.HasError())
                            GuardarErrores(_uowFactory, $"Error Modificar objeto", result.Errors.FirstOrDefault().Mensaje);
                    }
                }

                if (datosDestino.Egreso != null && datosDestino.Egreso.IsTrackingHabilitado && datosOrigen.Egreso?.Id != datosDestino.Egreso.Id)
                {
                    //Marco egreso como desincronizado por que no puedo agregar uno nuevo
                    CambiarEstadoSincronizacion(uow, datosDestino.Egreso, false);
                }
            }
        }

        #endregion

        #region Metodos generales

        public virtual void RegularizarBultosAnulados(IUnitOfWork uow, List<long> cargas)
        {
            var request = new ModificarObjetosRequest();

            foreach (var carga in cargas)
            {
                var eliminar = false;
                var datosObjeto = uow.TrackingRepository.GetPlanificacionCamion(carga);
                if (datosObjeto == null)
                {
                    eliminar = true;
                    datosObjeto = new PlanificacionCamion()
                    {
                        Numero = carga,
                        DescContenedor = carga.ToString(),
                        TipoBulto = TipoBultoContenedor.Ficticio,
                        TipoContenedor = BarcodeDb.TIPO_CONTENEDOR_W,
                    };
                }
                request = _trackingMapper.MapToRequest(datosObjeto, true, eliminar, _config, request);
            }

            var result = this._apiTrackingService.ModificarObjeto(request, out ModificarObjetosResponse response);

            if (result.HasError())
                GuardarErrores(_uowFactory, $"Error Modificar objeto", result.Errors.FirstOrDefault().Mensaje);


        }
        public virtual void SincronizarPredio(Predio predio)
        {
            string error = string.Empty;
            predio.SincronizacionRealizada = false;
            var result = this._apiTrackingService.AddPuntoEntrega(_trackingMapper.MapToRequest(predio, _config["agrupacionCD"]), out string cdPuntoEntrega, out string cdZona);

            if (!result.HasError())
            {
                predio.PuntoEntrega = cdPuntoEntrega;
                predio.SincronizacionRealizada = true;
                predio.IdExterno = predio.PuntoEntrega;
                predio.Modificacion = DateTime.Now;
            }
            else
            {
                error = result.Errors.FirstOrDefault().Mensaje;
                GuardarErrores(_uowFactory, $"Sincronizar Predio {predio.Numero} - Dirección: {predio.Direccion}", error);
                throw new Exception($"Tracking - {error}");
            }
        }
        public virtual void SincronizarAgente(Agente agente, bool bloquear)
        {
            string error = string.Empty;
            agente.SincronizacionRealizada = false;
            if (_trackingHablitado)
            {
                var result = this._apiTrackingService.AddAgente(_trackingMapper.MapToRequest(agente));

                if (!result.HasError() || result.Code == HttpStatusCode.Conflict.ToString())
                    agente.SincronizacionRealizada = true;
                else
                {
                    error = result.Errors.FirstOrDefault().Mensaje;
                    GuardarErrores(_uowFactory, $"Sincronizar agente - CD_CLIENTE: {agente.CodigoInterno} - CD_EMPRESA: {agente.Empresa}", error);

                    if (bloquear)
                        throw new ValidationFailedException($"Tracking - {error}");
                }
            }
        }
        public virtual void SincronizarUsuario(Usuario usuario, bool bloquear)
        {
            string error = string.Empty;
            usuario.SincronizacionRealizada = false;
            if (_trackingHablitado)
            {
                var user = _trackingMapper.MapToRequest(usuario);
                var result = this._apiTrackingService.UpdateUser(user);

                if (!result.HasError())
                    usuario.SincronizacionRealizada = true;
                else if (result.Code == HttpStatusCode.NotFound.ToString())
                {
                    result = this._apiTrackingService.AddUser(user);
                    if (!result.HasError())
                        usuario.SincronizacionRealizada = true;
                    else
                        error = result.Errors.FirstOrDefault().Mensaje;
                }
                else
                    error = result.Errors.FirstOrDefault().Mensaje;

                if (!string.IsNullOrEmpty(error))
                {
                    GuardarErrores(_uowFactory, $"SincronizarUsuario - USERID: {usuario.UserId}", error);
                    if (bloquear)
                        throw new Exception($"Tracking - {error}");
                }
            }
        }
        public virtual void SincronizarVehiculo(IUnitOfWork uow, Vehiculo vehiculo, bool bloquear)
        {
            string error = string.Empty;
            vehiculo.SincronizacionRealizada = false;

            if (_trackingHablitado)
            {
                var vehiculoTracking = _trackingMapper.MapToRequest(vehiculo);
                vehiculoTracking.Estado = uow.DominioRepository.GetDominio(vehiculoTracking.Estado)?.Valor;

                var result = this._apiTrackingService.UpdateVehiculo(vehiculoTracking);

                if (!result.HasError())
                    vehiculo.SincronizacionRealizada = true;
                else if (result.Code == HttpStatusCode.NotFound.ToString())
                {
                    result = this._apiTrackingService.AddVehiculo(vehiculoTracking);

                    if (!result.HasError())
                        vehiculo.SincronizacionRealizada = true;
                    else if (result.Code == HttpStatusCode.BadRequest.ToString())
                    {
                        result = this._apiTrackingService.AddTipoVehiculo(_trackingMapper.MapToRequest(vehiculo.Caracteristicas));

                        if (!result.HasError())
                        {
                            vehiculo.Caracteristicas.SincronizacionRealizada = true;
                            result = this._apiTrackingService.AddVehiculo(vehiculoTracking);

                            if (!result.HasError())
                                vehiculo.SincronizacionRealizada = true;
                            else
                                error = result.Errors.FirstOrDefault().Mensaje;
                        }
                        else
                            error = result.Errors.FirstOrDefault().Mensaje;
                    }
                    else
                        error = result.Errors.FirstOrDefault().Mensaje;
                }
                else
                    error = result.Errors.FirstOrDefault().Mensaje;


                if (!string.IsNullOrEmpty(error))
                {
                    GuardarErrores(_uowFactory, $"ModificarVehículo - CD_VEICULO: {vehiculo.Id}", error);
                    if (bloquear)
                        throw new Exception($"Tracking - {error}");
                }
            }
        }
        public virtual void SincronizarTipoVehiculo(VehiculoEspecificacion tipoVehiculo, bool bloquear)
        {
            string error = string.Empty;
            tipoVehiculo.SincronizacionRealizada = false;
            if (_trackingHablitado)
            {
                var tpVehiculotracking = _trackingMapper.MapToRequest(tipoVehiculo);
                var result = this._apiTrackingService.UpdateTipoVehiculo(tpVehiculotracking);

                if (!result.HasError())
                    tipoVehiculo.SincronizacionRealizada = true;
                else if (result.Code == HttpStatusCode.NotFound.ToString())
                {
                    result = this._apiTrackingService.AddTipoVehiculo(tpVehiculotracking);

                    if (!result.HasError())
                        tipoVehiculo.SincronizacionRealizada = true;
                    else
                        error = result.Errors.FirstOrDefault().Mensaje;
                }
                else
                    error = result.Errors.FirstOrDefault().Mensaje;

                if (!string.IsNullOrEmpty(error))
                {
                    GuardarErrores(_uowFactory, $"ModificarTipoVehículo - CD_TIPO_VEICULO: {tipoVehiculo.Id}", error);

                    if (bloquear)
                        throw new Exception($"Tracking - {error}");
                }
            }
        }

        public virtual void CerrarPedido(IUnitOfWork uow, Pedido pedido, Agente agente, bool bloquear)
        {
            if (_trackingHablitado && uow.PedidoRepository.PuedoCerrarPedido(pedido) && pedido.ConfiguracionExpedicion.FlTracking)
            {
                var result = this._apiTrackingService.CerrarPedido(_trackingMapper.MapToRequest(pedido, agente));

                if (result.HasError())
                {
                    GuardarErrores(_uowFactory, $"EliminarPedidosPendientes CerrarPedido - NU_PEDIDO: {pedido.Id} - CD_CLIENTE: {pedido.Cliente} - CD_EMPRESA: {pedido.Empresa}", result.Errors.FirstOrDefault().Mensaje);

                    if (bloquear)
                        throw new Exception($"Tracking - {result.Errors.FirstOrDefault().Mensaje}");
                }
            }
        }

        public virtual void SincronizarPedido(IUnitOfWork uow, Pedido pedido, Agente agente, bool bloquear)
        {
            string error = string.Empty;
            pedido.IsSincronizacionRealizada = false;

            if (_trackingHablitado && pedido.ConfiguracionExpedicion.FlTracking)
            {
                string direccion = GetDireccionFinal(uow, pedido, agente);
                pedido.DireccionEntrega = direccion.Truncate(400);

                var result = this._apiTrackingService.AddPuntoEntrega(_trackingMapper.MapToRequest(pedido, agente, direccion), out string cdPuntoEntrega, out string cdZona);

                if (!result.HasError())
                {
                    string fechaPrometida = string.Empty;
                    if (pedido.FechaEntrega != null)
                        fechaPrometida = pedido.FechaEntrega?.ToString("yyyy-MM-ddTHH:mm:ssZ");
                    else if (pedido.FechaAlta != null && double.TryParse(_config["cantDiasTarea"], out double cantDias))
                        fechaPrometida = pedido.FechaAlta?.AddDays(cantDias).ToString("yyyy-MM-ddTHH:mm:ssZ");
                    else
                        fechaPrometida = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");

                    result = this._apiTrackingService.AddTarea(_trackingMapper.MapToRequest(pedido, agente, cdPuntoEntrega, fechaPrometida));

                    if (!result.HasError())
                    {
                        pedido.PuntoEntrega = cdPuntoEntrega;
                        pedido.Ruta = uow.RutaRepository.GetRutaByZona(cdZona, true)?.Id ?? pedido.Ruta;
                        pedido.IsSincronizacionRealizada = true;
                        pedido.FechaModificacion = DateTime.Now;
                    }
                    else
                        error = result.Errors.FirstOrDefault().Mensaje;
                }
                else
                    error = result.Errors.FirstOrDefault().Mensaje;
            }

            if (!string.IsNullOrEmpty(error))
            {
                GuardarErrores(_uowFactory, $"Sincronizar Pedido - Pedido: {pedido.Id} - Cliente: {pedido.Cliente} - Empresa: {pedido.Empresa}", error);

                if (bloquear)
                    throw new Exception($"Tracking - {error}");
            }
        }

        public virtual string GetDireccionFinal(IUnitOfWork uow, Pedido pedido, Agente agente)
        {
            string dirAux = string.Empty;

            if (!string.IsNullOrEmpty(pedido.DireccionEntrega))
                dirAux = pedido.DireccionEntrega;
            else if (!string.IsNullOrEmpty(agente.Direccion))
                dirAux = agente.Direccion;
            else
                return null;

            var dir = dirAux.Split(',');
            if (dir.Length > 1)
                return dirAux;
            else
            {
                var localidad = uow.PaisSubdivisionLocalidadRepository.GetLocalidad(agente.IdLocalidad ?? 0);
                if (localidad == null)
                    return dirAux;
                else
                    return $"{dirAux}, {localidad.Nombre}, {localidad.Subdivision.Nombre}, {localidad.Subdivision.Pais.Nombre}";
            }
        }
        public virtual bool TrackingHabilitado()
        {
            return _trackingHablitado;
        }
        public virtual Dictionary<string, string> GetConfig()
        {
            return _config;
        }
        public virtual bool TestearConexion(Dictionary<string, string> config)
        {
            return _apiTrackingService.GetAgenteTest(config);
        }
        public virtual void GuardarErrores(IUnitOfWorkFactory uowFactory, string dsReferencia, string error)
        {
            using var uowAux = uowFactory.GetUnitOfWork();
            uowAux.BeginTransaction();

            try
            {
                InterfazEjecucion interfaz = new InterfazEjecucion
                {
                    CdInterfazExterna = CInterfazExterna.Tracking,
                    Archivo = "TRACKING",
                    Situacion = SituacionDb.ArchivoRespaldado,
                    Comienzo = DateTime.Now,
                    FechaSituacion = DateTime.Now,
                    ErrorCarga = "S",
                    ErrorProcedimiento = "N",
                    Referencia = dsReferencia,
                    GrupoConsulta = "S/N"
                };

                uowAux.InterfazRepository.AddInterfazEjecucion(interfaz);

                InterfazError intError = new InterfazError
                {
                    Id = interfaz.Id,
                    NroError = 1,
                    Registro = 1,
                    Referencia = dsReferencia,
                    Descripcion = error
                };
                uowAux.InterfazRepository.AddInterfazEjecucionError(intError);

                uowAux.SaveChanges();
                uowAux.Commit();
            }
            catch (Exception ex)
            {
                uowAux.Rollback();
            }
        }
        public virtual void CambiarEstadoSincronizacion(IUnitOfWork uow, IEnumerable<Camion> camiones, bool sincronizado)
        {
            if (_trackingHablitado)
            {
                foreach (var camion in camiones.Where(c => c.IsTrackingHabilitado))
                {
                    camion.IsSincronizacionRealizada = sincronizado;
                    camion.FechaModificacion = DateTime.Now;

                    uow.CamionRepository.UpdateCamion(camion);
                }
            }
        }
        public virtual void CambiarEstadoSincronizacion(IUnitOfWork uow, Camion camion, bool sincronizado)
        {
            CambiarEstadoSincronizacion(uow, new List<Camion>() { camion }, sincronizado);
        }
        public virtual void CambiarEstadoSincronizacion(IUnitOfWork uow, int cdCamion, bool sincronizado)
        {
            var camion = uow.CamionRepository.GetCamion(cdCamion);
            CambiarEstadoSincronizacion(uow, new List<Camion>() { camion }, sincronizado);
        }
        #endregion
    }
}
