using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo.Dtos;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class LiberacionOndaSyncBackgroundProcess : ILiberacionOndaSyncBackgroundProcess
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IAutomatismoAutoStoreClientService _automatismoAutoStoreClientService;
        protected readonly ILogger<LiberacionOndaSyncBackgroundProcess> _logger;

        public LiberacionOndaSyncBackgroundProcess(IUnitOfWorkFactory uowFactory, IAutomatismoAutoStoreClientService automatismoAutoStoreClientService)
        {
            _automatismoAutoStoreClientService = automatismoAutoStoreClientService;
            _uowFactory = uowFactory;
        }

        public virtual void Sync()
        {
            using var uow = _uowFactory.GetUnitOfWork();

            if (_automatismoAutoStoreClientService.IsEnabled() && uow.PreparacionRepository.AnyPreparacionPendienteNotificarAutomatismo())
            {
                uow.CreateTransactionNumber("WIS.SincronizarAutomatismoProcess");

                var preparacionesPendientesNotificar = uow.PreparacionRepository.GetPreparacionesPendienteNotificarAutomatismo();

                var agentes = GetAgentes(uow, preparacionesPendientesNotificar);
                var pedidos = GetPedidos(uow, preparacionesPendientesNotificar);
                var zonasUbicacion = GetZonas(uow, preparacionesPendientesNotificar);

                var cabezales = new Dictionary<string, SalidaStockAutomatismoRequest>();

                foreach (var preparacionNotificar in preparacionesPendientesNotificar)
                {
                    foreach (var linea in preparacionNotificar.Lineas.Where(w => w.Estado == EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO))
                    {
                        var zona = zonasUbicacion.GetValueOrDefault(linea.Ubicacion);
                        var agente = agentes.GetValueOrDefault($"{linea.Cliente}.{linea.Empresa}");
                        var pedido = pedidos.GetValueOrDefault($"{linea.Pedido}.{linea.Cliente}.{linea.Empresa}");

                        linea.ComparteContenedorPicking = pedido.ComparteContenedorPicking;


                        var key = $"{linea.NumeroPreparacion}";

                        switch (linea.Agrupacion)
                        {
                            case Agrupacion.Pedido:
                                key += $"~{linea.Pedido}~{agente.Tipo}~{linea.Cliente}~{linea.ComparteContenedorPicking}";
                                break;
                            case Agrupacion.Cliente:
                                key += $"~{agente.Tipo}~{linea.Cliente}~{linea.ComparteContenedorPicking}";
                                break;
                            case Agrupacion.Ruta:
                                key += $"~{linea.Carga}~{linea.ComparteContenedorPicking}";
                                break;
                            case Agrupacion.Onda:
                                key += $"~{linea.ComparteContenedorPicking}";
                                break;
                        }

                        if (!cabezales.ContainsKey(key))
                        {
                            cabezales[key] = new SalidaStockAutomatismoRequest()
                            {
                                Empresa = linea.Empresa,
                                Preparacion = linea.NumeroPreparacion,
                                DsReferencia = $"Liberacion Onda Sync Automatismo {linea.Agrupacion}~{linea.NumeroPreparacion}",
                            };

                        }

                        var preparacion = GetDetallePreparacionNotificacion(cabezales[key], zona, linea);
                        AddDetallePreparacionNotificacion(cabezales[key], preparacionNotificar, linea, zona, agente, pedido, preparacion);
                    }
                }

                List<int> preparacionesNotificadas = new List<int>();

                foreach (var cabezal in cabezales)
                {
                    try
                    {
                        _automatismoAutoStoreClientService.SendSalida(cabezal.Value);

                        preparacionesNotificadas.Add(cabezal.Value.Preparacion);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "WIS.SincronizarAutomatismoProcess");
                    }
                }

                try
                {
                    foreach (var nuPreparacion in preparacionesNotificadas.Distinct())
                        uow.PreparacionRepository.UpdatePreparacionesNotificadas(nuPreparacion, uow.GetTransactionNumber());

                    uow.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "WIS.SincronizarAutomatismoProcess");
                }
            }
        }

        public virtual void AddDetallePreparacionNotificacion(SalidaStockAutomatismoRequest cabezal, Preparacion preparacionNotificar, DetallePreparacion linea, string zona, Agente agente, Pedido pedido, SalidaStockLineaAutomatismoRequest preparacion)
        {
            string agrupacion = "P";
            if (!string.IsNullOrEmpty(linea.Agrupacion))
                agrupacion = linea.Agrupacion;

            if (preparacion == null)
            {
                preparacion = new SalidaStockLineaAutomatismoRequest
                {
                    Zona = zona,
                    Pedido = linea.Pedido,
                    CodigoAgente = agente.Codigo,
                    TipoAgente = agente.Tipo,
                    DescripcionAgente = agente.Descripcion,
                    FechaEntrega = pedido.FechaEntrega,
                    Observacion = pedido.Memo1,
                    Prioridad = 0,
                    TipoSalida = "01",
                    Preparacion = preparacionNotificar.Id,
                    ValorLanzamiento = null,
                    Producto = linea.Producto,
                    Identificador = linea.Lote,
                    Carga = linea.Carga,
                    Agrupacion = linea.Agrupacion,
                    Cantidad = 0,
                    ComparteContenedorPicking = pedido.ComparteContenedorPicking,
                };

                cabezal.Detalles.Add(preparacion);
            }

            switch (agrupacion)
            {
                case Agrupacion.Cliente:
                    preparacion.Pedido = string.Empty;
                    preparacion.Carga = null;
                    break;
                case Agrupacion.Ruta:
                    preparacion.Pedido = string.Empty;
                    preparacion.CodigoAgente = string.Empty;
                    preparacion.TipoAgente = string.Empty;
                    preparacion.DescripcionAgente = string.Empty;
                    break;
                case Agrupacion.Onda:
                    preparacion.Pedido = string.Empty;
                    preparacion.CodigoAgente = string.Empty;
                    preparacion.TipoAgente = string.Empty;
                    preparacion.DescripcionAgente = string.Empty;
                    preparacion.Carga = null;
                    break;
            }

            preparacion.Cantidad += linea.Cantidad;
        }

        public virtual SalidaStockLineaAutomatismoRequest GetDetallePreparacionNotificacion(SalidaStockAutomatismoRequest cabezal, string zona, DetallePreparacion linea)
        {
            SalidaStockLineaAutomatismoRequest preparacion = null;
            string agrupacion = "P";
            if (!string.IsNullOrEmpty(linea.Agrupacion))
                agrupacion = linea.Agrupacion;

            var comparteContenerdorPicking = string.IsNullOrEmpty(linea.ComparteContenedorPicking) ? null : linea.ComparteContenedorPicking;

            switch (agrupacion)
            {
                case Agrupacion.Pedido:
                    preparacion = cabezal.Detalles.FirstOrDefault(w => w.Zona == zona
                        && w.Pedido == linea.Pedido
                        && w.CodigoAgente == linea.Cliente
                        && w.Producto == linea.Producto
                        && w.Identificador == linea.Lote);
                    break;
                case Agrupacion.Cliente:
                    preparacion = cabezal.Detalles.FirstOrDefault(w => w.Zona == zona
                        && w.CodigoAgente == linea.Cliente
                        && w.Producto == linea.Producto
                        && w.Identificador == linea.Lote
                        && w.ComparteContenedorPicking == comparteContenerdorPicking);
                    break;
                case Agrupacion.Ruta:
                    preparacion = cabezal.Detalles.FirstOrDefault(w => w.Zona == zona
                        && w.Carga == linea.Carga
                        && w.Producto == linea.Producto
                        && w.Identificador == linea.Lote
                        && w.ComparteContenedorPicking == comparteContenerdorPicking);
                    break;
                case Agrupacion.Onda:
                    preparacion = cabezal.Detalles.FirstOrDefault(w => w.Zona == zona
                        && w.Producto == linea.Producto
                        && w.Identificador == linea.Lote
                        && w.ComparteContenedorPicking == comparteContenerdorPicking);
                    break;
            }

            return preparacion;
        }

        public virtual Dictionary<string, Pedido> GetPedidos(IUnitOfWork uow, List<Preparacion> preparaciones)
        {
            var keys = preparaciones
                .SelectMany(p => p.Lineas.Where(w => w.Estado == EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO))
                .GroupBy(d => new { d.Pedido, d.Cliente, d.Empresa })
                .Select(d => new Pedido()
                {
                    Id = d.Key.Pedido,
                    Cliente = d.Key.Cliente,
                    Empresa = d.Key.Empresa
                });

            var pedidos = uow.PedidoRepository.GetPedidos(keys);

            return pedidos.ToDictionary(p => $"{p.Id}.{p.Cliente}.{p.Empresa}", p => p);
        }

        public virtual Dictionary<string, Agente> GetAgentes(IUnitOfWork uow, List<Preparacion> preparaciones)
        {
            var keys = preparaciones
                .SelectMany(p => p.Lineas.Where(w => w.Estado == EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO))
                .GroupBy(d => new { d.Cliente, d.Empresa })
                .Select(d => new Agente()
                {
                    CodigoInterno = d.Key.Cliente,
                    Empresa = d.Key.Empresa
                });

            var agentes = uow.AgenteRepository.GetAgentesById(keys);

            return agentes.ToDictionary(p => $"{p.CodigoInterno}.{p.Empresa}", p => p);
        }

        public virtual Dictionary<string, string> GetZonas(IUnitOfWork uow, List<Preparacion> preparaciones)
        {
            var keys = preparaciones
                .SelectMany(p => p.Lineas.Where(w => w.Estado == EstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO))
                .GroupBy(d => d.Ubicacion)
                .Select(d => new Ubicacion()
                {
                    Id = d.Key,
                });

            var zonas = uow.ZonaUbicacionRepository.GetZonasByUbicaciones(keys);

            return zonas.ToDictionary(p => $"{p.Ubicacion}", p => p.Id);
        }

        public virtual List<SalidaStockAutomatismoRequest> GetCabezalesRequest(List<Preparacion> preparacionesPendientesNotificar)
        {
            return preparacionesPendientesNotificar
                .GroupBy(x => x.Empresa)
                .Select(x => new SalidaStockAutomatismoRequest()
                {
                    Empresa = x.Key.Value,
                    DsReferencia = "Liberacion Onda Sync Automatismo",
                })
                .ToList();
        }
    }
}
