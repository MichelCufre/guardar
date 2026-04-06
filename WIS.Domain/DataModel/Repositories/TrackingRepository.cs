using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Expedicion;
using WIS.Domain.Tracking.Models;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class TrackingRepository
    {
        protected readonly WISDB _context;
        protected readonly int _userId;
        protected readonly string _cdAplicacion;
        protected readonly TrackingMapper _trackingMapper;

        public TrackingRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._userId = userId;
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._trackingMapper = new TrackingMapper();
        }

        public virtual int GetCountPedidosNoPlanificados()
        {
            return GetPedidosNoPlanificados().Count;
        }
        public virtual bool AnyPedidoSinsincronizar(Camion camion)
        {
            return _context.V_PUNTOS_ENTREGA_TRACKING.AsNoTracking().Any(c => c.CD_CAMION == camion.Id && c.PEDIDO_SINCRONIZADO != "S");
        }
        public virtual bool AnyPedidoNoManejaTracking(Camion camion)
        {
            return _context.V_PUNTOS_ENTREGA_TRACKING.AsNoTracking().Any(c => c.CD_CAMION == camion.Id && c.TP_EXP_MANEJA_TRACKING != "S");
        }
        public virtual List<PedidoNoPlanificado> GetPedidosNoPlanificados()
        {
            return this._context.V_PEDIDOS_NO_PLANIFICADOS.AsNoTracking()
                .Select(w => this._trackingMapper.MapPedidoNoPlanificado(w)).ToList();
        }
        public virtual List<string> GetPedidoNoManejaTracking(Camion camion)
        {
            var pedidosConProblemas = new List<string>();
            var pedidos = _context.V_PUNTOS_ENTREGA_TRACKING.AsNoTracking().Where(c => c.CD_CAMION == camion.Id && c.TP_EXP_MANEJA_TRACKING != "S").ToList();
            if (pedidos != null)
            {
                foreach (var pedido in pedidos)
                {
                    pedidosConProblemas.Add($"Pedido: {pedido.NU_PEDIDO} - Cliente:{pedido.CD_CLIENTE} - Empresa: {pedido.CD_EMPRESA}");
                }
            }
            return pedidosConProblemas;
        }
        public virtual List<PuntoDeEntregaCliente> GetPuntosEntregaCliente()
        {
            return _context.V_PUNTOS_ENTREGA_CLIENTE.AsNoTracking()
                .Select(x => _trackingMapper.Map(x)).ToList();
        }
        public virtual List<PuntoEntregaCarga> GetPuntosEntregaCarga(int cdCamion)
        {
            return _context.V_PUNTOS_ENTREGA_TRACKING.AsNoTracking()
                .Where(c => c.CD_CAMION == cdCamion)
                .Select(w => this._trackingMapper.MapPuntoEntregaCarga(w)).ToList();
        }
        public virtual List<PlanificacionCamion> GetContenedoresPlanificacion(int cdCamion)
        {
            return _context.V_PLANIFICACION_CAMION.AsNoTracking()
                .Where(c => c.CD_CAMION == cdCamion)
                .Select(w => this._trackingMapper.MapPlanificacion(w)).ToList();
        }
        public virtual List<PedidoPlanificadoCamion> GetPedidosPlanificadosCamion(int cdCamion)
        {
            return _context.V_PEDIDOS_PLANIFICADOS_CAMION.AsNoTracking()
                .Where(p => p.CD_CAMION == cdCamion && p.TP_EXP_MANEJA_TRACKING == "S" && p.PEDIDO_SINCRONIZADO == "S")
                .Select(p => _trackingMapper.MapToObject(p)).ToList();
        }
        public virtual List<PedidoNoFinalizado> GetPedidosNoFinalizados(DateTime? fechaInicial = null)
        {
            var query = _context.V_PEDIDOS_SIN_CERRAR.AsQueryable();

            if (fechaInicial != null)
                query = _context.V_PEDIDOS_SIN_CERRAR.Where(c => c.DT_ADDROW >= fechaInicial);

            return query.ToList().Select(w => this._trackingMapper.MapPedidoNoFinalizado(w)).ToList();
        }
        public virtual List<PuntoDeEntregaCliente> GetPuntosEntregaCliente(string cliente, int empresa)
        {
            return _context.V_PUNTOS_ENTREGA_CLIENTE.AsNoTracking()
                .Where(x => x.CD_CLIENTE == cliente && x.CD_EMPRESA == empresa)
                .Select(x => _trackingMapper.Map(x)).ToList();
        }
        public virtual List<PedidoNoPlanificado> GetPedidosNoSincronizados(DateTime? fechaInicial = null)
        {
            var query = _context.V_PEDIDOS_NO_PLANIFICADOS_JOB.AsQueryable();

            if (fechaInicial != null)
                query = _context.V_PEDIDOS_NO_PLANIFICADOS_JOB.Where(c => c.DT_ADDROW >= fechaInicial);

            return query.ToList().Select(w => this._trackingMapper.MapPedidoNoPlanificado(w)).ToList();
        }
        public virtual List<PuntoEntregaCarga> GetPedidosSinSincronizar(Camion camion, bool filtroManejaTracking)
        {
            if (filtroManejaTracking)
            {
                return _context.V_PUNTOS_ENTREGA_TRACKING.AsNoTracking()
                    .Where(c => c.CD_CAMION == camion.Id && c.PEDIDO_SINCRONIZADO != "S" && c.TP_EXP_MANEJA_TRACKING == "S")
                    .Select(w => this._trackingMapper.MapPuntoEntregaCarga(w)).ToList();
            }
            else
            {
                return _context.V_PUNTOS_ENTREGA_TRACKING.AsNoTracking()
                    .Where(c => c.CD_CAMION == camion.Id && c.PEDIDO_SINCRONIZADO != "S")
                    .Select(w => this._trackingMapper.MapPuntoEntregaCarga(w)).ToList();
            }
        }
        public virtual List<ContenedorEntrega> GetContenedores(int cdCamion, int empresa, string cliente, string pedido)
        {
            return _context.V_CONTENEDORES_ENTREGA.AsNoTracking()
                .Where(c => c.CD_CAMION == cdCamion && c.CD_EMPRESA == empresa && c.CD_CLIENTE == cliente && c.NU_PEDIDO == pedido)
                .Select(w => this._trackingMapper.MapContenedorEntregaCarga(w)).ToList();
        }
        public virtual List<ContenedorEntrega> GetContenedoresExp(int cdCamion, int empresa, string cliente, string pedido)
        {
            return _context.V_CONTENEDORES_ENTREGA_EXP.AsNoTracking()
                .Where(c => c.CD_CAMION == cdCamion && c.CD_EMPRESA == empresa && c.CD_CLIENTE == cliente && c.NU_PEDIDO == pedido)
                .Select(w => this._trackingMapper.MapContenedorEntregaExpCarga(w)).ToList();
        }

        public virtual PlanificacionDevolucion GetDevolucionANotificar(int nuAgenda)
        {
            return _context.V_PLANIFICACION_DEVOLUCION.AsNoTracking()
                .Where(d => d.NU_AGENDA == nuAgenda).Select(d => _trackingMapper.Map(d)).FirstOrDefault();
        }
        public virtual List<PlanificacionDevolucionDetalle> GetDetallesDevolucion(int nuAgenda)
        {
            return _context.V_PLANIFICACION_DEVOLUCION_DET.AsNoTracking()
                .Where(d => d.NU_AGENDA == nuAgenda).Select(d => _trackingMapper.Map(d)).ToList();
        }

        public virtual ContenedorEntrega GetContenedorEntrega(int nuContenedor)
        {
            return _context.V_CONTENEDORES_ENTREGA
                .AsNoTracking()
                .Where(c => c.NU_CONTENEDOR == nuContenedor)
                .Select(c => _trackingMapper.MapContenedorEntregaCarga(c))
                .FirstOrDefault();
        }
        public virtual PlanificacionCamion GetPlanificacionCamion(long nuObjeto)
        {
            return _context.V_PLANIFICACION_CAMION
                .AsNoTracking()
                .Where(c => c.NU_CONTENEDOR == nuObjeto)
                .Select(c => _trackingMapper.MapPlanificacion(c))
                .FirstOrDefault();
        }
    }
}
