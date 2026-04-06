using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Eventos;
using WIS.Domain.Expedicion;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.General.API;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Interfaces;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Domain.Tracking.Models;
using WIS.Persistence.Database;
using WIS.Persistence.General;
using CEstadoDetallePreparacion = WIS.Domain.DataModel.Mappers.Constants.EstadoDetallePreparacion;

namespace WIS.Domain.DataModel.Repositories
{
    public class PedidoRepository
    {
        protected int userId;
        protected WISDB _context;

        protected string _cdAplicacion;
        protected int _userId;
        protected PedidoMapper _mapper;
        protected readonly IDapper _dapper;
        protected CamionMapper _mapperCamion;
        protected LpnMapper _lpnMapper;
        protected AgenteRepository _agenteRepository;
        protected ParametroRepository _parametroRepository;
        protected TransaccionRepository _transaccionRepository;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        public PedidoRepository(WISDB _context, string application, int userId, IDapper dapper)
        {
            this._dapper = dapper;
            this._context = _context;
            this._cdAplicacion = application;
            this._userId = userId;
            this._mapper = new PedidoMapper();
            this._lpnMapper = new LpnMapper();
            this._mapperCamion = new CamionMapper();
            this._agenteRepository = new AgenteRepository(_context, application, userId, _dapper);
            this._transaccionRepository = new TransaccionRepository(_context, application, userId, _dapper);
            this._parametroRepository = new ParametroRepository(_context, application, userId, dapper);

        }

        #region Add

        public virtual void AddPedido(Pedido pedido)
        {
            if (string.IsNullOrEmpty(pedido.Id))
                pedido.Id = Convert.ToString(this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_NU_PEDI_MANUAL));

            pedido.Id = pedido.Id.ToUpper();

            if (string.IsNullOrEmpty(pedido.Actividad))
                pedido.Actividad = EstadoPedidoDb.Activo;

            T_PEDIDO_SAIDA entity = this._mapper.Map(pedido);

            this._context.T_PEDIDO_SAIDA.Add(entity);
        }

        public virtual void AddDetallePedido(DetallePedido detalle)
        {
            T_DET_PEDIDO_SAIDA entity = this._mapper.MapDetalle(detalle);

            this._context.T_DET_PEDIDO_SAIDA.Add(entity);
        }

        public virtual void AddDetallePedido(Pedido pedido, DetallePedido detalle)
        {
            T_DET_PEDIDO_SAIDA entity = _mapper.MapDetalle(pedido, detalle);
            _context.T_DET_PEDIDO_SAIDA.Add(entity);
        }

        public virtual void AddPedidoConDetalle(Pedido pedido)
        {
            AddPedido(pedido);
            foreach (var detalle in pedido.Lineas)
            {
                detalle.Transaccion = pedido.Transaccion;
                AddDetallePedido(detalle);
            }
        }

        public virtual void AddDetallePedidoExpedido(DetallePedidoExpedido detExp)
        {
            T_DET_PEDIDO_EXPEDIDO entity = this._mapper.MapToDetalleExpedicionEntity(detExp);
            this._context.T_DET_PEDIDO_EXPEDIDO.Add(entity);
        }

        public virtual void AddPedidoMostrador(TempPedidoMostrador detExp)
        {
            T_TEMP_PEDIDO_MOSTRADOR entity = this._mapper.MapToPedidoMostradorEntity(detExp);
            this._context.T_TEMP_PEDIDO_MOSTRADOR.Add(entity);
        }

        public virtual void AddPedidoAnulado(PedidoAnulado pedAnulado)
        {
            pedAnulado.Id = this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_LOG_PEDIDO_ANULADO);
            var entity = this._mapper.MapToEntity(pedAnulado);
            this._context.T_LOG_PEDIDO_ANULADO.Add(entity);
        }

        public virtual void AddDetallePedidoLpn(DetallePedidoLpn detPedidoLpn)
        {
            T_DET_PEDIDO_SAIDA_LPN entity = this._lpnMapper.MapToEntity(detPedidoLpn);

            this._context.T_DET_PEDIDO_SAIDA_LPN.Add(entity);
        }

        public virtual void AddPedidoAnuladoLpn(PedidoAnuladoLpn pedAnuladoLpn)
        {
            var entity = this._mapper.MapToEntity(pedAnuladoLpn);
            this._context.T_LOG_PEDIDO_ANULADO_LPN.Add(entity);
        }
        #endregion

        #region Update

        public virtual void UpdatePedido(Pedido pedido)
        {
            T_PEDIDO_SAIDA entity = this._mapper.Map(pedido);
            T_PEDIDO_SAIDA attachedEntity = _context.T_PEDIDO_SAIDA.Local
                .FirstOrDefault(x => x.NU_PEDIDO == entity.NU_PEDIDO
                    && x.CD_CLIENTE == entity.CD_CLIENTE
                    && x.CD_EMPRESA == entity.CD_EMPRESA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_PEDIDO_SAIDA.Attach(entity);
                _context.Entry<T_PEDIDO_SAIDA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateDetallePedido(DetallePedido detalle)
        {
            T_DET_PEDIDO_SAIDA entity = this._mapper.MapDetalle(detalle);
            T_DET_PEDIDO_SAIDA attachedEntity = _context.T_DET_PEDIDO_SAIDA.Local
                .FirstOrDefault(x => x.NU_PEDIDO == entity.NU_PEDIDO
                    && x.CD_CLIENTE == entity.CD_CLIENTE
                    && x.CD_EMPRESA == entity.CD_EMPRESA
                    && x.CD_PRODUTO == entity.CD_PRODUTO
                    && x.CD_FAIXA == entity.CD_FAIXA
                    && x.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                    && x.ID_ESPECIFICA_IDENTIFICADOR == entity.ID_ESPECIFICA_IDENTIFICADOR);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_DET_PEDIDO_SAIDA.Attach(entity);
                _context.Entry<T_DET_PEDIDO_SAIDA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdatePedidoFuncionarioResponsable(Pedido pedido)
        {
            T_PEDIDO_SAIDA entity = _context.T_PEDIDO_SAIDA.FirstOrDefault(f =>
              f.NU_PEDIDO == pedido.Id
              && f.CD_CLIENTE == pedido.Cliente
              && f.CD_EMPRESA == pedido.Empresa);

            entity.CD_FUN_RESP = pedido.FuncionarioResponsable;
            entity.DT_FUN_RESP = pedido.FechaFuncionarioResponsable;
        }

        public virtual void UpdateDetallePedidoExpedido(DetallePedidoExpedido obj)
        {
            T_DET_PEDIDO_EXPEDIDO entity = this._mapper.MapToDetalleExpedicionEntity(obj);
            T_DET_PEDIDO_EXPEDIDO attachedEntity = _context.T_DET_PEDIDO_EXPEDIDO.Local
                .FirstOrDefault(w => w.NU_PEDIDO == entity.NU_PEDIDO
                    && w.CD_CLIENTE == entity.CD_CLIENTE
                    && w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.CD_FAIXA == entity.CD_FAIXA
                    && w.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                    && w.CD_CAMION == entity.CD_CAMION
                    && w.ID_ESPECIFICA_IDENTIFICADOR == entity.ID_ESPECIFICA_IDENTIFICADOR);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DET_PEDIDO_EXPEDIDO.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdatePedidoMostrador(TempPedidoMostrador obj)
        {
            T_TEMP_PEDIDO_MOSTRADOR entity = this._mapper.MapToPedidoMostradorEntity(obj);
            T_TEMP_PEDIDO_MOSTRADOR attachedEntity = _context.T_TEMP_PEDIDO_MOSTRADOR.Local
                .FirstOrDefault(w => w.NU_PEDIDO == entity.NU_PEDIDO
                    && w.CD_CLIENTE == entity.CD_CLIENTE
                    && w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.NU_PREPARACION == entity.NU_PREPARACION
                    && w.NU_CONTENEDOR == entity.NU_CONTENEDOR
                    && w.NU_CARGA == entity.NU_CARGA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_TEMP_PEDIDO_MOSTRADOR.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateDetallePedidoLpn(DetallePedidoLpn detPedidoLpn)
        {
            T_DET_PEDIDO_SAIDA_LPN entity = this._lpnMapper.MapToEntity(detPedidoLpn);
            T_DET_PEDIDO_SAIDA_LPN attachedEntity = _context.T_DET_PEDIDO_SAIDA_LPN.Local
                .FirstOrDefault(x => x.NU_PEDIDO == entity.NU_PEDIDO
                    && x.CD_CLIENTE == entity.CD_CLIENTE
                    && x.CD_EMPRESA == entity.CD_EMPRESA
                    && x.CD_PRODUTO == entity.CD_PRODUTO
                    && x.CD_FAIXA == entity.CD_FAIXA
                    && x.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                    && x.ID_ESPECIFICA_IDENTIFICADOR == entity.ID_ESPECIFICA_IDENTIFICADOR
                    && x.TP_LPN_TIPO == entity.TP_LPN_TIPO
                    && x.ID_LPN_EXTERNO == entity.ID_LPN_EXTERNO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_DET_PEDIDO_SAIDA_LPN.Attach(entity);
                _context.Entry<T_DET_PEDIDO_SAIDA_LPN>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateNuPonderacionPedido(int cdEmpresa, string cdCliente, string nuPedido, int qtPonderacion)
        {
            T_PEDIDO_SAIDA entity = this._context.T_PEDIDO_SAIDA.AsNoTracking().FirstOrDefault(x => x.CD_EMPRESA == cdEmpresa && x.CD_CLIENTE == cdCliente && x.NU_PEDIDO == nuPedido);

            this._context.T_PEDIDO_SAIDA.Attach(entity);
            entity.QT_PONDERACION_PEDIDO = qtPonderacion;
            entity.DT_UPDROW = DateTime.Now;
            entity.NU_TRANSACCION = this._context.GetTransactionNumber();
            this._context.Entry(entity).State = EntityState.Modified;
        }

        #endregion

        #region Any
        public virtual bool AnyPedido(int empresa, string cliente, string pedido)
        {
            return this._context.T_PEDIDO_SAIDA
                .AsNoTracking()
                .Any(d => d.CD_EMPRESA == empresa && d.CD_CLIENTE == cliente && d.NU_PEDIDO.ToUpper() == pedido.ToUpper());
        }

        public virtual bool AnyLpnPedido(string nuPedido, string codCliente, int codEmpresa, string codProducto, decimal codFaixa, string nuIdentificador, string idEspecificaIDen)
        {
            return this._context.V_PRE152_DETALLE_PEDIDO_LPN
                .AsNoTracking()
                .Any(d => d.NU_PEDIDO == nuPedido
                    && d.CD_CLIENTE == codCliente
                    && d.CD_EMPRESA == codEmpresa
                    && d.CD_PRODUTO == codProducto
                    && d.CD_FAIXA == codFaixa
                    && d.NU_IDENTIFICADOR == nuIdentificador
                    && d.ID_ESPECIFICA_IDENTIFICADOR == idEspecificaIDen);
        }

        public virtual bool AnyDetalleAtributo(long numeroDetalle)
        {
            return this._context.V_PRE156_DETALLE_ATRIBUTOS_DE_DETALLE_PEDIDO
                .AsNoTracking()
                .Any(d => d.NU_DET_PED_SAI_ATRIB == numeroDetalle);
        }

        public virtual bool AnyDetalleAtributoDeDetallePedido(long numeroDetalle)
        {
            return this._context.V_PRE154_DETALLE_ATRIBUTOS_LPN_DETALLE_PEDIDO
                .AsNoTracking()
                .Any(d => d.NU_DET_PED_SAI_ATRIB == numeroDetalle);
        }

        public virtual bool AnyAtributoLpnDetallePedido(string numeroPedido, string idLpnExterno, string tipoLpn)
        {
            return this._context.V_PRE153_ATRIBUTO_LPN_DETALLE_PEDIDO
                .AsNoTracking()
                .Any(d => d.NU_PEDIDO == numeroPedido &&
                          d.ID_LPN_EXTERNO == idLpnExterno &&
                          d.TP_LPN_TIPO == tipoLpn);
        }

        public virtual bool AnyAtributoPedido(string nuPedido, string codCliente, int codEmpresa, string codProducto, decimal codFaixa, string nuIdentificador, string idEspecificaIDen)
        {
            return this._context.V_PRE155_ATRIBUTOS_DE_DETALLE_DE_PEDIDO
                .AsNoTracking()
                .Any(d => d.NU_PEDIDO == nuPedido
                    && d.CD_CLIENTE == codCliente
                    && d.CD_EMPRESA == codEmpresa
                    && d.CD_PRODUTO == codProducto
                    && d.CD_FAIXA == codFaixa
                    && d.NU_IDENTIFICADOR == nuIdentificador
                    && d.ID_ESPECIFICA_IDENTIFICADOR == idEspecificaIDen);
        }

        public virtual bool AnyPedidoProducto(int empresa, string cliente, string pedido, string producto)
        {
            return this._context.T_DET_PEDIDO_SAIDA
                .AsNoTracking()
                .Any(d => d.CD_EMPRESA == empresa && d.CD_CLIENTE == cliente && d.NU_PEDIDO == pedido && d.CD_PRODUTO == producto);
        }

        public virtual bool AnyPedidoIdentificador(int empresa, string cliente, string pedido, string producto, string identificador)
        {
            return this._context.T_DET_PEDIDO_SAIDA
                .AsNoTracking()
                .Any(d => d.CD_EMPRESA == empresa && d.CD_CLIENTE == cliente && d.NU_PEDIDO == pedido && d.CD_PRODUTO == producto && d.NU_IDENTIFICADOR == identificador);
        }

        public virtual bool LiberadoCompletamente(string nroPedido, string cdCliente, int cdEmpresa)
        {
            decimal cantLiberada;
            decimal cantPedido;
            decimal cantAnulada;

            var cantPicking = _context.T_DET_PEDIDO_SAIDA.Where(dps => dps.CD_CLIENTE == cdCliente && dps.NU_PEDIDO == nroPedido && dps.CD_EMPRESA == cdEmpresa).AsEnumerable()
                .GroupBy(dps => dps)
                .Select(dps => new
                {
                    QT_LIBERADO = dps.Sum(d => d.QT_LIBERADO ?? 0),
                    QT_PEDIDO = dps.Sum(d => d.QT_PEDIDO ?? 0),
                    QT_ANULADO = dps.Sum(d => d.QT_ANULADO ?? 0)
                }).ToList();

            cantLiberada = cantPicking.Sum(d => d.QT_LIBERADO);
            cantPedido = cantPicking.Sum(d => d.QT_PEDIDO);
            cantAnulada = cantPicking.Sum(d => d.QT_ANULADO);

            if (cantLiberada < cantPedido - cantAnulada)
                return false;
            var estadosAnulacion = EstadoDetallePreparacion.GetEstadosAnulacion();

            List<T_DET_PICKING> colDetPicking = _context.T_DET_PICKING
                .Where(dp => dp.CD_CLIENTE == cdCliente
                    && dp.NU_PEDIDO == nroPedido
                    && dp.CD_EMPRESA == cdEmpresa
                    && !estadosAnulacion.Contains(dp.ND_ESTADO))
                .ToList();

            decimal cantProductoPicking = 0;
            if (colDetPicking != null && colDetPicking.Count > 0)
            {
                cantProductoPicking = colDetPicking.Sum(s => (s.QT_PRODUTO ?? 0));
            }

            if (cantLiberada == cantProductoPicking)
                return true;

            return false;
        }

        public virtual bool TodoPickeado(string nroPedido, string cdCliente, int cdEmpresa)
        {
            var estados = CEstadoDetallePreparacion.GetEstadosAnulacion();
            estados.Add(CEstadoDetallePreparacion.ESTADO_PREPARADO);

            return !_context.T_DET_PICKING
                .AsNoTracking()
                .Any(dp => dp.CD_CLIENTE == cdCliente
                    && dp.NU_PEDIDO == nroPedido
                    && dp.CD_EMPRESA == cdEmpresa
                    && !estados.Contains(dp.ND_ESTADO));
        }

        public virtual bool TodoAsignadoCamion(string nroPedido, string cdCliente, int cdEmpresa, int cdCamion)
        {
            //Buscar lineas de picking para el pedido y seleccionar camion donde contenedor este en situación 601, 602 o null
            var listaCamiones = _context.V_TODO_ASIGNADO_CAMION
                .Where(dp => dp.NU_PEDIDO == nroPedido && dp.CD_CLIENTE == cdCliente && dp.CD_EMPRESA == cdEmpresa).Select(w => w.CD_CAMION).ToList();

            if (listaCamiones.Count() == 1 && listaCamiones.First() == cdCamion)
            {
                //Existe camión y es el que vino como parámetro
                return true;
            }

            return false;
        }

        public virtual bool TodoEmpaquetado(string nroPedido, string cdCliente, int cdEmpresa)
        {
            return !_context.T_DET_PICKING
                .Include("T_CONTENEDOR")
                .AsNoTracking()
                .Any(dpc => dpc.CD_CLIENTE == cdCliente
                    && dpc.NU_PEDIDO == nroPedido
                    && dpc.CD_EMPRESA == cdEmpresa
                    && (dpc.T_CONTENEDOR.ID_CONTENEDOR_EMPAQUE ?? "N") == "N");
        }

        public virtual bool TodoTienePrecinto(IUnitOfWork uow, string nroPedido, string cdCliente, int cdEmpresa, int cdCamion)
        {
            var controlParcial = uow.ParametroRepository.GetParameter("PRECINTO_CONTROL_PARCIAL");

            bool valid;

            if (controlParcial == "N")
            {
                valid = _context.V_CONTENEDOR_PRECINTO.Any(e => e.CD_CAMION == cdCamion
                    && e.CD_CLIENTE == cdCliente
                    && e.CD_EMPRESA == cdEmpresa
                    && e.NU_PEDIDO == nroPedido
                    && (string.IsNullOrEmpty(e.ID_PRECINTO_1) || string.IsNullOrEmpty(e.ID_PRECINTO_2)));
            }
            else
            {
                valid = _context.V_CONTENEDOR_PRECINTO.Any(e => e.CD_CAMION == cdCamion
                    && e.CD_CLIENTE == cdCliente
                    && e.CD_EMPRESA == cdEmpresa
                    && e.NU_PEDIDO == nroPedido
                    && string.IsNullOrEmpty(e.ID_PRECINTO_1)
                    && string.IsNullOrEmpty(e.ID_PRECINTO_2));
            }

            return !valid;
        }

        public virtual bool TodoFacturado(string nroPedido, string cdCliente, int cdEmpresa)
        {
            return _context.T_TEMP_PEDIDO_MOSTRADOR
                .AsNoTracking()
                .Where(x => x.NU_PEDIDO == nroPedido
                    && x.CD_CLIENTE == cdCliente
                    && x.CD_EMPRESA == cdEmpresa
                    && x.CD_CAMION_FACTURADO == null)
                .Count() == 0;
        }

        public virtual bool IsTipoPedidoCompatibleTipoExpedicion(string tipoPedido, string tipoExpedicion)
        {
            return this._context.V_REL_TP_EXPEDICION_TP_PEDIDO.Any(d => d.TP_PEDIDO == tipoPedido && d.TP_EXPEDICION == tipoExpedicion);
        }

        public virtual bool ExistePedidoMostrador(int nuPreparacion, int nuContenedor, string nuPedido, int codigoEmpresa, string codigoCliente)
        {
            return _context.T_TEMP_PEDIDO_MOSTRADOR.Any(x => x.NU_PEDIDO == nuPedido && x.NU_PREPARACION == nuPreparacion && x.CD_EMPRESA == codigoEmpresa && x.NU_CONTENEDOR == nuContenedor && x.CD_CLIENTE == codigoCliente);
        }

        public virtual bool AnyAnulacionPendiente(string cliente, string pedido, int empresa)
        {
            return this._context.T_ANULACIONES_PENDIENTES.Any(a => a.NU_PEDIDO == pedido && a.CD_CLIENTE == cliente && a.CD_EMPRESA == empresa && a.QT_PENDIENTE > 0);
        }

        public virtual bool AnyTipoExpedicion(string tipoExpedicion)
        {
            return this._context.T_TIPO_EXPEDICION.Any(f => f.TP_EXPEDICION == tipoExpedicion);
        }

        public virtual bool AnyTipoPedido(string tipoPedido)
        {
            return this._context.T_TIPO_PEDIDO.Any(f => f.TP_PEDIDO == tipoPedido);
        }

        public virtual bool PuedoCerrarPedido(Pedido pedido)
        {
            return (AnyPedidoTodoPlanificado(pedido.Id, pedido.Empresa, pedido.Cliente) && pedido.IsSincronizacionRealizada);
        }

        public virtual bool AnyPedidoTodoPlanificado(string pedido, int empresa, string cliente)
        {
            return this._context.V_TAREA_REFERENCIA_TRACKING.Any(d => d.NU_PEDIDO == pedido && d.CD_EMPRESA == empresa && d.CD_CLIENTE == cliente);
        }

        public virtual bool AnyDuplicado(string pedido, int empresa, string cliente, string producto, string identificador, string especificaIdenficador)
        {
            return this._context.T_DET_PEDIDO_SAIDA_DUP
                .AsNoTracking()
                .Any(d => d.NU_PEDIDO == pedido
                    && d.CD_EMPRESA == empresa
                    && d.CD_CLIENTE == cliente
                    && d.CD_PRODUTO == producto
                    && d.NU_IDENTIFICADOR == identificador
                    && d.ID_ESPECIFICA_IDENTIFICADOR == especificaIdenficador
                    && d.CD_FAIXA == 1);
        }

        public virtual bool AnyDetallesEspecificadoYNoEspecificado(string id, int empresa, string cliente, out string producto)
        {
            bool DetallesEspecificadoYNoEsprcificado = false;
            producto = "";

            var detallesConDosCriterios = _context.T_DET_PEDIDO_SAIDA.Where(x => x.NU_PEDIDO == id
                                            && x.CD_EMPRESA == empresa
                                            && x.CD_CLIENTE == cliente
                                            ).GroupBy(x => x.CD_PRODUTO).Select(x => new
                                            {
                                                CD_PRODUTO = x.Key,
                                                MIN_ESPECIFICA_IDENTIFICADOR = x.Min(x => x.ID_ESPECIFICA_IDENTIFICADOR),
                                                MAX_ESPECIFICA_IDENTIFICADOR = x.Max(x => x.ID_ESPECIFICA_IDENTIFICADOR)
                                            }).FirstOrDefault(x => x.MIN_ESPECIFICA_IDENTIFICADOR != x.MAX_ESPECIFICA_IDENTIFICADOR);
            if (detallesConDosCriterios != null)
            {
                producto = detallesConDosCriterios.CD_PRODUTO;
                DetallesEspecificadoYNoEsprcificado = true;
            }
            return DetallesEspecificadoYNoEsprcificado;
        }

        public virtual bool AnyPedidoFacturaEnEmpaque(int nuPreparacion, int nuContenedor)
        {
            return _context.T_DET_PICKING
                .Join(
                    _context.T_PEDIDO_SAIDA,
                    dp => new { dp.NU_PEDIDO, dp.CD_EMPRESA, dp.CD_CLIENTE },
                    ps => new { ps.NU_PEDIDO, ps.CD_EMPRESA, ps.CD_CLIENTE },
                    (dp, ps) => new { DetallePicking = dp, Pedido = ps }
                )
                .Join(
                    _context.T_TIPO_EXPEDICION,
                    j => j.Pedido.TP_EXPEDICION,
                    te => te.TP_EXPEDICION,
                    (j, te) => new { j.DetallePicking, TipoExpedicion = te }
                )
                .Any(x => x.DetallePicking.NU_PREPARACION == nuPreparacion &&
                          x.DetallePicking.NU_CONTENEDOR == nuContenedor &&
                          x.TipoExpedicion.FL_FACTURAR_EN_EMPAQUETADO == "S");
        }

        public virtual bool HayProblemasEnPedido(string nuPedido, int cdEmpresa, string cdCliente, int cdCamion)
        {
            var detPedidoSalida = _context.T_DET_PEDIDO_SAIDA.FirstOrDefault(a => a.NU_PEDIDO.Equals(nuPedido)
                                                                                && a.CD_CLIENTE.Equals(cdCliente)
                                                                                && a.CD_EMPRESA == cdEmpresa
                                                                                && a.QT_PEDIDO > 0);
            if (detPedidoSalida != null)
            {
                decimal? qtEnviada = GetCantidadEnviada(detPedidoSalida.NU_PEDIDO, detPedidoSalida.CD_CLIENTE, detPedidoSalida.CD_EMPRESA, detPedidoSalida.CD_PRODUTO, detPedidoSalida.CD_FAIXA, detPedidoSalida.NU_IDENTIFICADOR, detPedidoSalida.ID_ESPECIFICA_IDENTIFICADOR);
                decimal? qtPrparada = GetCantidadPreparada(cdCamion, detPedidoSalida.NU_PEDIDO, detPedidoSalida.CD_CLIENTE, detPedidoSalida.CD_EMPRESA, detPedidoSalida.CD_PRODUTO, detPedidoSalida.CD_FAIXA, detPedidoSalida.NU_IDENTIFICADOR, detPedidoSalida.ID_ESPECIFICA_IDENTIFICADOR);

                if (detPedidoSalida.QT_PEDIDO != (qtEnviada + qtPrparada))
                    return true;

            }
            return false;
        }

        #endregion

        #region Get
        public virtual List<Pedido> GetPedidosByPreparacionAndContenedor(int nuPreparacion, int nuContenedor)
        {
            return _context.T_DET_PICKING
                .Join(
                   _context.T_PEDIDO_SAIDA,
                   dp => new { dp.NU_PEDIDO, dp.CD_EMPRESA, dp.CD_CLIENTE },
                   ps => new { ps.NU_PEDIDO, ps.CD_EMPRESA, ps.CD_CLIENTE },
                   (dp, ps) => new { Picking = dp, Pedido = ps }
                )
                .AsNoTracking()
                .Where(dp => dp.Picking.NU_PREPARACION == nuPreparacion && dp.Picking.NU_CONTENEDOR == nuContenedor)
                .Select(s => s.Pedido)
                .Distinct()
                .Select(w => _mapper.Map(w))
                .ToList();
        }

        public virtual Pedido GetPedido(int empresa, string cliente, string pedidoId)
        {
            var entity = this._context.T_PEDIDO_SAIDA
                .Include("T_DET_PEDIDO_SAIDA")
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_EMPRESA == empresa
                    && d.CD_CLIENTE == cliente
                    && d.NU_PEDIDO == pedidoId);

            if (entity == null)
                return null;

            var tipoExpedicion = this._context.T_TIPO_EXPEDICION
                .AsNoTracking()
                .FirstOrDefault(d => d.TP_EXPEDICION == entity.TP_EXPEDICION);

            Pedido pedido = this._mapper.Map(entity);

            pedido.ConfiguracionExpedicion = this._mapper.MapConfiguracionExpedicion(tipoExpedicion);

            return pedido;
        }

        public virtual Pedido GetCabezalPedidoProduccion(int empresa, string cliente, string ingreso)
        {
            var entity = this._context.T_PEDIDO_SAIDA.Include("T_DET_PEDIDO_SAIDA").AsNoTracking().Where(d => d.CD_EMPRESA == empresa && d.CD_CLIENTE == cliente && d.NU_PRDC_INGRESO == ingreso).FirstOrDefault();

            if (entity == null)
                return null;

            Pedido pedido = this._mapper.Map(entity);

            return pedido;
        }

        public virtual Pedido GetConfiguracionExpedicionPedido(Pedido pedido)
        {
            var tipoExpedicion = this._context.T_TIPO_EXPEDICION.Where(d => d.TP_EXPEDICION == pedido.TipoExpedicionId).AsNoTracking().FirstOrDefault();

            pedido.ConfiguracionExpedicion = this._mapper.MapConfiguracionExpedicion(tipoExpedicion);

            return pedido;
        }

        public virtual DetallePedido GetDetallePedido(string pedido, int empresa, string cliente, string producto, string identidicador, decimal faixa, string espIdentificador)
        {
            T_DET_PEDIDO_SAIDA ped = _context.T_DET_PEDIDO_SAIDA
                .AsNoTracking()
                .FirstOrDefault(s => s.CD_CLIENTE == cliente
                    && s.CD_EMPRESA == empresa && s.NU_PEDIDO == pedido
                    && s.CD_PRODUTO == producto
                    && s.NU_IDENTIFICADOR == identidicador
                    && s.CD_FAIXA == faixa
                    && s.ID_ESPECIFICA_IDENTIFICADOR == espIdentificador);

            return _mapper.MapDetalle(ped);
        }

        public virtual List<DetallePedido> GetDetallesPedido(Pedido pedido)
        {
            return _context.T_DET_PEDIDO_SAIDA
                .AsNoTracking()
                .Where(w => w.NU_PEDIDO == pedido.Id
                    && w.CD_CLIENTE == pedido.Cliente
                    && w.CD_EMPRESA == pedido.Empresa)
                .ToList()
                .Select(w => _mapper.MapDetalle(w))
                .ToList();
        }

        public virtual List<Pedido> GetPedidosPreparacionProgramada(int nroPreparacion)
        {
            var entities = this._context.T_PEDIDO_SAIDA
                .Include("T_DET_PEDIDO_SAIDA")
                .Where(d => d.NU_PREPARACION_PROGRAMADA == nroPreparacion)
                .ToList();

            var pedidos = new List<Pedido>();

            foreach (var entity in entities)
            {
                var tipoExpedicion = this._context.T_TIPO_EXPEDICION
                    .Where(d => d.TP_EXPEDICION == entity.TP_EXPEDICION)
                    .AsNoTracking()
                    .FirstOrDefault();

                Pedido pedido = this._mapper.Map(entity);
                pedido.ConfiguracionExpedicion = this._mapper.MapConfiguracionExpedicion(tipoExpedicion);
                pedidos.Add(pedido);
            }

            return pedidos;
        }

        public virtual List<Pedido> GetPedidosConPendienteCrossDocking(int agenda, int preparacion)
        {
            var resultado = new List<Pedido>();

            var entities = (
                from ped in this._context.T_PEDIDO_SAIDA.AsNoTracking().Include("T_DET_PEDIDO_SAIDA").AsNoTracking()
                join detCros in this._context.T_DET_CROSS_DOCK.AsNoTracking() on new { ped.CD_EMPRESA, ped.CD_CLIENTE, ped.NU_PEDIDO } equals new { detCros.CD_EMPRESA, detCros.CD_CLIENTE, detCros.NU_PEDIDO }
                where detCros.NU_AGENDA == agenda && detCros.NU_PREPARACION == preparacion && (detCros.QT_PRODUTO > (detCros.QT_PREPARADO ?? 0))
                select ped
            ).ToList();

            var tempEntities = (
                from ped in this._context.T_PEDIDO_SAIDA.AsNoTracking().Include("T_DET_PEDIDO_SAIDA").AsNoTracking()
                join detCros in this._context.T_CROSS_DOCK_TEMP.AsNoTracking() on new { ped.CD_EMPRESA, ped.CD_CLIENTE, ped.NU_PEDIDO } equals new { detCros.CD_EMPRESA, detCros.CD_CLIENTE, detCros.NU_PEDIDO }
                where detCros.NU_AGENDA == agenda
                select ped
            ).ToList();

            entities.AddRange(tempEntities);

            foreach (var entity in entities)
            {
                if (resultado.Any(w => w.Id == entity.NU_PEDIDO && w.Cliente == entity.CD_CLIENTE && w.Empresa == entity.CD_EMPRESA)) continue;

                var tipoExpedicion = this._context.T_TIPO_EXPEDICION.Where(d => d.TP_EXPEDICION == entity.TP_EXPEDICION).AsNoTracking().FirstOrDefault();

                Pedido pedido = this._mapper.Map(entity);
                pedido.ConfiguracionExpedicion = this._mapper.MapConfiguracionExpedicion(tipoExpedicion);
                resultado.Add(pedido);
            }

            return resultado;
        }

        public virtual List<Pedido> GetsPedidosDeCargas(List<long> cargas)
        {
            List<Pedido> list = new List<Pedido>();
            var estado = new List<string>() { CEstadoDetallePreparacion.ESTADO_PREP_PENDIENTE, CEstadoDetallePreparacion.ESTADO_PENDIENTE_AUTO, CEstadoDetallePreparacion.ESTADO_PREPARADO };

            var pedidos = _context.T_DET_PICKING
                .Join(
                    _context.T_PEDIDO_SAIDA,
                    dp => new { dp.NU_PEDIDO, dp.CD_EMPRESA, dp.CD_CLIENTE },
                    ps => new { ps.NU_PEDIDO, ps.CD_EMPRESA, ps.CD_CLIENTE },
                    (dp, ps) => new { Picking = dp, Pedido = ps }
                )
                .AsNoTracking()
                .Where(w => cargas.Contains(w.Picking.NU_CARGA ?? -1) && estado.Contains(w.Picking.ND_ESTADO))
                .Select(s => s.Pedido)
                .Distinct()
                .Join(
                    _context.T_TIPO_EXPEDICION,
                    ps => ps.TP_EXPEDICION,
                    te => te.TP_EXPEDICION,
                    (ps, te) => new { Pedido = ps, TipoExpedicion = te }
                )
                .ToList();

            foreach (var pedido in pedidos)
            {
                var tipoExpedicion = this._context.T_TIPO_EXPEDICION.Where(d => d.TP_EXPEDICION == pedido.TipoExpedicion.TP_EXPEDICION).AsNoTracking().FirstOrDefault();
                Pedido ped = this._mapper.Map(pedido.Pedido);
                ped.ConfiguracionExpedicion = this._mapper.MapConfiguracionExpedicion(tipoExpedicion);

                list.Add(ped);
            }

            return list;
        }

        public virtual List<ContenedorExpedir> GetsPedidosMostradorExpedicion(string pedido, string cliente, int empresa)
        {
            List<ContenedorExpedir> listTemp = new List<ContenedorExpedir>();
            List<T_TEMP_PEDIDO_MOSTRADOR> list = _context.T_TEMP_PEDIDO_MOSTRADOR.AsNoTracking().Where(x => x.NU_PEDIDO.Equals(pedido) && x.CD_CLIENTE.Equals(cliente) && x.CD_EMPRESA == empresa).ToList();
            foreach (var a in list)
            {
                ContenedorExpedir pedidoM = new ContenedorExpedir();
                pedidoM.NumeroPreparacion = a.NU_PREPARACION;
                pedidoM.NumeroContenedor = a.NU_CONTENEDOR;
                pedidoM.NumeroCarga = a.NU_CARGA;
                pedidoM.CodigoCliente = a.CD_CLIENTE;
                pedidoM.CodigoEmpresa = a.CD_EMPRESA;
                listTemp.Add(pedidoM);
            }
            return listTemp;
        }

        public virtual TempPedidoMostrador GetPedidoMostrador(ContenedorFacturar cont)
        {
            T_TEMP_PEDIDO_MOSTRADOR entity = _context.T_TEMP_PEDIDO_MOSTRADOR.AsNoTracking().FirstOrDefault(x => x.NU_PEDIDO.Equals(cont.NumeroPedido) && x.CD_CLIENTE.Equals(cont.CodigoCliente) && x.CD_EMPRESA == cont.CodigoEmpresa && x.NU_CARGA == cont.NumeroCarga && x.NU_CONTENEDOR == cont.NumeroContenedor && x.NU_PREPARACION == cont.NumeroPreparacion);
            return entity == null ? null : _mapper.MapToPedidoMostradorObject(entity);
        }

        public virtual TempPedidoMostrador GetPedidoMostrador(ContenedorExpedir cont)
        {
            T_TEMP_PEDIDO_MOSTRADOR entity = _context.T_TEMP_PEDIDO_MOSTRADOR.AsNoTracking().FirstOrDefault(x => x.CD_CLIENTE.Equals(cont.CodigoCliente) && x.CD_EMPRESA == cont.CodigoEmpresa && x.NU_CARGA == cont.NumeroCarga && x.NU_CONTENEDOR == cont.NumeroContenedor && x.NU_PREPARACION == cont.NumeroPreparacion);
            return entity == null ? null : _mapper.MapToPedidoMostradorObject(entity);
        }

        public virtual List<TempPedidoMostrador> GetPedidoMostradorSinFac(string pedido, string cliente, int empresa)
        {
            List<TempPedidoMostrador> listTemp = new List<TempPedidoMostrador>();
            List<T_TEMP_PEDIDO_MOSTRADOR> list = _context.T_TEMP_PEDIDO_MOSTRADOR.AsNoTracking().Where(x => x.NU_PEDIDO.Equals(pedido) && x.CD_CLIENTE.Equals(cliente) && x.CD_EMPRESA == empresa && x.CD_CAMION_FACTURADO == null).ToList();
            foreach (var a in list)
            {
                TempPedidoMostrador pedidoM = new TempPedidoMostrador();
                pedidoM = _mapper.MapToPedidoMostradorObject(a);
                listTemp.Add(pedidoM);
            }
            return listTemp;
        }

        public virtual List<ContenedorFacturar> GetPedidoMostradorSinFacturar(string pedido, string cliente, int empresa, out List<int> contenedoresConProblemas)
        {
            contenedoresConProblemas = new List<int>();

            List<ContenedorFacturar> listTemp = new List<ContenedorFacturar>();
            List<T_TEMP_PEDIDO_MOSTRADOR> pedidosMostrador = _context.T_TEMP_PEDIDO_MOSTRADOR
                .AsNoTracking()
                .Where(x => x.NU_PEDIDO.Equals(pedido)
                    && x.CD_CLIENTE.Equals(cliente)
                    && x.CD_EMPRESA == empresa
                    && x.CD_CAMION_FACTURADO == null)
                .ToList();

            foreach (var pedidoMostrador in pedidosMostrador)
            {
                T_CONTENEDOR contenedor = _context.T_CONTENEDOR
                    .AsNoTracking()
                    .FirstOrDefault(con => con.NU_CONTENEDOR == pedidoMostrador.NU_CONTENEDOR
                        && con.NU_PREPARACION == pedidoMostrador.NU_PREPARACION
                        && con.CD_SITUACAO == 601);

                ContenedorFacturar pedidoM = new ContenedorFacturar();
                pedidoM.NumeroPreparacion = pedidoMostrador.NU_PREPARACION;
                pedidoM.NumeroContenedor = pedidoMostrador.NU_CONTENEDOR;
                pedidoM.NumeroCarga = pedidoMostrador.NU_CARGA;
                pedidoM.NumeroPedido = pedidoMostrador.NU_PEDIDO;
                pedidoM.CodigoCliente = pedidoMostrador.CD_CLIENTE;
                pedidoM.CodigoEmpresa = pedidoMostrador.CD_EMPRESA;

                if (contenedor.CD_CAMION_FACTURADO != pedidoMostrador.CD_CAMION_FACTURADO)
                {
                    var attachedEntity = _context.T_TEMP_PEDIDO_MOSTRADOR.Local
                        .FirstOrDefault(x => x.NU_PREPARACION == pedidoMostrador.NU_PREPARACION
                            && x.NU_CONTENEDOR == pedidoMostrador.NU_CONTENEDOR
                            && x.NU_PEDIDO == pedidoMostrador.NU_PEDIDO
                            && x.CD_EMPRESA == pedidoMostrador.CD_EMPRESA
                            && x.CD_CLIENTE == pedidoMostrador.CD_CLIENTE
                            && x.NU_CARGA == pedidoMostrador.NU_CARGA);

                    pedidoMostrador.CD_CAMION_FACTURADO = contenedor.CD_CAMION_FACTURADO;

                    if (attachedEntity != null)
                    {
                        var attachedEntry = _context.Entry(attachedEntity);
                        attachedEntry.CurrentValues.SetValues(pedidoMostrador);
                        attachedEntry.State = EntityState.Modified;
                    }
                    else
                    {
                        this._context.T_TEMP_PEDIDO_MOSTRADOR.Attach(pedidoMostrador);
                        _context.Entry<T_TEMP_PEDIDO_MOSTRADOR>(pedidoMostrador).State = EntityState.Modified;
                    }
                }

                T_CAMION camionAsociado = _context.T_CLIENTE_CAMION
                    .Include("T_CAMION")
                    .FirstOrDefault(w => w.NU_CARGA == pedidoM.NumeroCarga)?.T_CAMION;

                if (camionAsociado != null && camionAsociado.TP_ARMADO_EGRESO != TipoArmadoEgreso.Retira)
                {
                    contenedoresConProblemas.Add(pedidoMostrador.NU_CONTENEDOR);
                }
                else
                {
                    listTemp.Add(pedidoM);
                }
            }

            return listTemp;
        }

        public virtual ConfiguracionExpedicionPedido GetConfiguracionExpedicion(string tipoExpedicion)
        {
            T_TIPO_EXPEDICION entity = _context.T_TIPO_EXPEDICION.AsNoTracking().FirstOrDefault(f => f.TP_EXPEDICION == tipoExpedicion);
            return entity == null ? null : _mapper.MapConfiguracionExpedicion(entity);
        }

        public virtual List<ConfiguracionExpedicionPedido> GetConfiguracionesExpedicion()
        {
            List<T_TIPO_EXPEDICION> entities = _context.T_TIPO_EXPEDICION.AsNoTracking().ToList();

            List<ConfiguracionExpedicionPedido> configuraciones = new List<ConfiguracionExpedicionPedido>();

            foreach (var entity in entities)
            {
                configuraciones.Add(_mapper.MapConfiguracionExpedicion(entity));
            }

            return configuraciones;
        }

        public virtual Dictionary<string, string> GetTiposPedido()
        {
            List<T_TIPO_PEDIDO> entities = this._context.T_TIPO_PEDIDO.AsNoTracking().ToList();

            Dictionary<string, string> tipos = new Dictionary<string, string>();

            foreach (var entity in entities)
            {
                tipos.Add(entity.TP_PEDIDO, entity.DS_TIPO_PEDIDO);
            }

            return tipos;
        }

        public virtual Dictionary<string, string> GetTiposPedido(string tipoExpedicion)
        {
            List<V_REL_TP_EXPEDICION_TP_PEDIDO> entities = this._context.V_REL_TP_EXPEDICION_TP_PEDIDO.AsNoTracking().Where(d => d.TP_EXPEDICION == tipoExpedicion).ToList();

            Dictionary<string, string> tipos = new Dictionary<string, string>();

            foreach (var entity in entities)
            {
                tipos.Add(entity.TP_PEDIDO, entity.DS_TIPO_PEDIDO);
            }

            return tipos;
        }

        public virtual IEnumerable<Tuple<string, string>> GetTiposExpedicionPedido()
        {
            var entities = this._context.V_REL_TP_EXPEDICION_TP_PEDIDO.AsNoTracking().ToList();
            var tipos = new List<Tuple<string, string>>();

            foreach (var entity in entities)
            {
                tipos.Add(new Tuple<string, string>(entity.TP_EXPEDICION, entity.TP_PEDIDO));
            }

            return tipos;
        }

        public virtual void LiberarPedidosConPendiente(int nuPrep, long nuTransaccion)
        {
            int? value = null;

            _context.T_PEDIDO_SAIDA
                .Where(d => d.NU_PREPARACION_MANUAL == nuPrep)
                .ExecuteUpdate(setters => setters
                    .SetProperty(d => d.NU_PREPARACION_MANUAL, value)
                    .SetProperty(d => d.NU_TRANSACCION, nuTransaccion)
                    .SetProperty(d => d.DT_UPDROW, DateTime.Now));
        }

        public virtual DetallePedidoExpedido GetDetallePedidoExpedido(int cdCam, string nuPedido, string cdCliente, int cdEmp, string cdProd, decimal cdFaixa, string Iden, bool IdEspecIdent)
        {
            string especificaIdentificador = this._mapper.MapBooleanToString(IdEspecIdent);

            T_DET_PEDIDO_EXPEDIDO entity = _context.T_DET_PEDIDO_EXPEDIDO
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_PEDIDO == nuPedido
                    && x.CD_CAMION == cdCam
                    && x.CD_CLIENTE == cdCliente
                    && x.CD_EMPRESA == cdEmp
                    && x.CD_FAIXA == cdFaixa
                    && x.CD_PRODUTO == cdProd
                    && x.NU_IDENTIFICADOR == Iden
                    && x.ID_ESPECIFICA_IDENTIFICADOR == especificaIdentificador);

            return entity == null ? null : _mapper.MapToDetalleExpedicionObject(entity);

        }

        public virtual List<PedidosExpedidosExel> GetListDetallePedidoExpedidoByTpAgente(int cdCam, List<ParametroInstancia> parametros)
        {
            List<PedidosExpedidosExel> res = new List<PedidosExpedidosExel>();
            var query = _context.V_PEDIDOS_EXPEDIDOS_EXEL.AsNoTracking().Where(x => x.CD_CAMION == cdCam);
            foreach (var p in parametros)
            {
                if (!string.IsNullOrEmpty(p.Valor))
                {
                    var array = p.Valor.Split(';');
                    var filtersIn = array.Where(s => s.First() != '!');
                    var filtersNotIn = new List<string>();
                    foreach (var n in array.Where(s => s.First() == '!').ToList())
                        filtersNotIn.Add(n.Remove(0, 1));
                    switch (p.Codigo)
                    {
                        case "CD_EMPRESA":
                            if (filtersIn.Any())
                                query = query.Where(s => filtersIn.Contains(s.CD_EMPRESA));
                            if (filtersNotIn.Any())
                                query = query.Where(s => !filtersNotIn.Contains(s.CD_EMPRESA));
                            break;
                        case "CD_CLIENTE":
                            if (filtersIn.Any())
                                query = query.Where(s => filtersIn.Contains(s.CD_CLIENTE));
                            if (filtersNotIn.Any())
                                query = query.Where(s => !filtersNotIn.Contains(s.CD_CLIENTE));
                            break;
                        case "CD_ROTA":
                            if (filtersIn.Any())
                                query = query.Where(s => filtersIn.Contains(s.CD_ROTA));
                            if (filtersNotIn.Any())
                                query = query.Where(s => !filtersNotIn.Contains(s.CD_ROTA));
                            break;
                        case "CD_TRANSPORTADORA":
                            if (filtersIn.Any())
                                query = query.Where(s => filtersIn.Contains(s.CD_TRANSPORTADORA));
                            if (filtersNotIn.Any())
                                query = query.Where(s => !filtersNotIn.Contains(s.CD_TRANSPORTADORA));
                            break;
                        case "TP_AGENTE":
                            if (filtersIn.Any())
                                query = query.Where(s => filtersIn.Contains(s.TP_AGENTE));
                            if (filtersNotIn.Any())
                                query = query.Where(s => !filtersNotIn.Contains(s.TP_AGENTE));
                            break;
                        case "DS_CONTENEDOR":
                            if (filtersIn.Any())
                                query = query.Where(s => filtersIn.Contains(s.DS_CONTENEDOR));
                            if (filtersNotIn.Any())
                                query = query.Where(s => !filtersNotIn.Contains(s.DS_CONTENEDOR));
                            break;
                    }
                }
            }
            foreach (var entity in query)
                res.Add(_mapper.MapToPedidosExpedidosExelObject(entity));
            return res;
        }

        public virtual bool GetCumpleCondicionEnvioPedidoConfirmado(string Pedido, int empresa, string cliente, List<ParametroInstancia> parametros)
        {
            string empresaString = Convert.ToString(empresa);

            var query = _context.V_VERIFICACION_PED_MAIL.AsNoTracking().Where(x => x.NU_PEDIDO == Pedido && x.CD_EMPRESA == empresaString && x.CD_CLIENTE == cliente);
            foreach (var p in parametros)
            {
                if (!string.IsNullOrEmpty(p.Valor))
                {
                    var array = p.Valor.Split(';');
                    var filtersIn = array.Where(s => s.First() != '!');
                    var filtersNotIn = new List<string>();
                    foreach (var n in array.Where(s => s.First() == '!').ToList())
                        filtersNotIn.Add(n.Remove(0, 1));
                    switch (p.Codigo)
                    {

                        case "CD_EMPRESA":
                            if (filtersIn.Any())
                                query = query.Where(s => filtersIn.Contains(s.CD_EMPRESA));
                            if (filtersNotIn.Any())
                                query = query.Where(s => !filtersNotIn.Contains(s.CD_EMPRESA));
                            break;
                        case "CD_CLIENTE":
                            if (filtersIn.Any())
                                query = query.Where(s => filtersIn.Contains(s.CD_CLIENTE));
                            if (filtersNotIn.Any())
                                query = query.Where(s => !filtersNotIn.Contains(s.CD_CLIENTE));
                            break;
                        case "CD_ROTA":
                            if (filtersIn.Any())
                                query = query.Where(s => filtersIn.Contains(s.CD_ROTA));
                            if (filtersNotIn.Any())
                                query = query.Where(s => !filtersNotIn.Contains(s.CD_ROTA));
                            break;
                        case "CD_TRANSPORTADORA":
                            if (filtersIn.Any())
                                query = query.Where(s => filtersIn.Contains(s.CD_TRANSPORTADORA));
                            if (filtersNotIn.Any())
                                query = query.Where(s => !filtersNotIn.Contains(s.CD_TRANSPORTADORA));
                            break;
                        case "TP_AGENTE":
                            if (filtersIn.Any())
                                query = query.Where(s => filtersIn.Contains(s.TP_AGENTE));
                            if (filtersNotIn.Any())
                                query = query.Where(s => !filtersNotIn.Contains(s.TP_AGENTE));
                            break;
                        case "NU_PREDIO":
                            if (filtersIn.Any())
                                query = query.Where(s => filtersIn.Contains(s.NU_PREDIO));
                            if (filtersNotIn.Any())
                                query = query.Where(s => !filtersNotIn.Contains(s.NU_PREDIO));
                            break;
                    }
                }
            }

            return query.Any();
        }

        public virtual Dictionary<string, string> GetInformacionPedidoEnsambladoPorContenedor(int preparacion, int contenedor)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            var info = (from dp in _context.T_DET_PICKING.AsNoTracking()
                        join ps in _context.T_PEDIDO_SAIDA.AsNoTracking() on new { dp.CD_EMPRESA, dp.NU_PEDIDO, dp.CD_CLIENTE } equals new { ps.CD_EMPRESA, ps.NU_PEDIDO, ps.CD_CLIENTE }
                        join cli in _context.T_CLIENTE.AsNoTracking() on new { dp.CD_EMPRESA, dp.CD_CLIENTE } equals new { cli.CD_EMPRESA, cli.CD_CLIENTE }
                        join emp in _context.T_EMPRESA.AsNoTracking() on new { dp.CD_EMPRESA } equals new { emp.CD_EMPRESA }
                        where dp.NU_CONTENEDOR == contenedor && dp.NU_PREPARACION == preparacion
                        group ps by new { pedido = ps, DS_CLIENTE = cli.DS_CLIENTE, NM_EMPRESA = emp.NM_EMPRESA } into g
                        select g.Key).FirstOrDefault();

            dic["NU_PREPARACION"] = preparacion.ToString();
            dic["NU_CONTENEDOR"] = contenedor.ToString();
            dic["NU_PEDIDO"] = info.pedido.NU_PEDIDO;
            dic["CD_CLIENTE"] = info.pedido.CD_CLIENTE;
            dic["DS_CLIENTE"] = info.DS_CLIENTE;
            dic["CD_EMPRESA"] = info.pedido.CD_EMPRESA.ToString();
            dic["NM_EMPRESA"] = info.NM_EMPRESA;
            dic["NU_PRDC_INGRESO"] = info.pedido.NU_PRDC_INGRESO ?? "";
            dic["TP_PEDIDO"] = info.pedido.TP_PEDIDO ?? "";

            //Info pedido original
            var nuPedidoOriginal = info.pedido.VL_SERIALIZADO_1.Split(';')[0].Split(',')[1];
            var pedido = _context.T_PEDIDO_SAIDA
                .AsNoTracking()
                .FirstOrDefault(w => w.NU_PEDIDO == nuPedidoOriginal
                    && w.CD_EMPRESA == info.pedido.CD_EMPRESA
                    && w.CD_CLIENTE == info.pedido.CD_CLIENTE);

            dic["DS_ANEXO1"] = pedido.DS_ANEXO1 ?? "";
            dic["DS_ANEXO2"] = pedido.DS_ANEXO2 ?? "";
            dic["DS_ANEXO3"] = pedido.DS_ANEXO3 ?? "";
            dic["DS_ANEXO4"] = pedido.DS_ANEXO4 ?? "";
            dic["DS_MEMO"] = pedido.DS_MEMO ?? "";
            dic["DS_ENDERECO"] = pedido.DS_ENDERECO ?? "";
            dic["CD_ROTA"] = pedido.CD_ROTA?.ToString() ?? "";
            dic["DT_ENTREGA"] = pedido.DT_ENTREGA?.ToString("dd/MM/yyyy") ?? "";

            return dic;
        }

        public virtual Pedido GetPedidoEnsambladoPorContenedor(int preparacion, int contenedor)
        {
            var pedido = (from dp in _context.T_DET_PICKING.AsNoTracking()
                          join ps in _context.T_PEDIDO_SAIDA.AsNoTracking() on new { dp.CD_EMPRESA, dp.NU_PEDIDO, dp.CD_CLIENTE } equals new { ps.CD_EMPRESA, ps.NU_PEDIDO, ps.CD_CLIENTE }
                          where dp.NU_CONTENEDOR == contenedor && dp.NU_PREPARACION == preparacion && ps.TP_PEDIDO == "WISMAC"
                          select ps).Distinct().FirstOrDefault();

            return _mapper.Map(pedido);
        }

        public virtual Pedido GetPedidoOriginalEnsambladoPorPedido(Pedido pedido)
        {
            var nuPedidoOriginal = pedido.VlSerealizado_1.Split(';')[0].Split(',')[1];

            return _mapper.Map(
               _context.T_PEDIDO_SAIDA
               .AsNoTracking()
               .FirstOrDefault(w => w.NU_PEDIDO == nuPedidoOriginal
                && w.CD_EMPRESA == pedido.Empresa
                && w.CD_CLIENTE == pedido.Cliente)
               );
        }

        public virtual int GetCantidadPedidosOrden(int codigoEmpresa, string codigoCliente, string numeroIngreso)
        {
            return _context.T_PEDIDO_SAIDA
                .AsNoTracking()
                .Where(p => p.CD_EMPRESA == codigoEmpresa
                    && p.CD_CLIENTE == codigoCliente
                    && p.NU_PRDC_INGRESO == numeroIngreso)
                .Count();
        }

        public virtual bool GetTipoExpedicionModalidadEmpaque(Pedido pedido)
        {
            bool modalidadCorrecta = false;
            if (_context.T_TIPO_EXPEDICION.AsNoTracking().FirstOrDefault(f => f.TP_EXPEDICION == pedido.ConfiguracionExpedicion.Tipo).FL_MODALIDAD_EMPAQUETADO == "C")
            {
                modalidadCorrecta = true;
            }
            return modalidadCorrecta;
        }

        public virtual string GetTipodeArmadoEgreso(string nuPedido, string cliente, int empresa)
        {
            var estadosAnulacion = EstadoDetallePreparacion.GetEstadosAnulacion();

            bool existe = _context.T_DET_PICKING.AsNoTracking().Any(p => p.NU_PEDIDO == nuPedido && p.CD_CLIENTE == cliente && p.CD_EMPRESA == empresa
            && !estadosAnulacion.Contains(p.ND_ESTADO));
            return existe ? TipoArmadoPedido.ConCarga : TipoArmadoPedido.SinCarga;
        }

        public virtual List<Pedido> GetPedidosNuevasCargas(int cdCamion)
        {
            var pedidos = new List<Pedido>();
            var entities = _context.V_PED_NUEVAS_CARGAS.AsNoTracking().Where(p => p.CD_CAMION == cdCamion);

            foreach (var entity in entities)
            {
                var tipoExpedicion = _context.T_TIPO_EXPEDICION.AsNoTracking().FirstOrDefault(d => d.TP_EXPEDICION == entity.TP_EXPEDICION);

                Pedido pedido = this._mapper.Map(entity);
                pedido.ConfiguracionExpedicion = this._mapper.MapConfiguracionExpedicion(tipoExpedicion);
                pedidos.Add(pedido);
            }
            return pedidos;
        }

        public virtual int GetNextNuPedidoManual()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_NU_PEDI_MANUAL);
        }

        public virtual long GetNextIdLogPedidoAnuladoLpn()
        {
            return this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_LOG_PEDIDO_ANULADO_LPN);
        }

        public virtual decimal? GetNuPonderacionPedido(int cdEmpresa, string cdCliente, string nuPedido)
        {
            return _context.T_PEDIDO_SAIDA.FirstOrDefault(x => x.CD_EMPRESA == cdEmpresa && x.CD_CLIENTE == cdCliente && x.NU_PEDIDO == nuPedido)?.QT_PONDERACION_PEDIDO;
        }

        public virtual decimal? GetCantidadEnviada(string nuPedido, string cdCliente, int cdEmpresa, string cdProducto, decimal faixa, string identificador, string idEspecIdent)
        {
            return _context.V_CANTIDAD_ENVIADA_EXP
                .FirstOrDefault(x => x.NU_PEDIDO == nuPedido
                    && x.CD_CLIENTE == cdCliente
                    && x.CD_EMPRESA == cdEmpresa
                    && x.CD_PRODUTO == cdProducto
                    && x.CD_FAIXA == faixa
                    && x.NU_IDENTIFICADOR == identificador
                    && x.ID_ESPECIFICA_IDENTIFICADOR == idEspecIdent)?.QT_PRODUTO ?? 0;
        }

        public virtual decimal? GetCantidadPreparada(int cdCamion, string nuPedido, string cdCliente, int cdEmpresa, string cdProducto, decimal faixa, string identificador, string idEspecIdent)
        {
            return _context.V_CANT_PREPARADA_WEXP
                .FirstOrDefault(x => x.CD_CAMION == cdCamion
                    && x.NU_PEDIDO == nuPedido
                    && x.CD_CLIENTE == cdCliente
                    && x.CD_EMPRESA == cdEmpresa
                    && x.CD_PRODUTO == cdProducto
                    && x.CD_FAIXA == faixa
                    && x.NU_IDENTIFICADOR == identificador
                    && x.ID_ESPECIFICA_IDENTIFICADOR == idEspecIdent
                    && x.ID_CARGAR == "S")?.QT_PREPARADO ?? 0;
        }

        #endregion

        #region Remove
        public virtual void DeleteDetallePedido(DetallePedido detalle)
        {
            T_DET_PEDIDO_SAIDA entity = this._mapper.MapDetalle(detalle);
            T_DET_PEDIDO_SAIDA attachedEntity = _context.T_DET_PEDIDO_SAIDA.Local
                .FirstOrDefault(w => w.NU_PEDIDO == entity.NU_PEDIDO
                    && w.CD_CLIENTE == entity.CD_CLIENTE
                    && w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.CD_PRODUTO == entity.CD_PRODUTO
                    && w.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                    && w.CD_FAIXA == entity.CD_FAIXA
                    && w.ID_ESPECIFICA_IDENTIFICADOR == entity.ID_ESPECIFICA_IDENTIFICADOR);

            if (attachedEntity != null)
            {
                this._context.T_DET_PEDIDO_SAIDA.Remove(attachedEntity);
            }
            else
            {
                this._context.T_DET_PEDIDO_SAIDA.Attach(entity);
                this._context.T_DET_PEDIDO_SAIDA.Remove(entity);
            }
        }

        public virtual void RemovePedidoMostrador(TempPedidoMostrador detExp)
        {
            T_TEMP_PEDIDO_MOSTRADOR entity = this._mapper.MapToPedidoMostradorEntity(detExp);
            T_TEMP_PEDIDO_MOSTRADOR attachedEntity = _context.T_TEMP_PEDIDO_MOSTRADOR.Local
                .FirstOrDefault(w => w.NU_PEDIDO == entity.NU_PEDIDO
                    && w.CD_CLIENTE == entity.CD_CLIENTE
                    && w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.NU_PREPARACION == entity.NU_PREPARACION
                    && w.NU_CONTENEDOR == entity.NU_CONTENEDOR
                    && w.NU_CARGA == entity.NU_CARGA);

            if (attachedEntity != null)
            {
                _context.T_TEMP_PEDIDO_MOSTRADOR.Remove(attachedEntity);
            }
            else
            {
                _context.T_TEMP_PEDIDO_MOSTRADOR.Attach(entity);
                _context.T_TEMP_PEDIDO_MOSTRADOR.Remove(entity);
            }
        }

        public virtual void DeleteDetallePedidoLpn(DetallePedidoLpn detPedidoLpn)
        {
            T_DET_PEDIDO_SAIDA_LPN entity = this._lpnMapper.MapToEntity(detPedidoLpn);
            T_DET_PEDIDO_SAIDA_LPN attachedEntity = _context.T_DET_PEDIDO_SAIDA_LPN.Local
                .FirstOrDefault(w => w.NU_PEDIDO == entity.NU_PEDIDO
                    && w.CD_CLIENTE == entity.CD_CLIENTE
                    && w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.CD_PRODUTO == entity.CD_PRODUTO
                    && w.CD_FAIXA == entity.CD_FAIXA
                    && w.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR
                    && w.ID_ESPECIFICA_IDENTIFICADOR == entity.ID_ESPECIFICA_IDENTIFICADOR
                    && w.TP_LPN_TIPO == entity.TP_LPN_TIPO
                    && w.ID_LPN_EXTERNO == entity.ID_LPN_EXTERNO);

            if (attachedEntity != null)
            {
                this._context.T_DET_PEDIDO_SAIDA_LPN.Remove(attachedEntity);
            }
            else
            {
                this._context.T_DET_PEDIDO_SAIDA_LPN.Attach(entity);
                this._context.T_DET_PEDIDO_SAIDA_LPN.Remove(entity);
            }
        }
        #endregion

        #region Dapper

        #region Get

        public virtual List<long> GetNewIdPedido(int count)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();
            return _dapper.GetNextSequenceValues<long>(connection, Secuencias.S_NU_PEDI_MANUAL, count, tran).ToList();
        }

        public virtual async Task<Pedido> GetPedidoOrNull(string nuPedido, int empresa, string tipoAgente, string codigoAgente, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var agente = _agenteRepository.GetAgenteOrNull(empresa, codigoAgente, tipoAgente).Result;

                if (agente == null)
                    return null;

                Pedido modelo = GetPedido(nuPedido, empresa, agente.CodigoInterno, connection, out DynamicParameters param);

                if (modelo != null)
                {
                    modelo.IsManual = modelo.ManualId.ToUpper() == "S";
                    modelo.ConfiguracionExpedicion = new ConfiguracionExpedicionPedido() { Tipo = modelo.TipoExpedicionId };
                    modelo.IsSincronizacionRealizada = modelo.SincronizacionRealizadaId.ToUpper() == "S";
                    modelo.Lineas = GetDetalles(connection, param);
                }

                return modelo;
            }
        }

        public virtual List<DetallePedido> GetDetalles(DbConnection connection, DynamicParameters param)
        {
            string sql = @"SELECT 
                        NU_PEDIDO as Id,
                        CD_CLIENTE as Cliente,
                        CD_EMPRESA as Empresa,
                        CD_FAIXA as Faixa,
                        CD_PRODUTO as Producto,
                        DS_MEMO as Memo,
                        DT_ADDROW as FechaAlta,
                        DT_GENERICO_1 as FechaGenerica_1,
                        DT_UPDROW as FechaModificacion,
                        ID_AGRUPACION as Agrupacion,
                        ID_ESPECIFICA_IDENTIFICADOR as EspecificaIdentificadorId,
                        NU_GENERICO_1 as NuGenerico_1,
                        NU_IDENTIFICADOR as Identificador,
                        NU_TRANSACCION as Transaccion,
                        QT_ABASTECIDO as CantidadAbastecida,
                        QT_ANULADO as CantidadAnulada,
                        QT_ANULADO_FACTURA as CantidadAnuladaFactura,
                        QT_CARGADO as CantidadCargada,
                        QT_CONTROLADO as CantidadControlada,
                        QT_CROSS_DOCK as CantidadCrossDocking,
                        QT_EXPEDIDO as CantidadExpedida,
                        QT_FACTURADO as CantidadFacturada,
                        QT_LIBERADO as CantidadLiberada,
                        QT_PEDIDO as Cantidad,
                        QT_PEDIDO_ORIGINAL as CantidadOriginal,
                        QT_PREPARADO as CantidadPreparada,
                        QT_TRANSFERIDO as CantidadTransferida,
                        QT_UND_ASOCIADO_CAMION as CantUndAsociadoCamion,
                        VL_GENERICO_1 as VlGenerico_1,
                        VL_PORCENTAJE_TOLERANCIA as PorcentajeTolerancia,
                        VL_SERIALIZADO_1 as DatosSerializados
                    FROM T_DET_PEDIDO_SAIDA
                    WHERE NU_PEDIDO = :nuPedido AND CD_EMPRESA = :empresa AND CD_CLIENTE = :cliente";

            var detalles = _dapper.Query<DetallePedido>(connection, sql, param: param, commandType: CommandType.Text).ToList();
            var dictDetalles = new Dictionary<string, DetallePedido>();

            foreach (var d in detalles)
            {
                var key = $"{d.Id}.{d.Cliente}.{d.Empresa}.{d.Producto}.{d.Faixa.ToString("#.###")}.{d.Identificador}.{d.EspecificaIdentificadorId}";

                dictDetalles[key] = d;

                d.EspecificaIdentificador = d.EspecificaIdentificadorId.ToUpper() == "S";
            }

            var duplicados = GetDuplicados(connection, param);

            foreach (var d in duplicados)
            {
                var key = $"{d.Pedido}.{d.Cliente}.{d.Empresa}.{d.Producto}.{d.Faixa.ToString("#.###")}.{d.Identificador}.{d.IdEspecificaIdentificador}";

                dictDetalles[key].Duplicados.Add(d);
            }

            return detalles;
        }

        public virtual DetallePedido GetDetalle(DbConnection connection, DynamicParameters param)
        {
            string sql = @"SELECT 
                        NU_PEDIDO as Id,
                        CD_CLIENTE as Cliente,
                        CD_EMPRESA as Empresa,
                        CD_FAIXA as Faixa,
                        CD_PRODUTO as Producto,
                        DS_MEMO as Memo,
                        DT_ADDROW as FechaAlta,
                        DT_GENERICO_1 as FechaGenerica_1,
                        DT_UPDROW as FechaModificacion,
                        ID_AGRUPACION as Agrupacion,
                        ID_ESPECIFICA_IDENTIFICADOR as EspecificaIdentificadorId,
                        NU_GENERICO_1 as NuGenerico_1,
                        NU_IDENTIFICADOR as Identificador,
                        NU_TRANSACCION as Transaccion,
                        QT_ABASTECIDO as CantidadAbastecida,
                        QT_ANULADO as CantidadAnulada,
                        QT_ANULADO_FACTURA as CantidadAnuladaFactura,
                        QT_CARGADO as CantidadCargada,
                        QT_CONTROLADO as CantidadControlada,
                        QT_CROSS_DOCK as CantidadCrossDocking,
                        QT_EXPEDIDO as CantidadExpedida,
                        QT_FACTURADO as CantidadFacturada,
                        QT_LIBERADO as CantidadLiberada,
                        QT_PEDIDO as Cantidad,
                        QT_PEDIDO_ORIGINAL as CantidadOriginal,
                        QT_PREPARADO as CantidadPreparada,
                        QT_TRANSFERIDO as CantidadTransferida,
                        QT_UND_ASOCIADO_CAMION as CantUndAsociadoCamion,
                        VL_GENERICO_1 as VlGenerico_1,
                        VL_PORCENTAJE_TOLERANCIA as PorcentajeTolerancia,
                        VL_SERIALIZADO_1 as DatosSerializados
                    FROM T_DET_PEDIDO_SAIDA
                    WHERE NU_PEDIDO = :nuPedido AND CD_EMPRESA = :empresa AND CD_CLIENTE = :cliente 
                    AND CD_PRODUTO = :producto AND CD_FAIXA = :faixa AND NU_IDENTIFICADOR= :identificador AND ID_ESPECIFICA_IDENTIFICADOR = :especificaIdentificador ";

            var detalles = _dapper.Query<DetallePedido>(connection, sql, param: param, commandType: CommandType.Text).FirstOrDefault();
            var dictDetalles = new Dictionary<string, DetallePedido>();
            var duplicados = GetDuplicados(connection, param);

            foreach (var d in duplicados)
            {
                var key = $"{d.Pedido}.{d.Cliente}.{d.Empresa}.{d.Producto}.{d.Faixa.ToString("#.###")}.{d.Identificador}.{d.IdEspecificaIdentificador}";

                dictDetalles[key].Duplicados.Add(d);
            }

            return detalles;
        }

        public virtual async Task<DetallePedido> GetDetallePedido(DetallePedido detalle, string cdCliente, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);
                var param = new DynamicParameters(new
                {
                    nuPedido = detalle.Id,
                    empresa = detalle.Empresa,
                    cliente = cdCliente,
                    faixa = detalle.Faixa,
                    producto = detalle.Producto,
                    identificador = detalle.Identificador,
                    especificaIdentificador = detalle.EspecificaIdentificadorId,
                });

                return GetDetalle(connection, param);
            }
        }

        public virtual List<DetallePedidoDuplicado> GetDuplicados(DbConnection connection, DynamicParameters param)
        {
            string sql = @"SELECT 
                        NU_PEDIDO as Pedido,
                        CD_CLIENTE as Cliente,
                        CD_EMPRESA as Empresa,
                        CD_PRODUTO as Producto,
                        CD_FAIXA as Faixa,
                        NU_IDENTIFICADOR as Identificador,
                        ID_ESPECIFICA_IDENTIFICADOR as IdEspecificaIdentificador,
                        ID_LINEA_SISTEMA_EXTERNO as IdLineaSistemaExterno,
                        TP_LINEA as TipoLinea,
                        QT_PEDIDO as CantidadPedida,
                        QT_EXPEDIDO as CantidadExpedida,
                        QT_ANULADO as CantidadAnulada,
                        QT_FACTURADO as CantidadFacturada,
                        DT_ADDROW as FechaAlta,
                        DT_UPDROW as FechaModificacion,
                        VL_SERIALIZADO_1 as DatosSerializados
                    FROM T_DET_PEDIDO_SAIDA_DUP
                    WHERE NU_PEDIDO = :nuPedido AND CD_EMPRESA = :empresa AND CD_CLIENTE = :cliente";

            return _dapper.Query<DetallePedidoDuplicado>(connection, sql, param: param, commandType: CommandType.Text).ToList();
        }

        public virtual Pedido GetPedido(string nuPedido, int empresa, string cliente, DbConnection connection, out DynamicParameters param)
        {
            var template = new { nuPedido = nuPedido, empresa = empresa, cliente = cliente };
            string sql = @"SELECT 
                    NU_PEDIDO as Id,
                    CD_CLIENTE as Cliente,
                    CD_EMPRESA as Empresa,
                    CD_CONDICION_LIBERACION as CondicionLiberacion,
                    CD_FUN_RESP as FuncionarioResponsable,
                    CD_ORIGEN as Origen,
                    CD_PUNTO_ENTREGA as PuntoEntrega,
                    CD_ROTA as Ruta,
                    CD_SITUACAO as Estado,
                    CD_TRANSPORTADORA as CodigoTransportadora,
                    CD_UF as CodigoUF,
                    CD_ZONA as Zona,
                    DS_ANEXO1 as Anexo,
                    DS_ANEXO2 as Anexo2,
                    DS_ANEXO3 as Anexo3,
                    DS_ANEXO4 as Anexo4,
                    DS_ENDERECO as DireccionEntrega,
                    DS_MEMO as Memo,
                    DS_MEMO_1 as Memo1,
                    DT_ADDROW as FechaAlta,
                    DT_EMITIDO as FechaEmision,
                    DT_ENTREGA as FechaEntrega,
                    DT_FUN_RESP as FechaFuncionarioResponsable,
                    DT_GENERICO_1 as FechaGenerica_1,
                    DT_LIBERAR_DESDE as FechaLiberarDesde,
                    DT_LIBERAR_HASTA as FechaLiberarHasta,
                    DT_ULT_PREPARACION as FechaUltimaPreparacion,
                    DT_UPDROW as FechaModificacion,
                    FL_SYNC_REALIZADA as SincronizacionRealizadaId,
                    ID_AGRUPACION as Agrupacion,
                    ID_MANUAL as ManualId,
                    ND_ACTIVIDAD as Actividad,
                    NU_GENERICO_1 as NuGenerico_1,
                    NU_INTERFAZ_FACTURACION as NroIntzFacturacion,
                    NU_ORDEN_ENTREGA as OrdenEntrega,
                    NU_ORDEN_LIBERACION as NumeroOrdenLiberacion,
                    NU_PRDC_INGRESO as IngresoProduccion,
                    NU_PREDIO as Predio,
                    NU_PREPARACION_MANUAL as NroPrepManual,
                    NU_PREPARACION_PROGRAMADA as PreparacionProgramada,
                    NU_TRANSACCION as Transaccion,
                    NU_ULT_PREPARACION as NumeroUltimaPreparacion,
                    TP_EXPEDICION as TipoExpedicionId,
                    TP_PEDIDO as Tipo,
                    VL_COMPARTE_CONTENEDOR_ENTREGA as ComparteContenedorEntrega,
                    VL_COMPARTE_CONTENEDOR_PICKING as ComparteContenedorPicking,
                    VL_GENERICO_1 as VlGenerico_1,
                    VL_SERIALIZADO_1 as VlSerealizado_1,
                    NU_CARGA as NuCarga,
                    NU_TELEFONE as Telefono,
                    NU_TELEFONE2 as TelefonoSecundario,
                    VL_LONGITUD as Longitud,
                    VL_LATITUD as Latitud
                FROM T_PEDIDO_SAIDA
                WHERE NU_PEDIDO = :nuPedido AND CD_EMPRESA = :empresa AND CD_CLIENTE = :cliente";

            param = new DynamicParameters(template);

            return _dapper.Query<Pedido>(connection, sql, param: param, commandType: CommandType.Text).FirstOrDefault();
        }

        #endregion

        #region AddPedidos

        public virtual async Task AddPedidos(List<Pedido> pedidos, IPedidoServiceContext context, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var bulkContext = GetBulkOperationContext(pedidos, context, connection);

                using (var tran = connection.BeginTransaction())
                {
                    await BulkInsertPedidos(connection, tran, bulkContext.NewPedidos);
                    await BulkInsertLineas(connection, tran, bulkContext.NewLineas);
                    await BulkInsertDuplicados(connection, tran, bulkContext.NewDuplicados);
                    await BulkInsertDetallesLpnAsync(connection, tran, bulkContext.NewDetalleLpn);
                    await BulkInsertAtributos(connection, tran, bulkContext.NewAtributos);
                    await BulkInsertAtributosLpn(connection, tran, bulkContext.NewAtributosLpn);
                    await BulkInsertAtributosDetalle(connection, tran, bulkContext.NewAtributosDetalle);

                    tran.Commit();
                }
            }
        }

        public virtual PedidoInsertBulkOperationContext GetBulkOperationContext(List<Pedido> pedidos, IPedidoServiceContext serviceContext, DbConnection connection)
        {
            var context = new PedidoInsertBulkOperationContext();

            foreach (var pedido in pedidos)
            {
                context.NewPedidos.Add(GetPedidoEntity(pedido));

                foreach (var linea in pedido.Lineas)
                {
                    linea.Transaccion = pedido.Transaccion;

                    context.NewLineas.Add(GetLineaEntity(linea));

                    foreach (var duplicado in linea.Duplicados)
                    {
                        duplicado.Transaccion = linea.Transaccion;
                        duplicado.FechaAlta = DateTime.Now;
                        context.NewDuplicados.Add(GetDuplicadoEntity(duplicado));
                    }

                    foreach (var detalleLpn in linea.DetallesLpn)
                    {
                        detalleLpn.Transaccion = linea.Transaccion;
                        detalleLpn.FechaAlta = DateTime.Now;
                        context.NewDetalleLpn.Add(GetDetalleLpnEntity(detalleLpn));

                        foreach (var configuracion in detalleLpn.Atributos)
                        {
                            configuracion.InternalId = Guid.NewGuid();
                            configuracion.Transaccion = linea.Transaccion;
                            configuracion.FechaAlta = DateTime.Now;

                            context.NewAtributosLpn.Add(GetAtributosLpnEntity(configuracion));

                            foreach (var atributo in configuracion.Atributos)
                            {
                                atributo.InternalId = configuracion.InternalId;
                                atributo.IdAtributo = serviceContext.GetAtributo(atributo.Nombre).Id;
                                atributo.Transaccion = linea.Transaccion;
                                atributo.FechaAlta = DateTime.Now;

                                context.NewAtributosDetalle.Add(GetAtributoEntity(atributo));
                            }
                        }
                    }

                    foreach (var configuracion in linea.Atributos)
                    {
                        configuracion.InternalId = Guid.NewGuid();
                        configuracion.Transaccion = linea.Transaccion;
                        configuracion.FechaAlta = DateTime.Now;

                        context.NewAtributos.Add(GetAtributosEntity(configuracion));

                        foreach (var atributo in configuracion.Atributos)
                        {
                            atributo.InternalId = configuracion.InternalId;
                            atributo.IdAtributo = serviceContext.GetAtributo(atributo.Nombre).Id;
                            atributo.Transaccion = linea.Transaccion;
                            atributo.FechaAlta = DateTime.Now;

                            context.NewAtributosDetalle.Add(GetAtributoEntity(atributo));
                        }
                    }
                }

                foreach (var lpnPedido in pedido.Lpns)
                {
                    var lpn = serviceContext.GetLpn(lpnPedido.IdExterno, lpnPedido.Tipo);
                    var detalles = serviceContext.GetDetallesLpn(lpn.NumeroLPN);
                    var detalleLpnAgrupado = detalles.GroupBy(x => new { x.NumeroLPN, x.IdExterno, x.Tipo, x.CodigoProducto, x.Faixa, x.Lote })
                        .Select(x => new LpnDetalle()
                        {
                            NumeroLPN = x.Key.NumeroLPN,
                            IdExterno = x.Key.IdExterno,
                            Tipo = x.Key.Tipo,
                            CodigoProducto = x.Key.CodigoProducto,
                            Faixa = x.Key.Faixa,
                            Lote = x.Key.Lote,
                            Cantidad = x.Sum(d => d.Cantidad),
                            CantidadReserva = x.Sum(d => d.CantidadReserva),
                            CantidadDeclarada = x.Sum(d => d.CantidadDeclarada),
                            CantidadRecibida = x.Sum(d => d.CantidadRecibida)
                        })
                        .ToList();

                    foreach (var detalle in detalleLpnAgrupado)
                    {
                        var detallePedido = context.NewLineas
                            .FirstOrDefault(x => x.Id == pedido.Id
                                && x.Cliente == pedido.Cliente
                                && x.Empresa == pedido.Empresa
                                && x.Producto == detalle.CodigoProducto
                                && x.Faixa == detalle.Faixa
                                && (x.Identificador == ManejoIdentificadorDb.IdentificadorAuto || x.Identificador == detalle.Lote));

                        if (detallePedido != null)
                            ModificarDetallePedidoLpn(context, pedido, lpn, detalle, detallePedido);
                        else
                            AgregarDetallePedidoLpn(context, pedido, lpn, detalle);
                    }
                }
            }

            var totalNewAtributos = context.NewAtributos.Count + context.NewAtributosLpn.Count;

            if (totalNewAtributos > 0)
            {
                var idConfiguracionByInternalId = new Dictionary<Guid, long>();
                var configuracionIds = GetNewConfiguracionAtributoIds(totalNewAtributos, connection);

                for (int i = 0; i < context.NewAtributos.Count; i++)
                {
                    var configuracion = context.NewAtributos[i];
                    configuracion.IdConfiguracion = configuracionIds[i];
                    idConfiguracionByInternalId[configuracion.InternalId] = configuracionIds[i];
                }

                for (int i = 0; i < context.NewAtributosLpn.Count; i++)
                {
                    var configuracion = context.NewAtributosLpn[i];
                    configuracion.IdConfiguracion = configuracionIds[i + context.NewAtributos.Count];
                    idConfiguracionByInternalId[configuracion.InternalId] = configuracion.IdConfiguracion;
                }

                foreach (var atributo in context.NewAtributosDetalle)
                {
                    atributo.IdConfiguracion = idConfiguracionByInternalId[atributo.InternalId];
                }
            }

            return context;
        }

        public virtual List<long> GetNewConfiguracionAtributoIds(int count, DbConnection connection)
        {
            return _dapper.GetNextSequenceValues<long>(connection, "S_DET_PEDIDO_SAIDA_ATRIB", count).ToList();
        }

        public virtual void AgregarDetallePedidoLpn(PedidoInsertBulkOperationContext context, Pedido pedido, Lpn lpn, LpnDetalle detalle)
        {
            var identificador = detalle.Lote?.Trim() ?? string.Empty;
            var especificaIdentificador = true;

            var cantidad = GetCantidadPedidoLpn(lpn, detalle);

            if (cantidad > 0)
            {
                if (string.IsNullOrEmpty(identificador) || identificador == ManejoIdentificadorDb.IdentificadorAuto)
                    especificaIdentificador = false;

                var especificaLote = _mapper.MapBooleanToString(especificaIdentificador);

                var model = new DetallePedido(especificaLote);

                model.Id = pedido.Id;
                model.Cliente = pedido.Cliente;
                model.Agrupacion = pedido.Agrupacion;
                model.Cantidad = cantidad;
                model.FechaAlta = DateTime.Now;
                model.EspecificaIdentificador = especificaIdentificador;
                model.EspecificaIdentificadorId = especificaLote;
                model.CantidadAnulada = 0;
                model.CantidadLiberada = 0;
                model.CantidadOriginal = cantidad;
                model.Empresa = lpn.Empresa;
                model.Faixa = 1;
                model.Id = pedido.Id;
                model.Identificador = identificador;
                model.Producto = detalle.CodigoProducto;
                model.Transaccion = pedido.Transaccion;
                context.NewLineas.Add(model);

                var detalleLpnPedido = context.NewDetalleLpn
                    .FirstOrDefault(x => x.Pedido == pedido.Id
                        && x.Cliente == pedido.Cliente
                        && x.Empresa == pedido.Empresa
                        && x.Producto == detalle.CodigoProducto
                        && x.Faixa == detalle.Faixa
                        && (x.Identificador == ManejoIdentificadorDb.IdentificadorAuto || x.Identificador == detalle.Lote));

                if (detalleLpnPedido == null)
                {
                    var detalleLpn = new DetallePedidoLpn()
                    {
                        Cliente = pedido.Cliente,
                        CantidadPedida = cantidad,
                        Empresa = lpn.Empresa,
                        Faixa = 1,
                        Identificador = identificador,
                        IdEspecificaIdentificador = especificaLote,
                        Pedido = pedido.Id.Trim(),
                        Producto = detalle.CodigoProducto,
                        Tipo = lpn.Tipo ?? string.Empty,
                        IdLpnExterno = lpn.IdExterno,
                        CantidadLiberada = 0,
                        CantidadAnulada = 0,
                        NumeroLpn = lpn.NumeroLPN
                    };

                    detalleLpn.Transaccion = pedido.Transaccion;
                    detalleLpn.FechaAlta = DateTime.Now;

                    context.NewDetalleLpn.Add(GetDetalleLpnEntity(detalleLpn));
                }
                else
                {
                    context.NewDetalleLpn.Remove(detalleLpnPedido);
                    detalleLpnPedido.CantidadPedida = (detalleLpnPedido.CantidadPedida ?? 0) + cantidad;
                    context.NewDetalleLpn.Add(detalleLpnPedido);
                }

                var existeAlgunDuplicado = context.NewDuplicados
                    .Any(x => x.Pedido == pedido.Id.Trim()
                        && x.Empresa == lpn.Empresa
                        && x.Producto == detalle.CodigoProducto
                        && x.Cliente == pedido.Cliente
                        && x.Identificador == identificador
                        && x.IdEspecificaIdentificador == especificaLote);

                if (existeAlgunDuplicado)
                {
                    var sistemaExterno = $"LPN-{lpn.Tipo}-{lpn.IdExterno}";

                    var detalleDuplicado = context.NewDuplicados
                        .FirstOrDefault(x => x.Pedido == pedido.Id.Trim()
                            && x.Empresa == lpn.Empresa
                            && x.Producto == detalle.CodigoProducto
                            && x.Cliente == pedido.Cliente
                            && x.Identificador == identificador
                            && x.IdEspecificaIdentificador == especificaLote
                            && x.IdLineaSistemaExterno == sistemaExterno);

                    if (detalleDuplicado == null)
                    {
                        context.NewDuplicados.Add(new DetallePedidoDuplicado()
                        {
                            Cliente = pedido.Cliente,
                            CantidadPedida = cantidad,
                            Empresa = lpn.Empresa,
                            Faixa = 1,
                            Identificador = identificador,
                            Pedido = pedido.Id.Trim(),
                            Producto = detalle.CodigoProducto,
                            IdEspecificaIdentificador = especificaLote,
                            FechaAlta = DateTime.Now,
                            IdLineaSistemaExterno = sistemaExterno,
                            Transaccion = pedido.Transaccion,
                        });
                    }
                    else
                    {
                        context.NewDuplicados.Remove(detalleDuplicado);
                        detalleDuplicado.CantidadPedida += detalle.Cantidad;
                        context.NewDuplicados.Add(detalleDuplicado);
                    }
                }
            }
        }

        public static decimal GetCantidadPedidoLpn(Lpn detalleLpn, LpnDetalle detalle)
        {
            decimal cantidad;

            if (detalleLpn.Estado == EstadosLPN.Activo)
                cantidad = detalle.Cantidad - (detalle.CantidadReserva ?? 0);
            else
                cantidad = (detalle.CantidadRecibida ?? 0) == 0 ? (detalle.CantidadDeclarada ?? 0) : (detalle.CantidadRecibida ?? 0);

            return cantidad;
        }

        public virtual void ModificarDetallePedidoLpn(PedidoInsertBulkOperationContext context, Pedido pedido, Lpn lpn, LpnDetalle detalle, DetallePedido detallePedido)
        {
            context.NewLineas.Remove(detallePedido);

            var cantidad = GetCantidadPedidoLpn(lpn, detalle);

            if (cantidad > 0)
            {
                detallePedido.Cantidad += cantidad;
                context.NewLineas.Add(detallePedido);

                var detalleLpnPedido = context.NewDetalleLpn
                    .FirstOrDefault(x => x.Pedido == pedido.Id
                        && x.Cliente == pedido.Cliente
                        && x.Empresa == pedido.Empresa
                        && x.Producto == detalle.CodigoProducto
                        && x.Faixa == detalle.Faixa
                        && (x.Identificador == ManejoIdentificadorDb.IdentificadorAuto || x.Identificador == detalle.Lote));

                if (detalleLpnPedido == null)
                {
                    var detalleLpn = new DetallePedidoLpn()
                    {
                        Cliente = pedido.Cliente,
                        CantidadPedida = cantidad,
                        Empresa = lpn.Empresa,
                        Faixa = 1,
                        Identificador = detallePedido.Identificador,
                        IdEspecificaIdentificador = detallePedido.EspecificaIdentificadorId,
                        Pedido = pedido.Id.Trim(),
                        Producto = detalle.CodigoProducto,
                        Tipo = lpn.Tipo ?? string.Empty,
                        IdLpnExterno = lpn.IdExterno,
                        CantidadLiberada = 0,
                        CantidadAnulada = 0,
                        NumeroLpn = lpn.NumeroLPN,
                        Transaccion = pedido.Transaccion,
                        FechaAlta = DateTime.Now,
                        FechaModificacion = DateTime.Now,
                    };

                    context.NewDetalleLpn.Add(GetDetalleLpnEntity(detalleLpn));
                }
                else
                {
                    context.NewDetalleLpn.Remove(detalleLpnPedido);

                    detalleLpnPedido.CantidadPedida = (detalleLpnPedido.CantidadPedida ?? 0) + cantidad;
                    detalleLpnPedido.Transaccion = pedido.Transaccion;
                    detalleLpnPedido.FechaModificacion = DateTime.Now;

                    context.NewDetalleLpn.Add(detalleLpnPedido);
                }

                var existeAlgunDuplicado = context.NewDuplicados
                    .Any(x => x.Pedido == pedido.Id.Trim()
                        && x.Empresa == lpn.Empresa
                        && x.Producto == detalle.CodigoProducto
                        && x.Cliente == pedido.Cliente
                        && x.Identificador == detallePedido.Identificador
                        && x.IdEspecificaIdentificador == detallePedido.EspecificaIdentificadorId);

                if (existeAlgunDuplicado)
                {
                    string sistemaExterno = $"LPN-{lpn.Tipo}-{lpn.IdExterno}";
                    var detalleDuplicado = context.NewDuplicados
                        .FirstOrDefault(x => x.Pedido == pedido.Id.Trim()
                            && x.Empresa == lpn.Empresa
                            && x.Producto == detalle.CodigoProducto
                            && x.Cliente == pedido.Cliente
                            && x.Identificador == detallePedido.Identificador
                            && x.IdEspecificaIdentificador == detallePedido.EspecificaIdentificadorId
                            && x.IdLineaSistemaExterno == sistemaExterno);

                    if (detalleDuplicado == null)
                    {
                        context.NewDuplicados.Add(new DetallePedidoDuplicado()
                        {
                            Pedido = pedido.Id.Trim(),
                            Empresa = lpn.Empresa,
                            Cliente = pedido.Cliente,
                            Producto = detalle.CodigoProducto,
                            Faixa = 1,
                            Identificador = detallePedido.Identificador,
                            IdEspecificaIdentificador = detallePedido.EspecificaIdentificadorId,
                            CantidadPedida = cantidad,
                            FechaAlta = DateTime.Now,
                            FechaModificacion = DateTime.Now,
                            IdLineaSistemaExterno = sistemaExterno,
                            Transaccion = pedido.Transaccion,
                        });
                    }
                    else
                    {
                        context.NewDuplicados.Remove(detalleDuplicado);

                        detalleDuplicado.CantidadPedida += cantidad;
                        detalleDuplicado.Transaccion = pedido.Transaccion;
                        detalleDuplicado.FechaModificacion = DateTime.Now;

                        context.NewDuplicados.Add(detalleDuplicado);
                    }
                }
            }
        }

        public virtual DetallePedido GetLineaEntity(DetallePedido detalle)
        {
            return new DetallePedido()
            {
                Id = detalle.Id,
                Empresa = detalle.Empresa,
                Cliente = detalle.Cliente,
                Faixa = detalle.Faixa,
                Producto = detalle.Producto,
                Memo = detalle.Memo,
                FechaAlta = DateTime.Now,
                FechaGenerica_1 = detalle.FechaGenerica_1,
                Agrupacion = detalle.Agrupacion,
                EspecificaIdentificadorId = detalle.EspecificaIdentificadorId,
                NuGenerico_1 = detalle.NuGenerico_1,
                Identificador = detalle.Identificador,
                CantidadAnulada = detalle.CantidadAnulada,
                CantidadLiberada = detalle.CantidadLiberada,
                Cantidad = detalle.Cantidad,
                CantidadOriginal = detalle.CantidadOriginal,
                VlGenerico_1 = detalle.VlGenerico_1,
                PorcentajeTolerancia = detalle.PorcentajeTolerancia,
                DatosSerializados = detalle.DatosSerializados,
                Transaccion = detalle.Transaccion,
            };
        }

        public virtual async Task BulkInsertLineas(DbConnection connection, DbTransaction tran, List<DetallePedido> lineas)
        {
            var sql = @" INSERT INTO T_DET_PEDIDO_SAIDA
                        (NU_PEDIDO,
                        CD_EMPRESA,
                        CD_CLIENTE,
                        CD_FAIXA,
                        CD_PRODUTO,
                        DS_MEMO,
                        DT_ADDROW,
                        DT_GENERICO_1,
                        ID_AGRUPACION,
                        ID_ESPECIFICA_IDENTIFICADOR,
                        NU_GENERICO_1,
                        NU_IDENTIFICADOR,
                        QT_ANULADO,
                        QT_LIBERADO,
                        QT_PEDIDO,
                        QT_PEDIDO_ORIGINAL,
                        VL_GENERICO_1,
                        VL_PORCENTAJE_TOLERANCIA,
                        NU_TRANSACCION,
                        VL_SERIALIZADO_1) 
                        VALUES(
                        :Id,                          
                        :Empresa,                     
                        :Cliente,                     
                        :Faixa,                       
                        :Producto,                    
                        :Memo,                        
                        :FechaAlta,                   
                        :FechaGenerica_1,             
                        :Agrupacion,                  
                        :EspecificaIdentificadorId, 
                        :NuGenerico_1,                
                        :Identificador,               
                        :CantidadAnulada,             
                        :CantidadLiberada,            
                        :Cantidad,                    
                        :CantidadOriginal,            
                        :VlGenerico_1,                
                        :PorcentajeTolerancia,
                        :Transaccion,
                        :DatosSerializados)";

            await _dapper.ExecuteAsync(connection, sql, lineas, transaction: tran);
        }

        public virtual DetallePedidoDuplicado GetDuplicadoEntity(DetallePedidoDuplicado duplicado)
        {
            return new DetallePedidoDuplicado()
            {
                Pedido = duplicado.Pedido,
                Cliente = duplicado.Cliente,
                Empresa = duplicado.Empresa,
                Producto = duplicado.Producto,
                Faixa = duplicado.Faixa,
                Identificador = duplicado.Identificador,
                IdEspecificaIdentificador = duplicado.IdEspecificaIdentificador,
                IdLineaSistemaExterno = duplicado.IdLineaSistemaExterno,
                TipoLinea = duplicado.TipoLinea,
                CantidadPedida = duplicado.CantidadPedida,
                FechaAlta = DateTime.Now,
                DatosSerializados = duplicado.DatosSerializados,
                Transaccion = duplicado.Transaccion,
            };
        }

        public virtual DetallePedidoLpn GetDetalleLpnEntity(DetallePedidoLpn detalle)
        {
            return new DetallePedidoLpn()
            {
                Pedido = detalle.Pedido,
                Empresa = detalle.Empresa,
                Cliente = detalle.Cliente,
                Faixa = detalle.Faixa,
                Producto = detalle.Producto,
                FechaAlta = detalle.FechaAlta,
                FechaModificacion = detalle.FechaModificacion,
                Identificador = detalle.Identificador,
                IdEspecificaIdentificador = detalle.IdEspecificaIdentificador,
                IdLpnExterno = detalle.IdLpnExterno,
                Tipo = detalle.Tipo,
                CantidadPedida = detalle.CantidadPedida,
                CantidadLiberada = detalle.CantidadLiberada,
                CantidadAnulada = detalle.CantidadAnulada,
                Transaccion = detalle.Transaccion,
                NumeroLpn = detalle.NumeroLpn
            };
        }

        public virtual DetallePedidoAtributos GetAtributosEntity(DetallePedidoAtributos configuracion)
        {
            return new DetallePedidoAtributos()
            {
                IdConfiguracion = configuracion.IdConfiguracion,
                Pedido = configuracion.Pedido,
                Cliente = configuracion.Cliente,
                Empresa = configuracion.Empresa,
                Producto = configuracion.Producto,
                Faixa = configuracion.Faixa,
                Identificador = configuracion.Identificador,
                IdEspecificaIdentificador = configuracion.IdEspecificaIdentificador,
                FechaAlta = configuracion.FechaAlta,
                FechaModificacion = DateTime.Now,
                CantidadPedida = configuracion.CantidadPedida,
                CantidadLiberada = 0,
                CantidadAnulada = 0,
                Transaccion = configuracion.Transaccion,
                InternalId = configuracion.InternalId,
            };
        }

        public virtual DetallePedidoAtributosLpn GetAtributosLpnEntity(DetallePedidoAtributosLpn configuracion)
        {
            return new DetallePedidoAtributosLpn()
            {
                IdConfiguracion = configuracion.IdConfiguracion,
                Pedido = configuracion.Pedido,
                Cliente = configuracion.Cliente,
                Empresa = configuracion.Empresa,
                Producto = configuracion.Producto,
                Faixa = configuracion.Faixa,
                Identificador = configuracion.Identificador,
                IdEspecificaIdentificador = configuracion.IdEspecificaIdentificador,
                FechaAlta = configuracion.FechaAlta,
                FechaModificacion = DateTime.Now,
                CantidadPedida = configuracion.CantidadPedida,
                CantidadLiberada = 0,
                CantidadAnulada = 0,
                Transaccion = configuracion.Transaccion,
                InternalId = configuracion.InternalId,
                IdLpnExterno = configuracion.IdLpnExterno,
                Tipo = configuracion.Tipo,
            };
        }

        public virtual DetallePedidoConfigAtributo GetAtributoEntity(DetallePedidoConfigAtributo atributo)
        {
            return new DetallePedidoConfigAtributo()
            {
                IdConfiguracion = atributo.IdConfiguracion,
                IdAtributo = atributo.IdAtributo,
                IdCabezal = atributo.IdCabezal,
                Valor = atributo.Valor,
                FechaAlta = atributo.FechaAlta,
                FechaModificacion = DateTime.Now,
                Transaccion = atributo.Transaccion,
                Tipo = atributo.Tipo,
                InternalId = atributo.InternalId,
            };
        }

        public virtual async Task BulkInsertDetallesLpnAsync(DbConnection connection, DbTransaction tran, List<DetallePedidoLpn> detallesLpn)
        {
            string sql = GetSqlInsertDetallesLpn();

            await _dapper.ExecuteAsync(connection, sql, detallesLpn, transaction: tran);
        }

        public virtual string GetSqlInsertDetallesLpn()
        {
            return @" INSERT INTO T_DET_PEDIDO_SAIDA_LPN (
                                NU_PEDIDO,
                                CD_CLIENTE,
                                CD_EMPRESA,
                                CD_PRODUTO,
                                CD_FAIXA,
                                NU_IDENTIFICADOR,
                                ID_ESPECIFICA_IDENTIFICADOR,
                                QT_PEDIDO,
                                DT_ADDROW,
                                DT_UPDROW,
                                NU_TRANSACCION,
                                ID_LPN_EXTERNO,
                                TP_LPN_TIPO,
                                QT_LIBERADO,
                                QT_ANULADO,
                                NU_LPN) 
                        VALUES (
                                :Pedido,
                                :Cliente,
                                :Empresa,
                                :Producto,
                                :Faixa,
                                :Identificador,
                                :IdEspecificaIdentificador,
                                :CantidadPedida,
                                :FechaAlta,
                                :FechaModificacion,
                                :Transaccion,
                                :IdLpnExterno,
                                :Tipo,
                                :CantidadLiberada,
                                :CantidadAnulada,
                                :NumeroLpn)";
        }

        public virtual async Task BulkInsertAtributos(DbConnection connection, DbTransaction tran, List<DetallePedidoAtributos> atributos)
        {
            var sql = @" INSERT INTO T_DET_PEDIDO_SAIDA_ATRIB (
                                NU_PEDIDO,
                                CD_CLIENTE,
                                CD_EMPRESA,
                                CD_PRODUTO,
                                CD_FAIXA,
                                NU_IDENTIFICADOR,
                                ID_ESPECIFICA_IDENTIFICADOR,
                                NU_DET_PED_SAI_ATRIB,
                                QT_PEDIDO,
                                DT_ADDROW,
                                DT_UPDROW,
                                NU_TRANSACCION,
                                QT_LIBERADO,
                                QT_ANULADO) 
                        VALUES (
                                :Pedido,
                                :Cliente,
                                :Empresa,
                                :Producto,
                                :Faixa,
                                :Identificador,
                                :IdEspecificaIdentificador,
                                :IdConfiguracion,
                                :CantidadPedida,
                                :FechaAlta,
                                :FechaModificacion,
                                :Transaccion,
                                :CantidadLiberada,
                                :CantidadAnulada)";

            await _dapper.ExecuteAsync(connection, sql, atributos, transaction: tran);
        }

        public virtual async Task BulkInsertAtributosDetalle(DbConnection connection, DbTransaction tran, List<DetallePedidoConfigAtributo> atributos)
        {
            var sql = @" INSERT INTO T_DET_PEDIDO_SAIDA_ATRIB_DET (
                                NU_DET_PED_SAI_ATRIB,
                                ID_ATRIBUTO,
                                FL_CABEZAL,
                                VL_ATRIBUTO,
                                DT_ADDROW,
                                DT_UPDROW,
                                NU_TRANSACCION) 
                        VALUES (
                                :IdConfiguracion,
                                :IdAtributo,
                                :IdCabezal,
                                :Valor,
                                :FechaAlta,
                                :FechaModificacion,
                                :Transaccion)";

            await _dapper.ExecuteAsync(connection, sql, atributos, transaction: tran);
        }

        public virtual async Task BulkInsertDuplicados(DbConnection connection, DbTransaction tran, List<DetallePedidoDuplicado> duplicados)
        {
            var sql = @" INSERT INTO T_DET_PEDIDO_SAIDA_DUP
                        (NU_PEDIDO,
                        CD_CLIENTE,
                        CD_EMPRESA,
                        CD_PRODUTO, 
                        CD_FAIXA,
                        NU_IDENTIFICADOR,                       
                        ID_ESPECIFICA_IDENTIFICADOR,
                        ID_LINEA_SISTEMA_EXTERNO,
                        TP_LINEA,
                        QT_PEDIDO,
                        DT_ADDROW,
                        NU_TRANSACCION,
                        VL_SERIALIZADO_1) 
                        VALUES(
                        :Pedido,
                        :Cliente,
                        :Empresa,                     
                        :Producto,                      
                        :Faixa,                       
                        :Identificador, 
                        :IdEspecificaIdentificador, 
                        :IdLineaSistemaExterno,
                        :TipoLinea,
                        :CantidadPedida,               
                        :FechaAlta,
                        :Transaccion,
                        :DatosSerializados)";

            await _dapper.ExecuteAsync(connection, sql, duplicados, transaction: tran);
        }

        public virtual object GetPedidoEntity(Pedido pedido)
        {
            return new
            {
                Id = pedido.Id,
                Empresa = pedido.Empresa,
                Cliente = pedido.Cliente,
                CondicionLiberacion = pedido.CondicionLiberacion,
                Origen = pedido.Origen,
                PuntoEntrega = pedido.PuntoEntrega,
                Ruta = pedido.Ruta,
                Situacion = pedido.Estado,
                CodigoTransportadora = pedido.CodigoTransportadora,
                Zona = pedido.Zona,
                Anexo = pedido.Anexo,
                Anexo2 = pedido.Anexo2,
                Anexo3 = pedido.Anexo3,
                Anexo4 = pedido.Anexo4,
                DireccionEntrega = pedido.DireccionEntrega,
                Memo = pedido.Memo,
                Memo1 = pedido.Memo1,
                FechaAlta = DateTime.Now,
                FechaEmision = pedido.FechaEmision,
                FechaEntrega = pedido.FechaEntrega,
                FechaGenerica_1 = pedido.FechaGenerica_1,
                FechaLiberarDesde = pedido.FechaLiberarDesde,
                FechaLiberarHasta = pedido.FechaLiberarHasta,
                SincronizacionRealizada = pedido.SincronizacionRealizadaId,
                Agrupacion = pedido.Agrupacion,
                IdManual = pedido.ManualId,
                NuGenerico_1 = pedido.NuGenerico_1,
                OrdenEntrega = pedido.OrdenEntrega,
                Predio = pedido.Predio,
                TipoExpedicion = pedido.TipoExpedicionId,
                Tipo = pedido.Tipo,
                ComparteContenedorEntrega = pedido.ComparteContenedorEntrega,
                ComparteContenedorPicking = pedido.ComparteContenedorPicking,
                VlGenerico_1 = pedido.VlGenerico_1,
                VlSerealizado_1 = pedido.VlSerealizado_1,
                Telefono = pedido.Telefono,
                TelefonoSecundario = pedido.TelefonoSecundario,
                Longitud = pedido.Longitud,
                Latitud = pedido.Latitud,
                Transaccion = pedido.Transaccion,
                Actividad = pedido.Actividad,
            };
        }

        public virtual async Task BulkInsertPedidos(DbConnection connection, DbTransaction tran, List<object> pedidos)
        {
            string sql = GetSqlInsertPedido();

            await _dapper.ExecuteAsync(connection, sql, pedidos, transaction: tran);
        }

        public virtual string GetSqlInsertPedido()
        {
            return @"INSERT INTO T_PEDIDO_SAIDA 
                        (NU_PEDIDO, 
                        CD_EMPRESA,
                        CD_CLIENTE,
                        CD_CONDICION_LIBERACION,
                        CD_ORIGEN,
                        CD_PUNTO_ENTREGA,
                        CD_ROTA,
                        CD_SITUACAO,
                        CD_TRANSPORTADORA,
                        CD_ZONA,
                        DS_ANEXO1,
                        DS_ANEXO2,
                        DS_ANEXO3,
                        DS_ANEXO4,
                        DS_ENDERECO,
                        DS_MEMO,
                        DS_MEMO_1,
                        DT_ADDROW,
                        DT_EMITIDO,
                        DT_ENTREGA,
                        DT_GENERICO_1,
                        DT_LIBERAR_DESDE,
                        DT_LIBERAR_HASTA,
                        FL_SYNC_REALIZADA,
                        ID_AGRUPACION,
                        ID_MANUAL,
                        NU_GENERICO_1,
                        NU_ORDEN_ENTREGA,
                        NU_PREDIO,
                        TP_EXPEDICION,
                        TP_PEDIDO,
                        VL_COMPARTE_CONTENEDOR_ENTREGA,
                        VL_COMPARTE_CONTENEDOR_PICKING,
                        VL_GENERICO_1,
                        NU_TELEFONE,
                        NU_TELEFONE2,
                        VL_LONGITUD,
                        VL_LATITUD, 
                        NU_TRANSACCION,
                        VL_SERIALIZADO_1,
                        ND_ACTIVIDAD) 
                        VALUES (
                        :Id,                          
                        :Empresa,                     
                        :Cliente,                     
                        :CondicionLiberacion,         
                        :Origen,                      
                        :PuntoEntrega,                
                        :Ruta,                        
                        :Situacion,               
                        :CodigoTransportadora,        
                        :Zona,                        
                        :Anexo,                       
                        :Anexo2,                      
                        :Anexo3,                      
                        :Anexo4,                      
                        :DireccionEntrega,            
                        :Memo,                        
                        :Memo1,                       
                        :FechaAlta,                           
                        :FechaEmision,                
                        :FechaEntrega,                
                        :FechaGenerica_1,             
                        :FechaLiberarDesde,           
                        :FechaLiberarHasta,           
                        :SincronizacionRealizada, 
                        :Agrupacion,                  
                        :IdManual,                
                        :NuGenerico_1,                
                        :OrdenEntrega,                
                        :Predio,                      
                        :TipoExpedicion,          
                        :Tipo,                        
                        :ComparteContenedorEntrega,   
                        :ComparteContenedorPicking,   
                        :VlGenerico_1,                
                        :Telefono,
                        :TelefonoSecundario,
                        :Longitud,
                        :Latitud,
                        :Transaccion,
                        :VlSerealizado_1,
                        :Actividad)";
        }

        #endregion

        #region ModificarPedidos

        public virtual async Task ModificarPedidos(List<Pedido> pedidos, IModificarPedidoServiceContext context, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var bulkContext = GetBulkUpdateOperationContext(pedidos, context, connection);

                using (var tran = connection.BeginTransaction())
                {
                    await BulkUpdatePedidos(connection, tran, bulkContext.UpdPedidos);

                    await BulkInsertLineas(connection, tran, bulkContext.NewDetalles);
                    await BulkUpdateDetalles(connection, tran, bulkContext.UpdDetalles);

                    await BulkInsertDuplicados(connection, tran, bulkContext.NewDuplicados);
                    await BulkUpdateDuplicados(connection, tran, bulkContext.UpdDuplicados);

                    await BulkBeforeDeleteAtributosDetalle(connection, tran, bulkContext.DeleteAtributosDetalle);
                    await BulkDeleteAtributosDetalle(connection, tran, bulkContext.DeleteAtributosDetalle);

                    await BulkBeforeDeleteAtributos(connection, tran, bulkContext.DeleteAtributos);
                    await BulkDeleteAtributos(connection, tran, bulkContext.DeleteAtributos);

                    await BulkBeforeDeleteAtributosLpn(connection, tran, bulkContext.DeleteAtributosLpn);
                    await BulkDeleteAtributosLpn(connection, tran, bulkContext.DeleteAtributosLpn);

                    await BulkBeforeDeleteDetalleLpn(connection, tran, bulkContext.DeleteDetalleLpn);
                    await BulkDeleteDetalleLpn(connection, tran, bulkContext.DeleteDetalleLpn);

                    await BulkInsertDetallesLpnAsync(connection, tran, bulkContext.NewDetalleLpn);
                    await BulkUpdateDetalleLpn(connection, tran, bulkContext.UpdateDetalleLpn);

                    await BulkInsertAtributos(connection, tran, bulkContext.NewAtributos);
                    await BulkUpdateAtributos(connection, tran, bulkContext.UpdateAtributos);

                    await BulkInsertAtributosLpn(connection, tran, bulkContext.NewAtributosLpn);
                    await BulkUpdateAtributosLpn(connection, tran, bulkContext.UpdateAtributosLpn);
                    await BulkInsertAtributosDetalle(connection, tran, bulkContext.NewAtributosDetalle);

                    tran.Commit();
                }
            }
        }

        public virtual async Task BulkInsertAtributosLpn(DbConnection connection, DbTransaction tran, List<DetallePedidoAtributosLpn> atributos)
        {
            var sql = @" INSERT INTO T_DET_PEDIDO_SAIDA_LPN_ATRIB (
                                NU_PEDIDO,
                                CD_CLIENTE,
                                CD_EMPRESA,
                                CD_PRODUTO,
                                CD_FAIXA,
                                NU_IDENTIFICADOR,
                                ID_ESPECIFICA_IDENTIFICADOR,
                                NU_DET_PED_SAI_ATRIB,
                                QT_PEDIDO,
                                DT_ADDROW,
                                DT_UPDROW,
                                ID_LPN_EXTERNO,
                                TP_LPN_TIPO,
                                NU_TRANSACCION,
                                QT_LIBERADO,
                                QT_ANULADO) 
                        VALUES (
                                :Pedido,
                                :Cliente,
                                :Empresa,
                                :Producto,
                                :Faixa,
                                :Identificador,
                                :IdEspecificaIdentificador,
                                :IdConfiguracion,
                                :CantidadPedida,
                                :FechaAlta,
                                :FechaModificacion,
                                :IdLpnExterno,
                                :Tipo,
                                :Transaccion,
                                :CantidadLiberada,
                                :CantidadAnulada)";

            await _dapper.ExecuteAsync(connection, sql, atributos, transaction: tran);
        }

        public virtual async Task BulkUpdateAtributosLpn(DbConnection connection, DbTransaction tran, List<DetallePedidoAtributosLpn> atributos)
        {
            var sql = @" 
                UPDATE T_DET_PEDIDO_SAIDA_LPN_ATRIB 
                SET QT_PEDIDO = :CantidadPedida,
                    DT_UPDROW = :FechaModificacion,
                    NU_TRANSACCION = :Transaccion
                WHERE NU_PEDIDO = :Pedido
                    AND CD_CLIENTE = :Cliente
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto
                    AND CD_FAIXA = :Faixa
                    AND NU_IDENTIFICADOR = :Identificador
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                    AND ID_LPN_EXTERNO = :IdLpnExterno
                    AND TP_LPN_TIPO = :Tipo
                    AND NU_DET_PED_SAI_ATRIB = :IdConfiguracion ";

            await _dapper.ExecuteAsync(connection, sql, atributos, transaction: tran);
        }

        public virtual async Task BulkDeleteAtributosLpn(DbConnection connection, DbTransaction tran, List<DetallePedidoAtributosLpn> atributos)
        {
            var sql = @" 
                DELETE T_DET_PEDIDO_SAIDA_LPN_ATRIB 
                WHERE NU_PEDIDO = :Pedido
                    AND CD_CLIENTE = :Cliente
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto
                    AND CD_FAIXA = :Faixa
                    AND NU_IDENTIFICADOR = :Identificador
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                    AND ID_LPN_EXTERNO = :IdLpnExterno
                    AND TP_LPN_TIPO = :Tipo
                    AND NU_DET_PED_SAI_ATRIB = :IdConfiguracion ";

            await _dapper.ExecuteAsync(connection, sql, atributos, transaction: tran);
        }

        public virtual async Task BulkUpdateAtributos(DbConnection connection, DbTransaction tran, List<DetallePedidoAtributos> atributos)
        {
            var sql = @" 
                UPDATE T_DET_PEDIDO_SAIDA_ATRIB 
                SET QT_PEDIDO = :CantidadPedida,
                    DT_UPDROW = :FechaModificacion,
                    NU_TRANSACCION = :Transaccion
                WHERE NU_PEDIDO = :Pedido
                    AND CD_CLIENTE = :Cliente
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto
                    AND CD_FAIXA = :Faixa
                    AND NU_IDENTIFICADOR = :Identificador
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                    AND NU_DET_PED_SAI_ATRIB = :IdConfiguracion ";

            await _dapper.ExecuteAsync(connection, sql, atributos, transaction: tran);
        }

        public virtual async Task BulkBeforeDeleteAtributos(DbConnection connection, DbTransaction tran, List<DetallePedidoAtributos> atributos)
        {
            var sql = @" 
                UPDATE T_DET_PEDIDO_SAIDA_ATRIB 
                SET DT_UPDROW = :FechaModificacion,
                    NU_TRANSACCION = :Transaccion,
                    NU_TRANSACCION_DELETE = :Transaccion
                WHERE NU_PEDIDO = :Pedido
                    AND CD_CLIENTE = :Cliente
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto
                    AND CD_FAIXA = :Faixa
                    AND NU_IDENTIFICADOR = :Identificador
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                    AND NU_DET_PED_SAI_ATRIB = :IdConfiguracion ";

            await _dapper.ExecuteAsync(connection, sql, atributos, transaction: tran);
        }

        public virtual async Task BulkBeforeDeleteAtributosLpn(DbConnection connection, DbTransaction tran, List<DetallePedidoAtributosLpn> atributos)
        {
            var sql = @" 
                UPDATE T_DET_PEDIDO_SAIDA_LPN_ATRIB 
                SET DT_UPDROW = :FechaModificacion,
                    NU_TRANSACCION = :Transaccion,
                    NU_TRANSACCION_DELETE = :Transaccion
                WHERE NU_PEDIDO = :Pedido
                    AND CD_CLIENTE = :Cliente
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto
                    AND CD_FAIXA = :Faixa
                    AND NU_IDENTIFICADOR = :Identificador
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                    AND ID_LPN_EXTERNO = :IdLpnExterno
                    AND TP_LPN_TIPO = :Tipo
                    AND NU_DET_PED_SAI_ATRIB = :IdConfiguracion ";

            await _dapper.ExecuteAsync(connection, sql, atributos, transaction: tran);
        }

        public virtual async Task BulkDeleteAtributos(DbConnection connection, DbTransaction tran, List<DetallePedidoAtributos> atributos)
        {
            var sql = @" 
                DELETE T_DET_PEDIDO_SAIDA_ATRIB 
                WHERE NU_PEDIDO = :Pedido
                    AND CD_CLIENTE = :Cliente
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto
                    AND CD_FAIXA = :Faixa
                    AND NU_IDENTIFICADOR = :Identificador
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                    AND NU_DET_PED_SAI_ATRIB = :IdConfiguracion ";

            await _dapper.ExecuteAsync(connection, sql, atributos, transaction: tran);
        }

        public virtual async Task BulkBeforeDeleteAtributosDetalle(DbConnection connection, DbTransaction tran, List<DetallePedidoConfigAtributo> atributos)
        {
            var sql = @" 
                UPDATE T_DET_PEDIDO_SAIDA_ATRIB_DET
                SET DT_UPDROW = :FechaModificacion,
                    NU_TRANSACCION = :Transaccion,
                    NU_TRANSACCION_DELETE = :Transaccion
                WHERE NU_DET_PED_SAI_ATRIB = :IdConfiguracion 
                    AND ID_ATRIBUTO = :IdAtributo 
                    AND FL_CABEZAL = :IdCabezal ";

            await _dapper.ExecuteAsync(connection, sql, atributos, transaction: tran);
        }

        public virtual async Task BulkDeleteAtributosDetalle(DbConnection connection, DbTransaction tran, List<DetallePedidoConfigAtributo> atributos)
        {
            var sql = @" 
                DELETE T_DET_PEDIDO_SAIDA_ATRIB_DET
                WHERE NU_DET_PED_SAI_ATRIB = :IdConfiguracion 
                    AND ID_ATRIBUTO = :IdAtributo 
                    AND FL_CABEZAL = :IdCabezal ";

            await _dapper.ExecuteAsync(connection, sql, atributos, transaction: tran);
        }

        public virtual PedidoUpdateBulkOperationContext GetBulkUpdateOperationContext(List<Pedido> pedidos, IModificarPedidoServiceContext serviceContext, DbConnection connection)
        {
            var context = new PedidoUpdateBulkOperationContext();

            foreach (var pedido in pedidos)
            {
                var model = serviceContext.GetPedido(pedido.Id, pedido.Empresa, pedido.Cliente);
                var ped = Map(pedido, model);
                context.UpdPedidos.Add(GetPedidoEntityUpdate(ped));

                foreach (var detalle in pedido.Lineas)
                {
                    detalle.Transaccion = pedido.Transaccion;

                    var modelDet = serviceContext.GetDetallePedido(detalle);
                    if (modelDet != null)
                    {
                        var det = MapDetalle(detalle, modelDet);
                        context.UpdDetalles.Add(GetDetallePedidoEntityUpdate(det));
                    }
                    else
                    {
                        detalle.Agrupacion = ped.Agrupacion;
                        context.NewDetalles.Add(GetLineaEntity(detalle));
                    }

                    foreach (var duplicado in detalle.Duplicados)
                    {
                        duplicado.Transaccion = detalle.Transaccion;

                        var modelDup = serviceContext.GetDuplicado(duplicado);
                        if (modelDup != null)
                        {
                            var dup = MapDuplicado(duplicado, modelDup);
                            context.UpdDuplicados.Add(GetDuplicadoEntityUpdate(dup));
                        }
                        else
                            context.NewDuplicados.Add(GetDuplicadoEntity(duplicado));
                    }

                    foreach (var detalleLpn in detalle.DetallesLpn)
                    {
                        detalleLpn.Transaccion = detalle.Transaccion;

                        var modelDetLpn = serviceContext.GetDetalleLpn(detalleLpn);
                        if (modelDetLpn != null)
                        {
                            var detLpn = MapDetalleLpn(detalleLpn, modelDetLpn);
                            context.UpdateDetalleLpn.Add(GetDetalleLpnEntityUpdate(detLpn));

                            foreach (var configuracion in detalleLpn.Atributos)
                            {
                                var modelConf = serviceContext.GetAtributos(configuracion);

                                configuracion.Transaccion = detalle.Transaccion;

                                if (modelConf != null)
                                {
                                    var conf = MapAtributos(configuracion, modelConf);
                                    context.UpdateAtributosLpn.Add(GetAtributosLpnEntity(conf));
                                }
                                else
                                {
                                    configuracion.InternalId = Guid.NewGuid();
                                    configuracion.FechaAlta = DateTime.Now;

                                    context.NewAtributosLpn.Add(GetAtributosLpnEntity(configuracion));

                                    foreach (var atributo in configuracion.Atributos)
                                    {
                                        atributo.InternalId = configuracion.InternalId;
                                        atributo.IdAtributo = serviceContext.GetAtributo(atributo.Nombre).Id;
                                        atributo.Transaccion = detalle.Transaccion;
                                        atributo.FechaAlta = DateTime.Now;

                                        context.NewAtributosDetalle.Add(GetAtributoEntity(atributo));
                                    }
                                }
                            }

                            foreach (var configuracion in serviceContext.GetAtributosLpnRegistrados(detalleLpn)
                                .Where(d => (d.CantidadLiberada ?? 0) == 0 && (d.CantidadAnulada ?? 0) == 0))

                            {
                                if (!detalleLpn.Atributos
                                    .Any(x => serviceContext.GetJson(x) == serviceContext.GetJson(configuracion)))
                                {
                                    configuracion.Transaccion = detalle.Transaccion;

                                    context.DeleteAtributosLpn.Add(GetAtributosLpnEntity(configuracion));

                                    foreach (var atributo in configuracion.Atributos)
                                    {
                                        atributo.Transaccion = detalle.Transaccion;

                                        context.DeleteAtributosDetalle.Add(GetAtributoEntity(atributo));
                                    }
                                }
                            }
                        }
                        else
                        {
                            context.NewDetalleLpn.Add(GetDetalleLpnEntity(detalleLpn));

                            foreach (var configuracion in detalleLpn.Atributos)
                            {
                                var modelConf = serviceContext.GetAtributos(configuracion);

                                configuracion.Transaccion = detalle.Transaccion;
                                configuracion.InternalId = Guid.NewGuid();
                                configuracion.FechaAlta = DateTime.Now;

                                context.NewAtributosLpn.Add(GetAtributosLpnEntity(configuracion));

                                foreach (var atributo in configuracion.Atributos)
                                {
                                    atributo.InternalId = configuracion.InternalId;
                                    atributo.IdAtributo = serviceContext.GetAtributo(atributo.Nombre).Id;
                                    atributo.Transaccion = detalle.Transaccion;
                                    atributo.FechaAlta = DateTime.Now;

                                    context.NewAtributosDetalle.Add(GetAtributoEntity(atributo));
                                }
                            }
                        }
                    }

                    foreach (var detalleLpn in serviceContext.GetDetallesLpn(pedido)
                        .Where(d => (d.CantidadLiberada ?? 0) == 0 && (d.CantidadAnulada ?? 0) == 0))
                    {
                        if (!detalle.DetallesLpn
                            .Any(x => x.Pedido == detalleLpn.Pedido
                                && x.Empresa == detalleLpn.Empresa
                                && x.Cliente == detalleLpn.Cliente
                                && x.Producto == detalleLpn.Producto
                                && x.Identificador == detalleLpn.Identificador
                                && x.IdLpnExterno == detalleLpn.IdLpnExterno
                                && x.Tipo == detalleLpn.Tipo
                                ))
                        {
                            detalleLpn.Transaccion = detalle.Transaccion;

                            context.DeleteDetalleLpn.Add(GetDetalleLpnEntity(detalleLpn));

                            foreach (var configuracion in serviceContext.GetAtributosLpnRegistrados(detalleLpn))
                            {
                                var modelConf = serviceContext.GetAtributos(configuracion);

                                configuracion.Transaccion = detalle.Transaccion;

                                context.DeleteAtributosLpn.Add(GetAtributosLpnEntity(configuracion));

                                foreach (var atributo in configuracion.Atributos)
                                {
                                    atributo.IdAtributo = serviceContext.GetAtributo(atributo.Nombre).Id;
                                    atributo.Transaccion = detalle.Transaccion;

                                    context.DeleteAtributosDetalle.Add(GetAtributoEntity(atributo));
                                }
                            }
                        }
                    }

                    foreach (var configuracion in detalle.Atributos)
                    {
                        var modelConf = serviceContext.GetAtributos(configuracion);

                        configuracion.Transaccion = detalle.Transaccion;

                        if (modelConf != null)
                        {
                            var conf = MapAtributos(configuracion, modelConf);
                            context.UpdateAtributos.Add(GetAtributosEntity(conf));
                        }
                        else
                        {
                            configuracion.InternalId = Guid.NewGuid();
                            configuracion.FechaAlta = DateTime.Now;

                            context.NewAtributos.Add(GetAtributosEntity(configuracion));

                            foreach (var atributo in configuracion.Atributos)
                            {
                                atributo.InternalId = configuracion.InternalId;
                                atributo.IdAtributo = serviceContext.GetAtributo(atributo.Nombre).Id;
                                atributo.Transaccion = detalle.Transaccion;
                                atributo.FechaAlta = DateTime.Now;

                                context.NewAtributosDetalle.Add(GetAtributoEntity(atributo));
                            }
                        }
                    }

                    foreach (var configuracion in serviceContext.GetAtributosRegistrados(detalle)
                        .Where(d => (d.CantidadLiberada ?? 0) == 0 && (d.CantidadAnulada ?? 0) == 0))
                    {
                        if (!detalle.Atributos
                            .Any(x => serviceContext.GetJson(x) == serviceContext.GetJson(configuracion)))
                        {
                            configuracion.Transaccion = detalle.Transaccion;

                            context.DeleteAtributos.Add(GetAtributosEntity(configuracion));

                            foreach (var atributo in configuracion.Atributos)
                            {
                                atributo.Transaccion = detalle.Transaccion;

                                context.DeleteAtributosDetalle.Add(GetAtributoEntity(atributo));
                            }
                        }
                    }
                }

                foreach (var lpnPedido in pedido.Lpns)
                {
                    var lpn = serviceContext.GetLpn(lpnPedido.IdExterno, lpnPedido.Tipo);
                    var detalles = serviceContext.GetDetallesLpn(lpn.NumeroLPN);
                    var detalleLpnAgrupado = detalles.GroupBy(x => new { x.NumeroLPN, x.IdExterno, x.Tipo, x.CodigoProducto, x.Faixa, x.Lote })
                        .Select(x => new LpnDetalle()
                        {
                            NumeroLPN = x.Key.NumeroLPN,
                            IdExterno = x.Key.IdExterno,
                            Tipo = x.Key.Tipo,
                            CodigoProducto = x.Key.CodigoProducto,
                            Faixa = x.Key.Faixa,
                            Lote = x.Key.Lote,
                            Cantidad = x.Sum(d => d.Cantidad),
                            CantidadReserva = x.Sum(d => d.CantidadReserva),
                            CantidadDeclarada = x.Sum(d => d.CantidadDeclarada),
                            CantidadRecibida = x.Sum(d => d.CantidadRecibida)
                        })
                        .ToList();

                    foreach (var detalle in detalleLpnAgrupado)
                    {
                        bool isNew = true;
                        var detallePedido = context.NewDetalles
                            .FirstOrDefault(x => x.Id == pedido.Id
                                && x.Cliente == pedido.Cliente
                                && x.Empresa == pedido.Empresa
                                && x.Producto == detalle.CodigoProducto
                                && x.Faixa == detalle.Faixa
                                && (x.Identificador == ManejoIdentificadorDb.IdentificadorAuto || x.Identificador == detalle.Lote));

                        if (detallePedido == null)
                        {
                            isNew = false;
                            detallePedido = context.UpdDetalles
                                .FirstOrDefault(x => x.Id == pedido.Id
                                    && x.Cliente == pedido.Cliente
                                    && x.Empresa == pedido.Empresa
                                    && x.Producto == detalle.CodigoProducto
                                    && x.Faixa == detalle.Faixa
                                    && (x.Identificador == ManejoIdentificadorDb.IdentificadorAuto || x.Identificador == detalle.Lote));
                        }

                        if (detallePedido != null)
                            ModificarDetallePedidoLpn(context, pedido, lpn, detalle, detallePedido, isNew ? context.NewDetalles : context.UpdDetalles);
                        else
                            AgregarDetallePedidoLpn(context, pedido, lpn, detalle, detallePedido);
                    }
                }
            }

            var totalNewAtributos = context.NewAtributos.Count + context.NewAtributosLpn.Count;

            if (totalNewAtributos > 0)
            {
                var idConfiguracionByInternalId = new Dictionary<Guid, long>();
                var configuracionIds = GetNewConfiguracionAtributoIds(totalNewAtributos, connection);

                for (int i = 0; i < context.NewAtributos.Count; i++)
                {
                    var configuracion = context.NewAtributos[i];
                    configuracion.IdConfiguracion = configuracionIds[i];
                    idConfiguracionByInternalId[configuracion.InternalId] = configuracionIds[i];
                }

                for (int i = 0; i < context.NewAtributosLpn.Count; i++)
                {
                    var configuracion = context.NewAtributosLpn[i];
                    configuracion.IdConfiguracion = configuracionIds[i + context.NewAtributos.Count];
                    idConfiguracionByInternalId[configuracion.InternalId] = configuracion.IdConfiguracion;
                }

                foreach (var atributo in context.NewAtributosDetalle)
                {
                    atributo.IdConfiguracion = idConfiguracionByInternalId[atributo.InternalId];
                }
            }

            return context;
        }

        public virtual DetallePedidoAtributos MapAtributos(DetallePedidoAtributos request, DetallePedidoAtributos model)
        {
            DetallePedidoAtributos atributos = new DetallePedidoAtributos();

            atributos.Pedido = model.Pedido;
            atributos.Empresa = model.Empresa;
            atributos.Cliente = model.Cliente;
            atributos.Faixa = model.Faixa;
            atributos.Producto = model.Producto;
            atributos.Identificador = model.Identificador;
            atributos.IdEspecificaIdentificador = model.IdEspecificaIdentificador;
            atributos.FechaAlta = model.FechaAlta;
            atributos.FechaModificacion = DateTime.Now;
            atributos.IdConfiguracion = model.IdConfiguracion;
            atributos.CantidadPedida = request.CantidadPedida;

            return atributos;
        }

        public virtual DetallePedidoAtributosLpn MapAtributos(DetallePedidoAtributosLpn request, DetallePedidoAtributosLpn model)
        {
            DetallePedidoAtributosLpn atributos = new DetallePedidoAtributosLpn();

            atributos.Pedido = model.Pedido;
            atributos.Empresa = model.Empresa;
            atributos.Cliente = model.Cliente;
            atributos.Faixa = model.Faixa;
            atributos.Producto = model.Producto;
            atributos.Identificador = model.Identificador;
            atributos.IdEspecificaIdentificador = model.IdEspecificaIdentificador;
            atributos.FechaAlta = model.FechaAlta;
            atributos.FechaModificacion = DateTime.Now;
            atributos.IdLpnExterno = model.IdLpnExterno;
            atributos.Tipo = model.Tipo;
            atributos.IdConfiguracion = model.IdConfiguracion;
            atributos.CantidadPedida = request.CantidadPedida;

            return atributos;
        }

        public virtual void ModificarDetallePedidoLpn(PedidoUpdateBulkOperationContext context, Pedido pedido, Lpn lpn, LpnDetalle detalle, DetallePedido detallePedido, List<DetallePedido> contextDetallePedido)
        {
            contextDetallePedido.Remove(detallePedido);

            decimal cantidad = GetCantidadPedidoLpn(lpn, detalle);

            if (cantidad > 0)
            {
                detallePedido.Cantidad += cantidad;
                contextDetallePedido.Add(detallePedido);

                var detalleLpnPedido = context.NewDetalleLpn
                    .FirstOrDefault(x => x.Pedido == pedido.Id
                        && x.Cliente == pedido.Cliente
                        && x.Empresa == pedido.Empresa
                        && x.Producto == detalle.CodigoProducto
                        && x.Faixa == detalle.Faixa
                        && (x.Identificador == ManejoIdentificadorDb.IdentificadorAuto || x.Identificador == detalle.Lote)
                        && x.IdLpnExterno == lpn.IdExterno
                        && x.Tipo == lpn.Tipo);

                if (detalleLpnPedido == null)
                {
                    var detalleLpn = new DetallePedidoLpn()
                    {
                        Cliente = pedido.Cliente,
                        CantidadPedida = cantidad,
                        Empresa = lpn.Empresa,
                        Faixa = 1,
                        Identificador = detallePedido.Identificador,
                        IdEspecificaIdentificador = detallePedido.Identificador == ManejoIdentificadorDb.IdentificadorAuto ? "N" : "S",
                        Pedido = pedido.Id.Trim(),
                        Producto = detalle.CodigoProducto,
                        Tipo = lpn.Tipo,
                        IdLpnExterno = lpn.IdExterno,
                        CantidadLiberada = 0,
                        CantidadAnulada = 0,
                        NumeroLpn = lpn.NumeroLPN
                    };

                    detalleLpn.Transaccion = pedido.Transaccion;
                    detalleLpn.FechaAlta = DateTime.Now;

                    context.NewDetalleLpn.Add(GetDetalleLpnEntity(detalleLpn));
                }
                else
                {
                    context.NewDetalleLpn.Remove(detalleLpnPedido);
                    detalleLpnPedido.CantidadPedida = (detalleLpnPedido.CantidadPedida ?? 0) + cantidad;
                    context.NewDetalleLpn.Add(detalleLpnPedido);
                }

                var especificaLote = _mapper.MapBooleanToString(detallePedido.Identificador != ManejoIdentificadorDb.IdentificadorAuto);
                if (context.NewDuplicados
                    .Any(x => x.Pedido == pedido.Id.Trim()
                        && x.Empresa == lpn.Empresa
                        && x.Producto == detalle.CodigoProducto
                        && x.Cliente == pedido.Cliente
                        && x.Identificador == detallePedido.Identificador
                        && x.IdEspecificaIdentificador == especificaLote)
                    || context.UpdDuplicados
                        .Any(x => x.Pedido == pedido.Id.Trim()
                            && x.Empresa == lpn.Empresa
                            && x.Producto == detalle.CodigoProducto
                            && x.Cliente == pedido.Cliente
                            && x.Identificador == detallePedido.Identificador
                            && x.IdEspecificaIdentificador == especificaLote))
                {
                    string sistemaExterno = "LPN-" + lpn.Tipo + "-" + lpn.IdExterno;
                    var detalleDuplicado = context.NewDuplicados
                        .FirstOrDefault(x => x.Pedido == pedido.Id.Trim()
                            && x.Empresa == lpn.Empresa
                            && x.Producto == detalle.CodigoProducto
                            && x.Cliente == pedido.Cliente
                            && x.Identificador == detallePedido.Identificador
                            && x.IdEspecificaIdentificador == especificaLote
                            && x.IdLineaSistemaExterno == sistemaExterno);

                    if (detalleDuplicado == null)
                    {
                        context.NewDuplicados.Add(new DetallePedidoDuplicado()
                        {
                            Pedido = pedido.Id.Trim(),
                            Empresa = lpn.Empresa,
                            Cliente = pedido.Cliente,
                            Producto = detalle.CodigoProducto,
                            Faixa = 1,
                            Identificador = detallePedido.Identificador,
                            IdEspecificaIdentificador = especificaLote,
                            CantidadPedida = cantidad,
                            FechaAlta = DateTime.Now,
                            IdLineaSistemaExterno = sistemaExterno,
                            TipoLinea = string.Empty,
                            Transaccion = pedido.Transaccion,
                        });
                    }
                    else
                    {
                        context.NewDuplicados.Remove(detalleDuplicado);
                        detalleDuplicado.CantidadPedida += cantidad;
                        context.NewDuplicados.Add(detalleDuplicado);
                    }
                }
            }
        }

        public virtual void AgregarDetallePedidoLpn(PedidoUpdateBulkOperationContext context, Pedido pedido, Lpn lpn, LpnDetalle detalle, DetallePedido detallePedido)
        {
            string identificador = detalle.Lote?.Trim() ?? string.Empty;
            bool especificaIdentificador = true;
            decimal cantidad = GetCantidadPedidoLpn(lpn, detalle);

            if (cantidad > 0)
            {
                if (string.IsNullOrEmpty(identificador) || identificador == ManejoIdentificadorDb.IdentificadorAuto)
                    especificaIdentificador = false;

                var especificaLote = _mapper.MapBooleanToString(identificador != ManejoIdentificadorDb.IdentificadorAuto);
                DetallePedido model = new DetallePedido(_mapper.MapBooleanToString(especificaIdentificador));

                model.Id = pedido.Id;
                model.Cliente = pedido.Cliente;
                model.Agrupacion = pedido.Agrupacion;
                model.Cantidad = cantidad;
                model.FechaAlta = DateTime.Now;
                model.EspecificaIdentificador = especificaIdentificador;
                model.EspecificaIdentificadorId = especificaLote;
                model.CantidadAnulada = 0;
                model.CantidadLiberada = 0;
                model.CantidadOriginal = cantidad;
                model.DatosSerializados = "";
                model.Empresa = lpn.Empresa;
                model.Faixa = 1;
                model.Id = pedido.Id;
                model.Identificador = identificador;
                model.Memo = "";
                model.Producto = detalle.CodigoProducto;
                model.Transaccion = pedido.Transaccion;
                context.NewDetalles.Add(model);

                var detalleLpnPedido = context.NewDetalleLpn
                    .FirstOrDefault(x => x.Pedido == pedido.Id
                        && x.Cliente == pedido.Cliente
                        && x.Empresa == pedido.Empresa
                        && x.Producto == detalle.CodigoProducto
                        && x.Faixa == detalle.Faixa
                        && (x.Identificador == ManejoIdentificadorDb.IdentificadorAuto || x.Identificador == detalle.Lote));

                if (detalleLpnPedido == null)
                {
                    var detalleLpn = new DetallePedidoLpn()
                    {
                        Cliente = pedido.Cliente,
                        CantidadPedida = cantidad,
                        Empresa = lpn.Empresa,
                        Faixa = 1,
                        Identificador = identificador,
                        IdEspecificaIdentificador = identificador == ManejoIdentificadorDb.IdentificadorAuto ? "N" : "S",
                        Pedido = pedido.Id.Trim(),
                        Producto = detalle.CodigoProducto,
                        Tipo = lpn.Tipo ?? string.Empty,
                        IdLpnExterno = lpn.IdExterno,
                        CantidadLiberada = 0,
                        CantidadAnulada = 0,
                        NumeroLpn = lpn.NumeroLPN
                    };
                    detalleLpn.Transaccion = pedido.Transaccion;
                    detalleLpn.FechaAlta = DateTime.Now;
                    context.NewDetalleLpn.Add(GetDetalleLpnEntity(detalleLpn));
                }
                else
                {
                    context.NewDetalleLpn.Remove(detalleLpnPedido);
                    detalleLpnPedido.CantidadPedida = (detalleLpnPedido.CantidadPedida ?? 0) + cantidad;
                    context.NewDetalleLpn.Add(detalleLpnPedido);
                }

                if (context.NewDuplicados
                    .Any(x => x.Pedido == pedido.Id.Trim()
                        && x.Empresa == lpn.Empresa
                        && x.Producto == detalle.CodigoProducto
                        && x.Cliente == pedido.Cliente
                        && x.Identificador == identificador
                        && x.IdEspecificaIdentificador == especificaLote))
                {
                    string sistemaExterno = "LPN-" + lpn.Tipo + "-" + lpn.IdExterno;
                    var detalleDuplicado = context.NewDuplicados
                        .FirstOrDefault(x => x.Pedido == pedido.Id.Trim()
                            && x.Empresa == lpn.Empresa
                            && x.Producto == detalle.CodigoProducto
                            && x.Cliente == pedido.Cliente
                            && x.Identificador == detallePedido.Identificador
                            && x.IdEspecificaIdentificador == especificaLote
                            && x.IdLineaSistemaExterno == sistemaExterno);

                    if (detalleDuplicado == null)
                    {
                        context.NewDuplicados.Add(new DetallePedidoDuplicado()
                        {
                            Cliente = pedido.Cliente,
                            CantidadPedida = cantidad,
                            Empresa = lpn.Empresa,
                            Faixa = 1,
                            Identificador = identificador,
                            Pedido = pedido.Id.Trim(),
                            Producto = detalle.CodigoProducto,
                            IdEspecificaIdentificador = especificaLote,
                            FechaAlta = DateTime.Now,
                            IdLineaSistemaExterno = sistemaExterno,
                            TipoLinea = string.Empty,
                            Transaccion = pedido.Transaccion,
                        });
                    }
                    else
                    {
                        context.NewDuplicados.Remove(detalleDuplicado);
                        detallePedido.Cantidad += detalle.Cantidad;
                        context.NewDuplicados.Add(detalleDuplicado);
                    }
                }
            }
        }

        public static string GetSqlSelectPedido(string sqlSaldo = "")
        {
            return $@"SELECT 
                    P.NU_PEDIDO as Id,
                    P.CD_CLIENTE as Cliente,
                    P.CD_EMPRESA as Empresa,
                    P.CD_CONDICION_LIBERACION as CondicionLiberacion,
                    P.CD_FUN_RESP as FuncionarioResponsable,
                    P.CD_ORIGEN as Origen,
                    P.CD_PUNTO_ENTREGA as PuntoEntrega,
                    P.CD_ROTA as Ruta,
                    P.CD_SITUACAO as Estado,
                    P.CD_TRANSPORTADORA as CodigoTransportadora,
                    P.CD_UF as CodigoUF,
                    P.CD_ZONA as Zona,
                    P.DS_ANEXO1 as Anexo,
                    P.DS_ANEXO2 as Anexo2,
                    P.DS_ANEXO3 as Anexo3,
                    P.DS_ANEXO4 as Anexo4,
                    P.DS_ENDERECO as DireccionEntrega,
                    P.DS_MEMO as Memo,
                    P.DS_MEMO_1 as Memo1,
                    P.DT_ADDROW as FechaAlta,
                    P.DT_EMITIDO as FechaEmision,
                    P.DT_ENTREGA as FechaEntrega,
                    P.DT_FUN_RESP as FechaFuncionarioResponsable,
                    P.DT_GENERICO_1 as FechaGenerica_1,
                    P.DT_LIBERAR_DESDE as FechaLiberarDesde,
                    P.DT_LIBERAR_HASTA as FechaLiberarHasta,
                    P.DT_ULT_PREPARACION as FechaUltimaPreparacion,
                    P.DT_UPDROW as FechaModificacion,
                    P.FL_SYNC_REALIZADA as SincronizacionRealizadaId,
                    P.ID_AGRUPACION as Agrupacion,
                    P.ID_MANUAL as ManualId,
                    P.ND_ACTIVIDAD as Actividad,
                    P.NU_GENERICO_1 as NuGenerico_1,
                    P.NU_INTERFAZ_FACTURACION as NroIntzFacturacion,
                    P.NU_ORDEN_ENTREGA as OrdenEntrega,
                    P.NU_ORDEN_LIBERACION as NumeroOrdenLiberacion,
                    P.NU_PRDC_INGRESO as IngresoProduccion,
                    P.NU_PREDIO as Predio,
                    P.NU_PREPARACION_MANUAL as NroPrepManual,
                    P.NU_PREPARACION_PROGRAMADA as PreparacionProgramada,
                    P.NU_TRANSACCION as Transaccion,
                    P.NU_ULT_PREPARACION as NumeroUltimaPreparacion,
                    P.TP_EXPEDICION as TipoExpedicionId,
                    P.TP_PEDIDO as Tipo,
                    P.VL_COMPARTE_CONTENEDOR_ENTREGA as ComparteContenedorEntrega,
                    P.VL_COMPARTE_CONTENEDOR_PICKING as ComparteContenedorPicking,
                    P.VL_GENERICO_1 as VlGenerico_1,
                    P.VL_SERIALIZADO_1 as VlSerealizado_1,
                    P.NU_CARGA as NuCarga
                    {sqlSaldo}
                FROM T_PEDIDO_SAIDA P ";
        }

        public virtual IEnumerable<DetallePedido> GetDetallesParcialPedidos(IEnumerable<object> keys)
        {
            IEnumerable<DetallePedido> resultado = new List<DetallePedido>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_DET_PEDIDO_SAIDA_TEMP (NU_PEDIDO, CD_CLIENTE, CD_EMPRESA, CD_PRODUTO) 
                                   VALUES (:Id, :Cliente, :Empresa, :Producto)";
                    _dapper.Execute(connection, sql, keys, transaction: tran);

                    sql = GetSqlSelectPedidoDetalle() +
                        @" INNER JOIN T_DET_PEDIDO_SAIDA_TEMP T ON DP.NU_PEDIDO = T.NU_PEDIDO
                            AND DP.CD_CLIENTE = T.CD_CLIENTE                            
                            AND DP.CD_EMPRESA = T.CD_EMPRESA
                            AND DP.CD_PRODUTO = T.CD_PRODUTO";

                    resultado = _dapper.Query<DetallePedido>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public static string GetSqlSelectPedidoDetalle()
        {
            return @"SELECT 
                        DP.NU_PEDIDO as Id,
                        DP.CD_CLIENTE as Cliente,
                        DP.CD_EMPRESA as Empresa,
                        DP.CD_FAIXA as Faixa,
                        DP.CD_PRODUTO as Producto,
                        DP.DS_MEMO as Memo,
                        DP.DT_ADDROW as FechaAlta,
                        DP.DT_GENERICO_1 as FechaGenerica_1,
                        DP.DT_UPDROW as FechaModificacion,
                        DP.ID_AGRUPACION as Agrupacion,
                        DP.ID_ESPECIFICA_IDENTIFICADOR as EspecificaIdentificadorId,
                        DP.NU_GENERICO_1 as NuGenerico_1,
                        DP.NU_IDENTIFICADOR as Identificador,
                        DP.NU_TRANSACCION as Transaccion,
                        DP.QT_ABASTECIDO as CantidadAbastecida,
                        DP.QT_ANULADO as CantidadAnulada,
                        DP.QT_ANULADO_FACTURA as CantidadAnuladaFactura,
                        DP.QT_CARGADO as CantidadCargada,
                        DP.QT_CONTROLADO as CantidadControlada,
                        DP.QT_CROSS_DOCK as CantidadCrossDocking,
                        DP.QT_EXPEDIDO as CantidadExpedida,
                        DP.QT_FACTURADO as CantidadFacturada,
                        DP.QT_LIBERADO as CantidadLiberada,
                        DP.QT_PEDIDO as Cantidad,
                        DP.QT_PEDIDO_ORIGINAL as CantidadOriginal,
                        DP.QT_PREPARADO as CantidadPreparada,
                        DP.QT_TRANSFERIDO as CantidadTransferida,
                        DP.QT_UND_ASOCIADO_CAMION as CantUndAsociadoCamion,
                        DP.VL_GENERICO_1 as VlGenerico_1,
                        DP.VL_PORCENTAJE_TOLERANCIA as PorcentajeTolerancia,
                        DP.VL_SERIALIZADO_1 as DatosSerializados,
                        DP.ID_ESPECIFICA_IDENTIFICADOR as EspecificaIdentificador
                    FROM T_DET_PEDIDO_SAIDA DP ";
        }

        public virtual IEnumerable<DetallePedidoDuplicado> GetDetallesDuplicadosParcial(IEnumerable<object> keys)
        {
            IEnumerable<DetallePedidoDuplicado> resultado = new List<DetallePedidoDuplicado>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_DET_PEDIDO_SAIDA_DUP_TEMP (NU_PEDIDO, CD_CLIENTE, CD_EMPRESA, CD_PRODUTO, ID_ESPECIFICA_IDENTIFICADOR) 
                        VALUES (:Id, :Cliente, :Empresa, :Producto, :EspecificaIdentificador)";
                    _dapper.Execute(connection, sql, keys, transaction: tran);

                    sql = GetSqlSelectDuplicadoPedido() +
                        @" INNER JOIN T_DET_PEDIDO_SAIDA_DUP_TEMP T ON DP.NU_PEDIDO = T.NU_PEDIDO
                            AND DP.CD_CLIENTE = T.CD_CLIENTE                            
                            AND DP.CD_EMPRESA = T.CD_EMPRESA
                            AND DP.CD_PRODUTO = T.CD_PRODUTO                            
                            AND DP.ID_ESPECIFICA_IDENTIFICADOR = T.ID_ESPECIFICA_IDENTIFICADOR";

                    resultado = _dapper.Query<DetallePedidoDuplicado>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public static string GetSqlSelectDuplicadoPedido()
        {
            return @"SELECT 
                        DP.NU_PEDIDO as Pedido,
                        DP.CD_CLIENTE as Cliente,
                        DP.CD_EMPRESA as Empresa,
                        DP.CD_PRODUTO as Producto,
                        DP.CD_FAIXA as Faixa,
                        DP.NU_IDENTIFICADOR as Identificador,
                        DP.ID_ESPECIFICA_IDENTIFICADOR as IdEspecificaIdentificador,
                        DP.ID_LINEA_SISTEMA_EXTERNO as IdLineaSistemaExterno,
                        DP.TP_LINEA as TipoLinea,
                        DP.QT_PEDIDO as CantidadPedida,
                        DP.QT_EXPEDIDO as CantidadExpedida,
                        DP.QT_ANULADO as CantidadAnulada,
                        DP.QT_FACTURADO as CantidadFacturada,
                        DP.DT_ADDROW as FechaAlta,
                        DP.DT_UPDROW as FechaModificacion,
                        DP.VL_SERIALIZADO_1 as DatosSerializados
                    FROM T_DET_PEDIDO_SAIDA_DUP DP ";
        }

        public virtual int GetCantDuplicados(DetallePedido det)
        {
            int result = 0;
            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                string sql = @"SELECT COUNT(*) FROM T_DET_PEDIDO_SAIDA_DUP WHERE NU_PEDIDO = :Id AND CD_CLIENTE = :Cliente AND CD_EMPRESA = :Empresa 
                                   AND CD_PRODUTO = :Producto AND NU_IDENTIFICADOR = :Identificador AND ID_ESPECIFICA_IDENTIFICADOR = :EspecificaIdentificador";

                result = _dapper.Query<int>(connection, sql, param: GetLineaEntity(det), commandType: CommandType.Text).FirstOrDefault();
            }
            return result;
        }

        public virtual object GetPedidoEntityUpdate(Pedido pedido)
        {
            return new
            {
                Id = pedido.Id,
                Cliente = pedido.Cliente,
                Empresa = pedido.Empresa,
                CondicionLiberacion = pedido.CondicionLiberacion,
                FuncionarioResponsable = pedido.FuncionarioResponsable,
                Origen = pedido.Origen,
                PuntoEntrega = pedido.PuntoEntrega,
                Ruta = pedido.Ruta,
                Estado = pedido.Estado,
                CodigoTransportadora = pedido.CodigoTransportadora,
                CodigoUF = pedido.CodigoUF,
                Zona = pedido.Zona,
                Anexo = pedido.Anexo,
                Anexo2 = pedido.Anexo2,
                Anexo3 = pedido.Anexo3,
                Anexo4 = pedido.Anexo4,
                DireccionEntrega = pedido.DireccionEntrega,
                Memo = pedido.Memo,
                Memo1 = pedido.Memo1,
                FechaAlta = pedido.FechaAlta,
                FechaEmision = pedido.FechaEmision,
                FechaEntrega = pedido.FechaEntrega,
                FechaFuncionarioResponsable = pedido.FechaFuncionarioResponsable,
                FechaGenerica_1 = pedido.FechaGenerica_1,
                FechaLiberarDesde = pedido.FechaLiberarDesde,
                FechaLiberarHasta = pedido.FechaLiberarHasta,
                FechaUltimaPreparacion = pedido.FechaUltimaPreparacion,
                FechaModificacion = DateTime.Now,
                SincronizacionRealizada = pedido.SincronizacionRealizadaId,
                Agrupacion = pedido.Agrupacion,
                Manual = pedido.ManualId,
                Actividad = pedido.Actividad,
                NuGenerico_1 = pedido.NuGenerico_1,
                NroIntzFacturacion = pedido.NroIntzFacturacion,
                OrdenEntrega = pedido.OrdenEntrega,
                NumeroOrdenLiberacion = pedido.NumeroOrdenLiberacion,
                IngresoProduccion = pedido.IngresoProduccion,
                Predio = pedido.Predio,
                NroPrepManual = pedido.NroPrepManual,
                PreparacionProgramada = pedido.PreparacionProgramada,
                Transaccion = pedido.Transaccion,
                NumeroUltimaPreparacion = pedido.NumeroUltimaPreparacion,
                TipoExpedicion = pedido.TipoExpedicionId,
                Tipo = pedido.Tipo,
                ComparteContenedorEntrega = pedido.ComparteContenedorEntrega,
                ComparteContenedorPicking = pedido.ComparteContenedorPicking,
                VlGenerico_1 = pedido.VlGenerico_1,
                VlSerealizado_1 = pedido.VlSerealizado_1,
                Telefono = pedido.Telefono,
                TelefonoSecundario = pedido.TelefonoSecundario,
                Longitud = pedido.Longitud,
                Latitud = pedido.Latitud
            };
        }

        public virtual Pedido Map(Pedido request, Pedido model)
        {
            Pedido pedido = new Pedido();

            pedido.Id = request.Id;
            pedido.Cliente = request.Cliente;
            pedido.Empresa = request.Empresa;
            pedido.Estado = model.Estado;
            pedido.FechaAlta = model.FechaAlta;
            pedido.FechaUltimaPreparacion = model.FechaUltimaPreparacion;
            pedido.SincronizacionRealizadaId = model.SincronizacionRealizadaId;
            pedido.ManualId = model.ManualId;
            pedido.Actividad = model.Actividad;
            pedido.NroIntzFacturacion = model.NroIntzFacturacion;
            pedido.NumeroOrdenLiberacion = model.NumeroOrdenLiberacion;
            pedido.IngresoProduccion = model.IngresoProduccion;
            pedido.NroPrepManual = model.NroPrepManual;
            pedido.PreparacionProgramada = model.PreparacionProgramada;
            pedido.Transaccion = model.Transaccion;
            pedido.NumeroUltimaPreparacion = model.NumeroUltimaPreparacion;
            pedido.FechaFuncionarioResponsable = model.FechaFuncionarioResponsable;

            pedido.CondicionLiberacion = request.CondicionLiberacion ?? model.CondicionLiberacion;
            pedido.FuncionarioResponsable = request.FuncionarioResponsable ?? model.FuncionarioResponsable;
            pedido.Origen = request.Origen ?? model.Origen;
            pedido.PuntoEntrega = request.PuntoEntrega ?? model.PuntoEntrega;
            pedido.Ruta = request.Ruta ?? model.Ruta;
            pedido.CodigoTransportadora = request.CodigoTransportadora ?? model.CodigoTransportadora;
            pedido.CodigoUF = request.CodigoUF ?? model.CodigoUF;
            pedido.Zona = request.Zona ?? model.Zona;
            pedido.Anexo = request.Anexo ?? model.Anexo;
            pedido.Anexo2 = request.Anexo2 ?? model.Anexo2;
            pedido.Anexo3 = request.Anexo3 ?? model.Anexo3;
            pedido.Anexo4 = request.Anexo4 ?? model.Anexo4;
            pedido.DireccionEntrega = request.DireccionEntrega ?? model.DireccionEntrega;
            pedido.Memo = request.Memo ?? model.Memo;
            pedido.Memo1 = request.Memo1 ?? model.Memo1;
            pedido.FechaEmision = request.FechaEmision ?? model.FechaEmision;
            pedido.FechaEntrega = request.FechaEntrega ?? model.FechaEntrega;
            pedido.FechaGenerica_1 = request.FechaGenerica_1 ?? model.FechaGenerica_1;
            pedido.FechaLiberarDesde = request.FechaLiberarDesde ?? model.FechaLiberarDesde;
            pedido.FechaLiberarHasta = request.FechaLiberarHasta ?? model.FechaLiberarHasta;
            pedido.FechaModificacion = DateTime.Now;
            pedido.Agrupacion = request.Agrupacion ?? model.Agrupacion;
            pedido.NuGenerico_1 = request.NuGenerico_1 ?? model.NuGenerico_1;
            pedido.OrdenEntrega = request.OrdenEntrega ?? model.OrdenEntrega;
            pedido.Predio = request.Predio ?? model.Predio;
            pedido.TipoExpedicionId = request.TipoExpedicionId ?? model.TipoExpedicionId;
            pedido.Tipo = request.Tipo ?? model.Tipo;
            pedido.ComparteContenedorEntrega = request.ComparteContenedorEntrega ?? model.ComparteContenedorEntrega;
            pedido.ComparteContenedorPicking = request.ComparteContenedorPicking ?? model.ComparteContenedorPicking;
            pedido.VlGenerico_1 = request.VlGenerico_1 ?? model.VlGenerico_1;
            pedido.VlSerealizado_1 = request.VlSerealizado_1 ?? model.VlSerealizado_1;
            pedido.Telefono = request.Telefono ?? model.Telefono;
            pedido.TelefonoSecundario = request.TelefonoSecundario ?? model.TelefonoSecundario;
            pedido.Longitud = request.Longitud ?? model.Longitud;
            pedido.Latitud = request.Latitud ?? model.Latitud;

            if (model.Actividad == EstadoPedidoDb.Vencido && model.FechaLiberarHasta.HasValue && request.FechaLiberarHasta.HasValue && model.FechaLiberarHasta.Value.Date != request.FechaLiberarHasta.Value.Date)
            {
                pedido.Actividad = EstadoPedidoDb.Activo;
            }

            return pedido;
        }

        public virtual DetallePedido GetDetallePedidoEntityUpdate(DetallePedido detalle)
        {
            return new DetallePedido()
            {
                Id = detalle.Id,
                Empresa = detalle.Empresa,
                Cliente = detalle.Cliente,
                Faixa = detalle.Faixa,
                Producto = detalle.Producto,
                Memo = detalle.Memo,
                FechaAlta = detalle.FechaAlta,
                FechaGenerica_1 = detalle.FechaGenerica_1,
                FechaModificacion = DateTime.Now,
                Agrupacion = detalle.Agrupacion,
                EspecificaIdentificadorId = detalle.EspecificaIdentificadorId,
                NuGenerico_1 = detalle.NuGenerico_1,
                Identificador = detalle.Identificador,
                Transaccion = detalle.Transaccion,
                CantidadAbastecida = detalle.CantidadAbastecida,
                CantidadAnulada = detalle.CantidadAnulada,
                CantidadAnuladaFactura = detalle.CantidadAnuladaFactura,
                CantidadCargada = detalle.CantidadCargada,
                CantidadControlada = detalle.CantidadControlada,
                CantidadCrossDocking = detalle.CantidadCrossDocking,
                CantidadExpedida = detalle.CantidadExpedida,
                CantidadFacturada = detalle.CantidadFacturada,
                CantidadLiberada = detalle.CantidadLiberada,
                Cantidad = detalle.Cantidad,
                CantidadOriginal = detalle.CantidadOriginal,
                CantidadPreparada = detalle.CantidadPreparada,
                CantidadTransferida = detalle.CantidadTransferida,
                CantUndAsociadoCamion = detalle.CantUndAsociadoCamion,
                VlGenerico_1 = detalle.VlGenerico_1,
                PorcentajeTolerancia = detalle.PorcentajeTolerancia,
                DatosSerializados = detalle.DatosSerializados,
            };
        }

        public virtual DetallePedido MapDetalle(DetallePedido request, DetallePedido model)
        {
            DetallePedido detalle = new DetallePedido();

            detalle.Id = request.Id;
            detalle.Empresa = request.Empresa;
            detalle.Cliente = request.Cliente;
            detalle.Faixa = request.Faixa;
            detalle.Producto = request.Producto;
            detalle.FechaAlta = model.FechaAlta;
            detalle.FechaModificacion = DateTime.Now;
            detalle.Identificador = request.Identificador;
            detalle.EspecificaIdentificadorId = request.EspecificaIdentificadorId;
            detalle.CantidadAbastecida = model.CantidadAbastecida;
            detalle.CantidadAnulada = model.CantidadAnulada;
            detalle.CantidadAnuladaFactura = model.CantidadAnuladaFactura;
            detalle.CantidadCargada = model.CantidadCargada;
            detalle.CantidadControlada = model.CantidadControlada;
            detalle.CantidadCrossDocking = model.CantidadCrossDocking;
            detalle.CantidadExpedida = model.CantidadExpedida;
            detalle.CantidadFacturada = model.CantidadFacturada;
            detalle.CantidadLiberada = model.CantidadLiberada;
            detalle.CantidadOriginal = model.CantidadOriginal;
            detalle.CantidadPreparada = model.CantidadPreparada;
            detalle.CantidadTransferida = model.CantidadTransferida;
            detalle.CantUndAsociadoCamion = model.CantUndAsociadoCamion;
            detalle.Transaccion = model.Transaccion;
            detalle.TransaccionDelete = model.TransaccionDelete;

            detalle.Memo = request.Memo ?? model.Memo;
            detalle.FechaGenerica_1 = request.FechaGenerica_1 ?? model.FechaGenerica_1;
            detalle.Agrupacion = request.Agrupacion ?? model.Agrupacion;
            detalle.NuGenerico_1 = request.NuGenerico_1 ?? model.NuGenerico_1;
            detalle.Cantidad = request.Cantidad ?? model.Cantidad;
            detalle.VlGenerico_1 = request.VlGenerico_1 ?? model.VlGenerico_1;
            detalle.PorcentajeTolerancia = request.PorcentajeTolerancia ?? model.PorcentajeTolerancia;
            detalle.DatosSerializados = request.DatosSerializados ?? model.DatosSerializados;

            return detalle;
        }

        public virtual DetallePedidoDuplicado GetDuplicadoEntityUpdate(DetallePedidoDuplicado duplicado)
        {
            return new DetallePedidoDuplicado()
            {
                Pedido = duplicado.Pedido,
                Empresa = duplicado.Empresa,
                Cliente = duplicado.Cliente,
                Faixa = duplicado.Faixa,
                Producto = duplicado.Producto,
                FechaAlta = duplicado.FechaAlta,
                FechaModificacion = DateTime.Now,
                IdEspecificaIdentificador = duplicado.IdEspecificaIdentificador,
                Identificador = duplicado.Identificador,
                IdLineaSistemaExterno = duplicado.IdLineaSistemaExterno,
                TipoLinea = duplicado.TipoLinea,
                CantidadAnulada = duplicado.CantidadAnulada,
                CantidadExpedida = duplicado.CantidadExpedida,
                CantidadFacturada = duplicado.CantidadFacturada,
                CantidadPedida = duplicado.CantidadPedida,
                DatosSerializados = duplicado.DatosSerializados,
                Transaccion = duplicado.Transaccion,
            };
        }

        public virtual DetallePedidoDuplicado MapDuplicado(DetallePedidoDuplicado request, DetallePedidoDuplicado model)
        {
            DetallePedidoDuplicado duplicado = new DetallePedidoDuplicado();

            duplicado.Pedido = model.Pedido;
            duplicado.Empresa = model.Empresa;
            duplicado.Cliente = model.Cliente;
            duplicado.Faixa = model.Faixa;
            duplicado.Producto = model.Producto;
            duplicado.FechaAlta = model.FechaAlta;
            duplicado.FechaModificacion = DateTime.Now;
            duplicado.IdEspecificaIdentificador = model.IdEspecificaIdentificador;
            duplicado.Identificador = model.Identificador;
            duplicado.IdLineaSistemaExterno = model.IdLineaSistemaExterno;
            duplicado.CantidadAnulada = model.CantidadAnulada;
            duplicado.CantidadExpedida = model.CantidadExpedida;
            duplicado.CantidadFacturada = model.CantidadFacturada;

            duplicado.CantidadPedida = request.CantidadPedida;
            duplicado.TipoLinea = request.TipoLinea ?? model.TipoLinea;
            duplicado.DatosSerializados = request.DatosSerializados ?? model.DatosSerializados;

            return duplicado;
        }

        public virtual DetallePedidoLpn MapDetalleLpn(DetallePedidoLpn request, DetallePedidoLpn model)
        {
            DetallePedidoLpn detalleLpn = new DetallePedidoLpn();

            detalleLpn.Pedido = model.Pedido;
            detalleLpn.Empresa = model.Empresa;
            detalleLpn.Cliente = model.Cliente;
            detalleLpn.Faixa = model.Faixa;
            detalleLpn.Producto = model.Producto;
            detalleLpn.FechaAlta = model.FechaAlta;
            detalleLpn.FechaModificacion = DateTime.Now;
            detalleLpn.IdLpnExterno = model.IdLpnExterno;
            detalleLpn.Identificador = model.Identificador;
            detalleLpn.IdEspecificaIdentificador = model.IdEspecificaIdentificador;
            detalleLpn.CantidadAnulada = model.CantidadAnulada;
            detalleLpn.CantidadLiberada = model.CantidadLiberada;
            detalleLpn.CantidadAnulada = model.CantidadAnulada;

            detalleLpn.CantidadPedida = request.CantidadPedida;
            detalleLpn.Tipo = request.Tipo ?? model.Tipo;

            return detalleLpn;
        }

        public virtual DetallePedidoLpn GetDetalleLpnEntityUpdate(DetallePedidoLpn detalleLpn)
        {
            return new DetallePedidoLpn
            {
                Pedido = detalleLpn.Pedido,
                Empresa = detalleLpn.Empresa,
                Cliente = detalleLpn.Cliente,
                Faixa = detalleLpn.Faixa,
                Producto = detalleLpn.Producto,
                FechaAlta = detalleLpn.FechaAlta,
                FechaModificacion = DateTime.Now,
                IdLpnExterno = detalleLpn.IdLpnExterno,
                Identificador = detalleLpn.Identificador,
                IdEspecificaIdentificador = detalleLpn.IdEspecificaIdentificador,
                Tipo = detalleLpn.Tipo,
                CantidadAnulada = detalleLpn.CantidadAnulada,
                CantidadLiberada = detalleLpn.CantidadLiberada,
                CantidadPedida = detalleLpn.CantidadPedida,
                Transaccion = detalleLpn.Transaccion,
            };
        }

        public virtual IEnumerable<PedidoContenedor> GetPedidosContenedor(IEnumerable<Contenedor> contenedores)
        {
            IEnumerable<PedidoContenedor> resultado = new List<PedidoContenedor>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_CONTENEDOR_TEMP (NU_CONTENEDOR) VALUES (:Numero)";
                    _dapper.Execute(connection, sql, contenedores, transaction: tran);

                    sql = @"SELECT
                                P.NU_PEDIDO as Pedido,
                                P.CD_CLIENTE as Cliente,
                                P.CD_EMPRESA as Empresa,
                                P.CD_ROTA as Ruta,                    
                                P.NU_PREDIO as Predio,
                                P.VL_COMPARTE_CONTENEDOR_ENTREGA as ComparteContenedorEntrega,
                                P.VL_COMPARTE_CONTENEDOR_PICKING as ComparteContenedorPicking,
                                DP.NU_PREPARACION as Preparacion,
                                DP.NU_CONTENEDOR as Contenedor,
                                C.CD_SITUACAO as EstadoContenedor
                            FROM T_PEDIDO_SAIDA P
                            INNER JOIN T_DET_PICKING DP ON P.NU_PEDIDO = DP.NU_PEDIDO AND P.CD_CLIENTE = DP.CD_CLIENTE AND P.CD_EMPRESA = DP.CD_EMPRESA
                            INNER JOIN T_CONTENEDOR_TEMP T ON DP.NU_CONTENEDOR = T.NU_CONTENEDOR
                            INNER JOIN T_CONTENEDOR C ON DP.NU_CONTENEDOR = C.NU_CONTENEDOR AND DP.NU_PREPARACION = C.NU_PREPARACION
                            GROUP BY
                                P.NU_PEDIDO ,
                                P.CD_CLIENTE ,
                                P.CD_EMPRESA ,
                                P.CD_ROTA ,                    
                                P.NU_PREDIO ,
                                P.VL_COMPARTE_CONTENEDOR_ENTREGA ,
                                P.VL_COMPARTE_CONTENEDOR_PICKING ,
                                DP.NU_PREPARACION,
                                DP.NU_CONTENEDOR,
                                C.CD_SITUACAO";

                    resultado = _dapper.Query<PedidoContenedor>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual async Task BulkUpdatePedidos(DbConnection connection, DbTransaction tran, List<object> pedidos)
        {
            string sql = @"UPDATE T_PEDIDO_SAIDA 
                           SET CD_CONDICION_LIBERACION = :CondicionLiberacion, 
                               CD_FUN_RESP = :FuncionarioResponsable,
                               CD_ORIGEN = :Origen,
                               CD_PUNTO_ENTREGA = :PuntoEntrega, 
                               CD_ROTA = :Ruta, 
                               CD_ZONA = :Zona, 
                               CD_TRANSPORTADORA = :CodigoTransportadora,
                               DS_ANEXO1 = :Anexo, 
                               DS_ANEXO2 = :Anexo2, 
                               DS_ANEXO3 = :Anexo3, 
                               DS_ANEXO4 = :Anexo4, 
                               DS_ENDERECO = :DireccionEntrega, 
                               DS_MEMO = :Memo, 
                               DS_MEMO_1 = :Memo1, 
                               DT_GENERICO_1 = :FechaGenerica_1, 
                               DT_EMITIDO = :FechaEmision, 
                               DT_ENTREGA = :FechaEntrega, 
                               DT_LIBERAR_DESDE = :FechaLiberarDesde, 
                               DT_LIBERAR_HASTA = :FechaLiberarHasta, 
                               ID_AGRUPACION = :Agrupacion, 
                               NU_GENERICO_1 = :NuGenerico_1, 
                               NU_ORDEN_ENTREGA = :OrdenEntrega, 
                               NU_PREDIO = :Predio, 
                               TP_EXPEDICION = :TipoExpedicion, 
                               TP_PEDIDO = :Tipo, 
                               VL_COMPARTE_CONTENEDOR_ENTREGA = :ComparteContenedorEntrega, 
                               VL_COMPARTE_CONTENEDOR_PICKING = :ComparteContenedorPicking, 
                               VL_GENERICO_1 = :VlGenerico_1, 
                               NU_TELEFONE = :Telefono,
                               NU_TELEFONE2 = :TelefonoSecundario,
                               VL_LONGITUD = :Longitud,
                               VL_LATITUD = :Latitud,
                               VL_SERIALIZADO_1 = :VlSerealizado_1,
                               NU_TRANSACCION = :Transaccion,
                               DT_UPDROW = :FechaModificacion,
                               ND_ACTIVIDAD = :Actividad
                           WHERE NU_PEDIDO = :Id AND CD_CLIENTE = :Cliente AND CD_EMPRESA = :Empresa";
            await _dapper.ExecuteAsync(connection, sql, pedidos, transaction: tran);
        }

        public virtual async Task BulkUpdateDetalles(DbConnection connection, DbTransaction tran, List<DetallePedido> detalles)
        {
            string sql = @"UPDATE T_DET_PEDIDO_SAIDA SET DS_MEMO = :Memo, DT_GENERICO_1 = :FechaGenerica_1, ID_AGRUPACION = :Agrupacion,
                           NU_GENERICO_1 = :NuGenerico_1, QT_PEDIDO = :Cantidad, VL_GENERICO_1 = :VlGenerico_1, VL_PORCENTAJE_TOLERANCIA = :PorcentajeTolerancia,
                           VL_SERIALIZADO_1= :DatosSerializados, NU_TRANSACCION = :Transaccion, DT_UPDROW = :FechaModificacion
                           WHERE NU_PEDIDO = :Id AND CD_CLIENTE = :Cliente AND CD_EMPRESA = :Empresa
                           AND CD_PRODUTO = :Producto AND CD_FAIXA = :Faixa AND NU_IDENTIFICADOR = :Identificador AND ID_ESPECIFICA_IDENTIFICADOR = :EspecificaIdentificadorId";

            await _dapper.ExecuteAsync(connection, sql, detalles, transaction: tran);
        }

        public static string GetSqlSelectPedidoContenedor()
        {
            return @"SELECT 
                    P.NU_PEDIDO as Pedido,
                    P.CD_CLIENTE as Cliente,
                    P.CD_EMPRESA as Empresa,
                    P.CD_ROTA as Ruta,                    
                    P.NU_PREDIO as Predio,
                    P.VL_COMPARTE_CONTENEDOR_ENTREGA as ComparteContenedorEntrega,
                    P.VL_COMPARTE_CONTENEDOR_PICKING as ComparteContenedorPicking,
                    P.VL_GENERICO_1 as Preparacion,
                    P.NU_CONTE as Contenedor
                FROM T_PEDIDO_SAIDA P ";
        }

        public virtual async Task BulkUpdateDuplicados(DbConnection connection, DbTransaction tran, List<DetallePedidoDuplicado> duplicados)
        {
            string sql = @"
                UPDATE T_DET_PEDIDO_SAIDA_DUP 
                SET QT_PEDIDO = :CantidadPedida, 
                    TP_LINEA = :TipoLinea, 
                    VL_SERIALIZADO_1 = :DatosSerializados, 
                    NU_TRANSACCION = :Transaccion, 
                    DT_UPDROW = :FechaModificacion
                WHERE NU_PEDIDO = :Pedido 
                    AND CD_CLIENTE = :Cliente 
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto 
                    AND CD_FAIXA = :Faixa 
                    AND NU_IDENTIFICADOR =:Identificador 
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                    AND ID_LINEA_SISTEMA_EXTERNO = :IdLineaSistemaExterno";

            await _dapper.ExecuteAsync(connection, sql, duplicados, transaction: tran);
        }

        public virtual async Task BulkUpdateDetalleLpn(DbConnection connection, DbTransaction tran, List<DetallePedidoLpn> detallesLpn)
        {
            string sql = @"
                UPDATE T_DET_PEDIDO_SAIDA_LPN 
                SET QT_PEDIDO = :CantidadPedida, 
                    NU_TRANSACCION = :Transaccion, 
                    DT_UPDROW = :FechaModificacion
                WHERE NU_PEDIDO = :Pedido 
                    AND CD_CLIENTE = :Cliente 
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto 
                    AND CD_FAIXA = :Faixa 
                    AND NU_IDENTIFICADOR =:Identificador 
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                    AND ID_LPN_EXTERNO = :IdLpnExterno
                    AND TP_LPN_TIPO = :Tipo";

            await _dapper.ExecuteAsync(connection, sql, detallesLpn, transaction: tran);
        }

        public virtual async Task BulkBeforeDeleteDetalleLpn(DbConnection connection, DbTransaction tran, List<DetallePedidoLpn> detallesLpn)
        {
            string sql = @"
                UPDATE T_DET_PEDIDO_SAIDA_LPN 
                SET NU_TRANSACCION = :Transaccion,
                    NU_TRANSACCION_DELETE = :Transaccion, 
                    DT_UPDROW = :FechaModificacion
                WHERE NU_PEDIDO = :Pedido 
                    AND CD_CLIENTE = :Cliente 
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto 
                    AND CD_FAIXA = :Faixa 
                    AND NU_IDENTIFICADOR =:Identificador 
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                    AND ID_LPN_EXTERNO = :IdLpnExterno
                    AND TP_LPN_TIPO = :Tipo";

            await _dapper.ExecuteAsync(connection, sql, detallesLpn, transaction: tran);
        }

        public virtual IEnumerable<DetallePedidoAtributos> GetAtributos(List<Pedido> pedidos)
        {
            var configuraciones = new Dictionary<string, DetallePedidoAtributos>();
            List<DetallePedidoConfiguracionAtributo> atributos;

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PEDIDO_SAIDA_TEMP (NU_PEDIDO, CD_EMPRESA, CD_CLIENTE) VALUES (:Id, :Empresa, :Cliente)";
                    _dapper.Execute(connection, sql, pedidos, transaction: tran);

                    sql = @"
                        SELECT
                            DPSA.NU_DET_PED_SAI_ATRIB as IdConfiguracion,
                            DPSA.NU_PEDIDO as Pedido,
                            DPSA.CD_CLIENTE as Cliente,
                            DPSA.CD_EMPRESA as Empresa,
                            DPSA.CD_PRODUTO as Producto,
                            DPSA.CD_FAIXA as Faixa,
                            DPSA.NU_IDENTIFICADOR as Identificador,
                            DPSA.ID_ESPECIFICA_IDENTIFICADOR as IdEspecificaIdentificador,
                            DPSA.QT_PEDIDO as CantidadPedida,
                            DPSA.QT_LIBERADO as CantidadLiberada,
                            DPSA.QT_ANULADO as CantidadAnulada,
                            DPSA.DT_ADDROW as FechaAltaConfiguracion,
                            DPSAD.ID_ATRIBUTO as IdAtributo,
                            A.NM_ATRIBUTO as Nombre,
                            DPSAD.FL_CABEZAL as IdCabezal,
                            DPSAD.VL_ATRIBUTO as Valor,
                            DPSAD.DT_ADDROW as FechaAltaAtributo
                        FROM T_DET_PEDIDO_SAIDA_ATRIB DPSA 
                        INNER JOIN T_PEDIDO_SAIDA PS ON PS.NU_PEDIDO = DPSA.NU_PEDIDO
                            AND PS.CD_EMPRESA = DPSA.CD_EMPRESA
                            AND PS.CD_CLIENTE = DPSA.CD_CLIENTE 
                        INNER JOIN T_DET_PEDIDO_SAIDA_ATRIB_DET DPSAD ON DPSA.NU_DET_PED_SAI_ATRIB = DPSAD.NU_DET_PED_SAI_ATRIB
                        INNER JOIN T_ATRIBUTO A ON DPSAD.ID_ATRIBUTO = A.ID_ATRIBUTO
                        INNER JOIN T_PEDIDO_SAIDA_TEMP T ON PS.NU_PEDIDO = T.NU_PEDIDO 
                            AND PS.CD_EMPRESA = T.CD_EMPRESA
                            AND PS.CD_CLIENTE = T.CD_CLIENTE";

                    atributos = _dapper.Query<DetallePedidoConfiguracionAtributo>(connection, sql, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            foreach (var atributo in atributos)
            {
                var keyConfiguracion = $"{atributo.IdConfiguracion}.{atributo.Pedido}.{atributo.Cliente}.{atributo.Empresa}.{atributo.Producto}.{atributo.Faixa.ToString("#.###")}.{atributo.Identificador}.{atributo.IdEspecificaIdentificador}";
                var configuracion = configuraciones.GetValueOrDefault(keyConfiguracion, null);

                if (configuracion == null)
                {
                    configuracion = new DetallePedidoAtributos
                    {
                        Atributos = new List<DetallePedidoConfigAtributo>(),
                        CantidadPedida = atributo.CantidadPedida,
                        Cliente = atributo.Cliente,
                        Empresa = atributo.Empresa,
                        Faixa = atributo.Faixa,
                        FechaAlta = atributo.FechaAltaConfiguracion,
                        IdConfiguracion = atributo.IdConfiguracion,
                        Identificador = atributo.Identificador,
                        IdEspecificaIdentificador = atributo.IdEspecificaIdentificador,
                        Pedido = atributo.Pedido,
                        Producto = atributo.Producto,
                    };

                    configuraciones[keyConfiguracion] = configuracion;
                }

                configuracion.Atributos.Add(new DetallePedidoConfigAtributo
                {
                    FechaAlta = atributo.FechaAltaAtributo,
                    IdAtributo = atributo.IdAtributo,
                    IdCabezal = atributo.IdCabezal,
                    IdConfiguracion = atributo.IdConfiguracion,
                    Nombre = atributo.Nombre,
                    Valor = atributo.Valor,
                });
            }

            return configuraciones.Values;
        }

        public virtual IEnumerable<DetallePedidoAtributosLpn> GetAtributosLpn(List<Pedido> pedidos)
        {
            var configuraciones = new Dictionary<string, DetallePedidoAtributosLpn>();
            List<DetallePedidoConfiguracionAtributoLpn> atributos;

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PEDIDO_SAIDA_TEMP (NU_PEDIDO, CD_EMPRESA, CD_CLIENTE) VALUES (:Id, :Empresa, :Cliente)";
                    _dapper.Execute(connection, sql, pedidos, transaction: tran);

                    sql = @"
                        SELECT
                            DPSLA.NU_DET_PED_SAI_ATRIB as IdConfiguracion,
                            L.ID_LPN_EXTERNO as IdLpnExterno,
                            L.TP_LPN_TIPO as Tipo,
                            DPSLA.NU_PEDIDO as Pedido,
                            DPSLA.CD_CLIENTE as Cliente,
                            DPSLA.CD_EMPRESA as Empresa,
                            DPSLA.CD_PRODUTO as Producto,
                            DPSLA.CD_FAIXA as Faixa,
                            DPSLA.NU_IDENTIFICADOR as Identificador,
                            DPSLA.ID_ESPECIFICA_IDENTIFICADOR as IdEspecificaIdentificador,
                            DPSLA.QT_PEDIDO as CantidadPedida,
                            DPSLA.QT_LIBERADO as CantidadLiberada,
                            DPSLA.QT_ANULADO as CantidadAnulada,
                            DPSLA.DT_ADDROW as FechaAltaConfiguracion,
                            DPSAD.ID_ATRIBUTO as IdAtributo,
                            A.NM_ATRIBUTO as Nombre,
                            DPSAD.FL_CABEZAL as IdCabezal,
                            DPSAD.VL_ATRIBUTO as Valor,
                            DPSAD.DT_ADDROW as FechaAltaAtributo
                        FROM T_DET_PEDIDO_SAIDA_LPN_ATRIB DPSLA
                        INNER JOIN T_PEDIDO_SAIDA PS ON PS.NU_PEDIDO = DPSLA.NU_PEDIDO
                            AND PS.CD_EMPRESA = DPSLA.CD_EMPRESA
                            AND PS.CD_CLIENTE = DPSLA.CD_CLIENTE 
                        INNER JOIN T_LPN L ON L.ID_LPN_EXTERNO = DPSLA.ID_LPN_EXTERNO
                            AND L.TP_LPN_TIPO = DPSLA.TP_LPN_TIPO
                        INNER JOIN T_DET_PEDIDO_SAIDA_ATRIB_DET DPSAD ON DPSLA.NU_DET_PED_SAI_ATRIB = DPSAD.NU_DET_PED_SAI_ATRIB
                        INNER JOIN T_ATRIBUTO A ON DPSAD.ID_ATRIBUTO = A.ID_ATRIBUTO
                        INNER JOIN T_PEDIDO_SAIDA_TEMP T ON PS.NU_PEDIDO = T.NU_PEDIDO 
                            AND PS.CD_EMPRESA = T.CD_EMPRESA
                            AND PS.CD_CLIENTE = T.CD_CLIENTE ";

                    atributos = _dapper.Query<DetallePedidoConfiguracionAtributoLpn>(connection, sql, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            foreach (var atributo in atributos)
            {
                var keyConfiguracion = $"{atributo.IdConfiguracion}.{atributo.IdLpnExterno}.{atributo.Tipo}.{atributo.Pedido}.{atributo.Cliente}.{atributo.Empresa}.{atributo.Producto}.{atributo.Faixa.ToString("#.###")}.{atributo.Identificador}.{atributo.IdEspecificaIdentificador}";
                var configuracion = configuraciones.GetValueOrDefault(keyConfiguracion, null);

                if (configuracion == null)
                {
                    configuracion = new DetallePedidoAtributosLpn
                    {
                        Atributos = new List<DetallePedidoConfigAtributo>(),
                        CantidadPedida = atributo.CantidadPedida,
                        CantidadLiberada = atributo.CantidadLiberada,
                        CantidadAnulada = atributo.CantidadAnulada,
                        Cliente = atributo.Cliente,
                        Empresa = atributo.Empresa,
                        Faixa = atributo.Faixa,
                        FechaAlta = atributo.FechaAltaConfiguracion,
                        IdConfiguracion = atributo.IdConfiguracion,
                        Identificador = atributo.Identificador,
                        IdEspecificaIdentificador = atributo.IdEspecificaIdentificador,
                        Pedido = atributo.Pedido,
                        Producto = atributo.Producto,
                        IdLpnExterno = atributo.IdLpnExterno,
                        Tipo = atributo.Tipo,
                    };

                    configuraciones[keyConfiguracion] = configuracion;
                }

                configuracion.Atributos.Add(new DetallePedidoConfigAtributo
                {
                    FechaAlta = atributo.FechaAltaAtributo,
                    IdAtributo = atributo.IdAtributo,
                    IdCabezal = atributo.IdCabezal,
                    IdConfiguracion = atributo.IdConfiguracion,
                    Nombre = atributo.Nombre,
                    Valor = atributo.Valor,
                });
            }

            return configuraciones.Values;
        }

        #endregion

        #region PedidosAnulados

        public virtual async Task<List<APITask>> GetPedidosAnuladosPendientes(CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var sql = @"SELECT 
                                ID_OPERACION AS Id,
                                DT_OPERACION AS Fecha
                            FROM V_CONFIRMACIONES_PENDIENTES
                            WHERE CD_INTERFAZ_EXTERNA = :cdInterfazExterna 
                              AND FL_HABILITADA = 'S'
                            ORDER BY 
                                DT_OPERACION ASC, 
                                ID_OPERACION ASC";

                return _dapper.Query<APITask>(connection, sql, param: new { cdInterfazExterna = CInterfazExterna.PedidosAnulados }, commandType: CommandType.Text).ToList();
            }
        }

        public virtual async Task<List<long>> GenerarInterfaces(string keyPedido, Func<int?, string> GetGrupoConsulta, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var pedido = new Pedido()
                {
                    Id = keyPedido.Split('#')[0],
                    Cliente = keyPedido.Split('#')[1],
                    Empresa = int.Parse(keyPedido.Split('#')[2]),
                };

                logger.Debug($"Pedido anulado. NroPedido: {pedido.Id} - Cliente{pedido.Cliente} - Empresa {pedido.Empresa}");

                var interfazHabilitada = _parametroRepository.GetParamInterfazHabilitada(CInterfazExterna.PedidosAnulados, pedido.Empresa, connection, null);

                if (!interfazHabilitada)
                {
                    logger.Debug($"La interfaz {CInterfazExterna.PedidosAnulados} no esta habilitada para la empresa {pedido.Empresa}.");
                    return new List<long>();
                }

                long nuEjecucion = -2;
                long? nuTransaccion = null;

                try
                {
                    nuTransaccion = await _transaccionRepository.CreateTransaction("GenerarInterfaces", connection, null);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"Pedido anulado. NroPedido: {pedido.Id} - Cliente: {pedido.Cliente} - Empresa: {pedido.Empresa} - Error: {ex}");
                }

                if (nuTransaccion.HasValue)
                {
                    try
                    {
                        using (var tran = connection.BeginTransaction())
                        {
                            var context = new PedidoAnuladoBulkOperationContext();
                            var data = Map(pedido, nuTransaccion.Value, connection, tran, context);
                            var grupoConsulta = GetGrupoConsulta(pedido.Empresa);
                            nuEjecucion = await CrearEjecucion(data, grupoConsulta, connection, tran);
                            context.UpdateLogPedidoAnulado = GetLogPedidoAnuladoObject(context.NuLogsPedidoAnulado, nuEjecucion);

                            await InsertDuplicados(context, connection, tran);
                            await UpdateDuplicados(context, connection, tran);
                            await UpdatePedidoAnulado(context.UpdateLogPedidoAnulado, connection, tran);

                            tran.Commit();
                            logger.Debug($"Log Pedido anulado actualizado. Nro InterfazEjecucion: {nuEjecucion}");
                        }
                    }
                    catch (Exception ex)
                    {
                        nuEjecucion = -2;
                        logger.Error($"Pedido anulado. NroPedido: {pedido.Id} - Cliente: {pedido.Cliente} - Empresa: {pedido.Empresa} - Error: {ex}");

                        var anulaciones = GetDetallesPedidoAnulado(pedido, connection, null);
                        if (anulaciones != null)
                        {
                            var logsId = anulaciones.Select(a => a.Id).ToList();
                            var logsPedidoAnulado = GetLogPedidoAnuladoObject(logsId, nuEjecucion);

                            await UpdatePedidoAnulado(logsPedidoAnulado, connection, null);
                        }
                    }
                }

                return new List<long>() { nuEjecucion };
            }
        }

        public virtual List<PedidoAnulado> GetDetallesPedidoAnulado(Pedido pedido, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                    CD_APLICACAO as Aplicacion,
                    CD_CLIENTE as Cliente,
                    CD_EMPRESA as Empresa,
                    CD_FAIXA as Embalaje,
                    CD_FUNCIONARIO as Funcionario,
                    CD_PRODUTO as Producto,
                    DS_MOTIVO as Motivo,
                    DT_ADDROW as FechaInsercion,
                    ID_ESPECIFICA_IDENTIFICADOR as EspecificaIdentificadorId,
                    NU_IDENTIFICADOR as Identificador,
                    NU_INTERFAZ_EJECUCION as InterfazEjecucion, 
                    NU_LOG_PEDIDO_ANULADO as Id,
                    NU_PEDIDO as Pedido,
                    QT_ANULADO as CantidadAnulada
                FROM T_LOG_PEDIDO_ANULADO 
                WHERE NU_PEDIDO = :nuPedido AND CD_CLIENTE = :cliente AND CD_EMPRESA = :empresa AND NU_INTERFAZ_EJECUCION = -1";

            return _dapper.Query<PedidoAnulado>(connection, sql, param: new
            {
                nuPedido = pedido.Id,
                cliente = pedido.Cliente,
                empresa = pedido.Empresa,
            }, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual PedidosAnuladosResponse Map(Pedido pedido, long nuTransaccion, DbConnection connection, DbTransaction tran, PedidoAnuladoBulkOperationContext context)
        {
            var model = new PedidosAnuladosResponse();
            var agente = _agenteRepository.GetAgenteOrNull(pedido.Empresa, pedido.Cliente).Result;
            var anulaciones = GetDetallesPedidoAnulado(pedido, connection, tran);

            var modelPed = new PedidoAnuladoResponse()
            {
                Pedido = pedido.Id,
                Empresa = pedido.Empresa,
                CodigoAgente = agente.Codigo,
                TipoAgente = agente.Tipo,
            };

            if (anulaciones != null)
            {
                context.NuLogsPedidoAnulado = anulaciones.Select(a => a.Id).ToList();

                var detalles = anulaciones.GroupBy(a => new { a.Producto, a.Embalaje, a.Identificador, a.EspecificaIdentificadorId, a.Funcionario, a.Motivo, a.Aplicacion })
                    .Select(a => new PedidoAnuladoDetalleAuxResponse()
                    {
                        Producto = a.Key.Producto,
                        Embalaje = a.Key.Embalaje,
                        Identificador = a.Key.Identificador,
                        EspecificaIdentificador = a.Key.EspecificaIdentificadorId,
                        Motivo = a.Key.Motivo,
                        Aplicacion = a.Key.Aplicacion,
                        Funcionario = a.Key.Funcionario,
                        FechaAlta = a.Min(e => e.FechaInsercion)?.ToString(CDateFormats.DATE_ONLY),
                        CantidadAnulada = a.Sum(e => e.CantidadAnulada ?? 0),
                        IdAnulaciones = a.Select(e => e.Id).ToList(),
                    });

                var detallesAnulacionLpn = GetDetalleAnulacionesLpn(pedido, connection, tran);
                var atributosConfiguracion = new Dictionary<long, List<DetallePedidoAtributoDefinicion>>();
                var detAtributosDefinidos = GetAtributosDetalleDefinidos(pedido, connection, tran);

                foreach (var at in detAtributosDefinidos)
                {
                    if (!atributosConfiguracion.ContainsKey(at.IdConfiguracion))
                        atributosConfiguracion[at.IdConfiguracion] = new List<DetallePedidoAtributoDefinicion>();

                    atributosConfiguracion[at.IdConfiguracion].Add(at);
                }

                foreach (var det in detalles)
                {
                    #region Duplicados

                    var saldo = det.CantidadAnulada;
                    DetallePedidoDuplicado auxDup = null;

                    var auxFlujo = false;
                    if ((det.Identificador != ManejoIdentificadorDb.IdentificadorAuto && det.EspecificaIdentificador == "N"))
                        auxFlujo = true;

                    List<DetallePedidoDuplicado> detallesDuplicado = GetDetallesPedidoDuplicados(pedido, det, auxFlujo, connection, tran);

                    foreach (var detalleDup in detallesDuplicado)
                    {
                        var modelDetalleDuplicado = new DetallePedidoSalidaDuplicadoResponse()
                        {
                            IdLineaSistemaExterno = detalleDup.IdLineaSistemaExterno,
                            TipoLinea = detalleDup.TipoLinea,
                            Serializado = detalleDup.DatosSerializados,
                        };

                        detalleDup.Transaccion = nuTransaccion;

                        if (saldo > 0)
                        {
                            if (detalleDup.Identificador != ManejoIdentificadorDb.IdentificadorAuto || det.Identificador == ManejoIdentificadorDb.IdentificadorAuto)
                            {
                                //Flujo 1: Existe linea de duplicado para el detalle con el lote
                                //o
                                //Flujo 2: Se anulo una linea con lote (AUTO) del pedido, 
                                //Debo anular directamente la linea (AUTO) del duplciado.
                                //(cantExcluir va a ser 0 siempre)

                                var cantExcluir = (detalleDup.CantidadFacturada ?? 0) > 0 ? (detalleDup.CantidadFacturada ?? 0) : (detalleDup.CantidadExpedida ?? 0);
                                var saldoLinea = detalleDup.CantidadPedida - cantExcluir - (detalleDup.CantidadAnulada ?? 0);

                                if (saldoLinea > 0 && (detalleDup.IdEspecificaIdentificador == "S" || det.Identificador == ManejoIdentificadorDb.IdentificadorAuto))
                                {
                                    decimal cantAnulada = 0;
                                    if (saldo >= saldoLinea)
                                    {
                                        cantAnulada = saldoLinea;
                                        detalleDup.CantidadAnulada = detalleDup.CantidadPedida - cantExcluir;
                                        saldo -= saldoLinea;
                                    }
                                    else
                                    {
                                        cantAnulada = saldo ?? 0;
                                        detalleDup.CantidadAnulada = (detalleDup.CantidadAnulada ?? 0) + saldo;
                                        saldo = 0;
                                    }

                                    modelDetalleDuplicado.CantidadProducto = cantAnulada;

                                    det.Duplicados.Add(modelDetalleDuplicado);
                                    context.UpdateDuplicados.Add(GetDuplicadosSalidaEntity(detalleDup));
                                }
                                else if (auxFlujo)
                                {
                                    auxDup = detalleDup;
                                    continue;
                                }
                            }
                            else
                            {
                                //Flujo No Existe linea de duplicado para el detalle con el lote.
                                //Debo bajar del duplicado auto y crear un duplicado con el lote.
                                var saldoLinea = detalleDup.CantidadPedida - (detalleDup.CantidadAnulada ?? 0);

                                if (saldoLinea > 0)
                                {
                                    decimal cantAnulada = 0;
                                    if (saldo >= saldoLinea)
                                    {
                                        cantAnulada = saldoLinea;
                                        detalleDup.CantidadPedida = detalleDup.CantidadPedida - saldoLinea;
                                        saldo -= saldoLinea;
                                    }
                                    else
                                    {
                                        cantAnulada = saldo ?? 0;
                                        detalleDup.CantidadPedida = detalleDup.CantidadPedida - (saldo ?? 0);
                                        saldo = 0;
                                    }
                                    context.UpdateDuplicados.Add(GetDuplicadosSalidaEntity(detalleDup));

                                    if (auxDup != null)
                                    {
                                        auxDup.CantidadPedida = auxDup.CantidadPedida + cantAnulada;
                                        auxDup.CantidadAnulada = (auxDup.CantidadAnulada ?? 0) + cantAnulada;
                                        auxDup.Transaccion = detalleDup.Transaccion;
                                        context.UpdateDuplicados.Add(GetDuplicadosSalidaEntity(auxDup));
                                    }
                                    else
                                    {
                                        var newDuplicado = new DetallePedidoDuplicado()
                                        {
                                            Pedido = pedido.Id,
                                            Empresa = pedido.Empresa,
                                            Cliente = pedido.Cliente,
                                            Producto = det.Producto,
                                            Identificador = det.Identificador,
                                            Faixa = det.Embalaje,
                                            IdEspecificaIdentificador = det.EspecificaIdentificador,
                                            IdLineaSistemaExterno = detalleDup.IdLineaSistemaExterno,
                                            TipoLinea = detalleDup.TipoLinea,
                                            CantidadAnulada = cantAnulada,
                                            CantidadPedida = cantAnulada,
                                            CantidadExpedida = null,
                                            CantidadFacturada = null,
                                            DatosSerializados = detalleDup.DatosSerializados,
                                            Transaccion = detalleDup.Transaccion,
                                        };

                                        context.NewDuplicados.Add(GetDuplicadosSalidaEntity(newDuplicado));
                                    }

                                    modelDetalleDuplicado.CantidadProducto = cantAnulada;
                                    det.Duplicados.Add(modelDetalleDuplicado);
                                }
                            }
                        }
                    }

                    #endregion

                    #region LPNs

                    var detsLpn = detallesAnulacionLpn
                    .Where(d => d.TipoOperacion == TipoAnulacionLpn.PedidoLpn
                        && det.IdAnulaciones.Contains(d.IdLogPedidoAnulado));

                    foreach (var dp in detsLpn)
                    {
                        var detsLpnAtributo = detallesAnulacionLpn
                            .Where(d => d.TipoOperacion == TipoAnulacionLpn.PedidoLpnAtributo
                                && d.IdExternoLpn == dp.IdExternoLpn
                                && d.TipoLpn == dp.TipoLpn
                                && d.IdLogPedidoAnulado == dp.IdLogPedidoAnulado);

                        var detallesAtributos = new List<DetallePedidoAtributoAnuladoResponse>();
                        foreach (var dla in detsLpnAtributo)
                        {
                            var atributos = atributosConfiguracion[dla.IdConfiguracion.Value]
                                .Select(a => new AtributoPedidoAnuladoResponse()
                                {
                                    Nombre = a.Nombre,
                                    Valor = a.Valor,
                                    Tipo = a.IdCabezal == "S" ? TipoAtributoDb.CABEZAL : TipoAtributoDb.DETALLE,
                                })
                                .ToList();

                            detallesAtributos.Add(new DetallePedidoAtributoAnuladoResponse()
                            {
                                CantidadAnulada = dla.CantidadAnulada,
                                Atributos = atributos
                            });
                        }

                        det.Lpns.Add(new DetallePedidoLpnAnuladoResponse()
                        {
                            IdExterno = dp.IdExternoLpn,
                            Tipo = dp.TipoLpn,
                            CantidadAnulada = dp.CantidadAnulada,
                            //DetallesAtributos = detallesAtributos
                        });
                    }

                    var detsAtributos = detallesAnulacionLpn
                    .Where(d => d.TipoOperacion == TipoAnulacionLpn.PedidoAtributo
                        && det.IdAnulaciones.Contains(d.IdLogPedidoAnulado));

                    foreach (var da in detsAtributos)
                    {
                        var atributos = atributosConfiguracion[da.IdConfiguracion.Value]
                            .Select(a => new AtributoPedidoAnuladoResponse()
                            {
                                Nombre = a.Nombre,
                                Valor = a.Valor,
                                Tipo = a.IdCabezal == "S" ? TipoAtributoDb.CABEZAL : TipoAtributoDb.DETALLE,
                            })
                            .ToList();

                        det.Atributos.Add(new DetallePedidoAtributoAnuladoResponse()
                        {
                            CantidadAnulada = da.CantidadAnulada,
                            Atributos = atributos
                        });
                    }

                    #endregion

                    var newDetalle = det.GetDetalleFinal();
                    modelPed.Detalles.Add(newDetalle);
                }
            }
            model.PedidosAnulados.Add(modelPed);
            return model;
        }

        public virtual IEnumerable<Pedido> GetPedidos(IEnumerable<Pedido> pedidos)
        {
            IEnumerable<Pedido> resultado = new List<Pedido>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PEDIDO_SAIDA_TEMP (NU_PEDIDO, CD_EMPRESA, CD_CLIENTE) VALUES (:Id, :Empresa, :Cliente)";
                    _dapper.Execute(connection, sql, pedidos, transaction: tran);

                    sql = GetSqlSelectPedido() +
                        @" INNER JOIN T_PEDIDO_SAIDA_TEMP T ON P.NU_PEDIDO = T.NU_PEDIDO 
                            AND P.CD_EMPRESA = T.CD_EMPRESA
                            AND P.CD_CLIENTE = T.CD_CLIENTE";

                    resultado = _dapper.Query<Pedido>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<Pedido> GetPedidosContext(IEnumerable<Pedido> keys)
        {
            var sql = @"INSERT INTO T_PEDIDO_SAIDA_TEMP (NU_PEDIDO, CD_EMPRESA, CD_CLIENTE) VALUES (:Id, :Empresa, :Cliente)";
            _dapper.Execute(_context.Database.GetDbConnection(), sql, keys, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = GetSqlSelectPedido() +
                @" INNER JOIN T_PEDIDO_SAIDA_TEMP T ON P.NU_PEDIDO = T.NU_PEDIDO 
                    AND P.CD_EMPRESA = T.CD_EMPRESA
                    AND P.CD_CLIENTE = T.CD_CLIENTE";

            var pedidos = _dapper.Query<Pedido>(_context.Database.GetDbConnection(), sql, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @"DELETE T_PEDIDO_SAIDA_TEMP 
                    WHERE NU_PEDIDO = :Id 
                        AND CD_EMPRESA = :Empresa
                        AND CD_CLIENTE = :Cliente ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, keys, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return pedidos;
        }

        public virtual IEnumerable<DetallePedido> GetDetallePedidos(IEnumerable<Pedido> keys)
        {
            var sql = @"INSERT INTO T_PEDIDO_SAIDA_TEMP (NU_PEDIDO, CD_EMPRESA, CD_CLIENTE) VALUES (:Id, :Empresa, :Cliente)";
            _dapper.Execute(_context.Database.GetDbConnection(), sql, keys, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = GetSqlSelectPedidoDetalle() +
                    @" INNER JOIN T_PEDIDO_SAIDA_TEMP T ON DP.NU_PEDIDO = T.NU_PEDIDO 
                    AND DP.CD_EMPRESA = T.CD_EMPRESA
                    AND DP.CD_CLIENTE = T.CD_CLIENTE";

            var detalles = _dapper.Query<DetallePedido>(_context.Database.GetDbConnection(), sql, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            sql = @"DELETE T_PEDIDO_SAIDA_TEMP 
                    WHERE NU_PEDIDO = :Id 
                        AND CD_EMPRESA = :Empresa
                        AND CD_CLIENTE = :Cliente ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, keys, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            return detalles;
        }

        public virtual IEnumerable<PedidoSaldo> GetPedidosWithSaldo(IEnumerable<Pedido> pedidos)
        {
            IEnumerable<PedidoSaldo> resultado = new List<PedidoSaldo>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PEDIDO_SAIDA_TEMP (NU_PEDIDO, CD_EMPRESA, CD_CLIENTE) VALUES (:Id, :Empresa, :Cliente)";
                    _dapper.Execute(connection, sql, pedidos, transaction: tran);

                    sql = GetSqlSelectPedido(", V.QT_PEDIDO_TOTAL as Total, V.QT_SALDO as Saldo ") +
                        @" INNER JOIN T_PEDIDO_SAIDA_TEMP T ON P.NU_PEDIDO = T.NU_PEDIDO 
                                AND P.CD_EMPRESA = T.CD_EMPRESA
                                AND P.CD_CLIENTE = T.CD_CLIENTE
                           INNER JOIN V_SALDO_TOTAL_PEDIDO V ON P.NU_PEDIDO = V.NU_PEDIDO 
                                AND P.CD_EMPRESA = V.CD_EMPRESA 
                                AND P.CD_CLIENTE = V.CD_CLIENTE ";

                    resultado = _dapper.Query<PedidoSaldo>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual List<DetallePedidoLpn> GetDetallesPedidosLpn(IEnumerable<Pedido> pedidos)
        {
            List<DetallePedidoLpn> resultado = new List<DetallePedidoLpn>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PEDIDO_SAIDA_TEMP (NU_PEDIDO, CD_EMPRESA, CD_CLIENTE) VALUES (:Id, :Empresa, :Cliente)";
                    _dapper.Execute(connection, sql, pedidos, transaction: tran);

                    sql = GetSqlSelectDetallePedidoLpn() +
                        @" INNER JOIN T_PEDIDO_SAIDA_TEMP T ON P.NU_PEDIDO = T.NU_PEDIDO 
                            AND P.CD_EMPRESA = T.CD_EMPRESA
                            AND P.CD_CLIENTE = T.CD_CLIENTE";

                    resultado = _dapper.Query<DetallePedidoLpn>(connection, sql, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public static string GetSqlSelectDetallePedidoLpn()
        {
            return @"SELECT 
                        P.NU_PEDIDO as Pedido,
                        P.CD_CLIENTE as Cliente,
                        P.CD_EMPRESA as Empresa,
                        P.CD_PRODUTO as Producto,
                        P.CD_FAIXA as Faixa,
                        P.NU_IDENTIFICADOR as Identificador,
                        P.ID_ESPECIFICA_IDENTIFICADOR as IdEspecificaIdentificador,
                        P.QT_PEDIDO as CantidadPedida,
                        P.DT_ADDROW as FechaAlta,
                        P.DT_UPDROW as FechaModificacion,
                        P.NU_TRANSACCION as Transaccion,
                        P.ID_LPN_EXTERNO as IdLpnExterno,
                        P.TP_LPN_TIPO as Tipo,
                        P.QT_LIBERADO as CantidadLiberada,
                        P.QT_ANULADO as CantidadAnulada,
                        P.NU_LPN as NumeroLpn,
                        P.NU_TRANSACCION_DELETE as TransaccionDelete
                FROM T_DET_PEDIDO_SAIDA_LPN P ";
        }

        public virtual async Task<long> CrearEjecucion(PedidosAnuladosResponse pedido, string grupoConsulta, DbConnection connection, DbTransaction tran)
        {
            var ejecucionRepository = new EjecucionRepository(_dapper);

            var ped = pedido.PedidosAnulados.FirstOrDefault();
            var interfaz = new InterfazEjecucion
            {
                CdInterfazExterna = CInterfazExterna.PedidosAnulados,
                Situacion = SituacionDb.ProcesadoPendiente,
                Comienzo = DateTime.Now,
                FechaSituacion = DateTime.Now,
                ErrorCarga = "N",
                ErrorProcedimiento = "N",
                Referencia = $"Interfaz de Pedidos anulados. Pedido: {ped.Pedido} - Empresa: {ped.Empresa} - Agente: {ped.CodigoAgente} - TipoAgente: {ped.TipoAgente}",
                Empresa = ped.Empresa,
                GrupoConsulta = grupoConsulta
            };

            var data = JsonConvert.SerializeObject(pedido);
            var itfzData = new InterfazData
            {
                Alta = DateTime.Now,
                Data = Encoding.UTF8.GetBytes(data)
            };

            interfaz = await ejecucionRepository.AddEjecucion(interfaz, itfzData, connection, tran);

            return interfaz.Id;
        }

        public virtual async Task UpdatePedidoAnulado(List<object> anulaciones, DbConnection connection, DbTransaction tran)
        {
            string sql = @"UPDATE T_LOG_PEDIDO_ANULADO 
                SET NU_INTERFAZ_EJECUCION = :InterfazEjecucion
                WHERE NU_LOG_PEDIDO_ANULADO = :Id";

            await _dapper.ExecuteAsync(connection, sql, anulaciones, transaction: tran);
        }

        public virtual List<DetallePedidoDuplicado> GetDetallesPedidoDuplicados(Pedido pedido, PedidoAnuladoDetalleAuxResponse detPedido, bool auxFlujo, DbConnection connection, DbTransaction tran)
        {
            var result = new List<DetallePedidoDuplicado>();
            var param = new DynamicParameters(new
            {
                nuPedido = pedido.Id,
                cliente = pedido.Cliente,
                empresa = pedido.Empresa,
                producto = detPedido.Producto,
                faixa = detPedido.Embalaje,
                identificador = detPedido.Identificador,
                especificaLote = detPedido.EspecificaIdentificador
            });

            string sql = @"SELECT 
                            NU_PEDIDO as Pedido,
                            CD_CLIENTE as Cliente,
                            CD_EMPRESA as Empresa,
                            CD_PRODUTO as Producto,
                            CD_FAIXA as Faixa, 
                            NU_IDENTIFICADOR as Identificador,
                            ID_ESPECIFICA_IDENTIFICADOR as IdEspecificaIdentificador,
                            ID_LINEA_SISTEMA_EXTERNO as IdLineaSistemaExterno,
                            TP_LINEA as TipoLinea,
                            QT_PEDIDO as CantidadPedida,
                            QT_EXPEDIDO as CantidadExpedida,
                            QT_ANULADO as CantidadAnulada,
                            QT_FACTURADO as CantidadFacturada,
                            DT_ADDROW as FechaAlta,
                            DT_UPDROW as FechaModificacion,
                            VL_SERIALIZADO_1 as DatosSerializados
                        FROM T_DET_PEDIDO_SAIDA_DUP
                        WHERE NU_PEDIDO = :nuPedido AND CD_CLIENTE = :cliente AND CD_EMPRESA = :empresa
                            AND CD_PRODUTO = :producto AND CD_FAIXA = :faixa AND NU_IDENTIFICADOR = :identificador
                            AND ID_ESPECIFICA_IDENTIFICADOR = :especificaLote ORDER BY QT_EXPEDIDO, QT_FACTURADO, ID_LINEA_SISTEMA_EXTERNO";

            result = _dapper.Query<DetallePedidoDuplicado>(connection, sql, param: param, commandType: CommandType.Text, transaction: tran).ToList();

            if (result == null || result.Count == 0 || auxFlujo)
            {
                sql = @"SELECT 
                        NU_PEDIDO as Pedido,
                        CD_CLIENTE as Cliente,
                        CD_EMPRESA as Empresa,
                        CD_PRODUTO as Producto,
                        CD_FAIXA as Faixa, 
                        NU_IDENTIFICADOR as Identificador,
                        ID_ESPECIFICA_IDENTIFICADOR as IdEspecificaIdentificador,
                        ID_LINEA_SISTEMA_EXTERNO as IdLineaSistemaExterno,
                        TP_LINEA as TipoLinea,
                        QT_PEDIDO as CantidadPedida,
                        QT_EXPEDIDO as CantidadExpedida,
                        QT_ANULADO as CantidadAnulada,
                        QT_FACTURADO as CantidadFacturada,
                        DT_ADDROW as FechaAlta,
                        DT_UPDROW as FechaModificacion,
                        VL_SERIALIZADO_1 as DatosSerializados
                        FROM T_DET_PEDIDO_SAIDA_DUP
                        WHERE NU_PEDIDO = :nuPedido AND CD_CLIENTE = :cliente AND CD_EMPRESA = :empresa
                        AND CD_PRODUTO = :producto AND CD_FAIXA = :faixa AND NU_IDENTIFICADOR = '(AUTO)'
                        AND ID_ESPECIFICA_IDENTIFICADOR = :especificaLote ORDER BY QT_EXPEDIDO, QT_FACTURADO, ID_LINEA_SISTEMA_EXTERNO";

                if (auxFlujo)
                    result.AddRange(_dapper.Query<DetallePedidoDuplicado>(connection, sql, param: param, commandType: CommandType.Text, transaction: tran).ToList());
                else
                    result = _dapper.Query<DetallePedidoDuplicado>(connection, sql, param: param, commandType: CommandType.Text, transaction: tran).ToList();
            }

            return result;
        }

        public virtual List<PedidoAnuladoLpn> GetDetalleAnulacionesLpn(Pedido pedido, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            lpal.NU_LOG_PEDIDO_ANULADO_LPN as Id,
                            lpal.NU_LOG_PEDIDO_ANULADO as IdLogPedidoAnulado,
                            lpal.TP_OPERACION as TipoOperacion,
                            lpal.ID_LPN_EXTERNO as IdExternoLpn,
                            lpal.TP_LPN_TIPO as TipoLpn, 
                            lpal.NU_DET_PED_SAI_ATRIB as IdConfiguracion,
                            lpal.QT_ANULADO as CantidadAnulada,
                            lpal.DT_ADDROW as FechaInsercion,
                            lpal.DT_UPDROW as FechaModificacion
                        FROM T_LOG_PEDIDO_ANULADO lpa
                        INNER JOIN T_LOG_PEDIDO_ANULADO_LPN lpal ON lpal.NU_LOG_PEDIDO_ANULADO = lpa.NU_LOG_PEDIDO_ANULADO
                        WHERE lpa.NU_PEDIDO = :nuPedido 
                            AND lpa.CD_CLIENTE = :cliente 
                            AND lpa.CD_EMPRESA = :empresa 
                            AND lpa.NU_INTERFAZ_EJECUCION = -1";

            return _dapper.Query<PedidoAnuladoLpn>(connection, sql, param: new
            {
                nuPedido = pedido.Id,
                cliente = pedido.Cliente,
                empresa = pedido.Empresa,
            }, commandType: CommandType.Text, transaction: tran).ToList();

        }

        public virtual List<DetallePedidoAtributoDefinicion> GetAtributosDetalleDefinidos(Pedido pedido, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            da.NU_DET_PED_SAI_ATRIB as IdConfiguracion,
                            da.ID_ATRIBUTO as IdAtributo,
                            da.FL_CABEZAL as IdCabezal,
                            da.VL_ATRIBUTO as Valor,
                            da.DT_ADDROW as FechaAlta,
                            da.DT_UPDROW as FechaModificacion,
                            da.NU_TRANSACCION as Transaccion, 
                            da.NU_TRANSACCION_DELETE as TransaccionDelete,
                            a.NM_ATRIBUTO as Nombre
                        FROM T_LOG_PEDIDO_ANULADO lpa
                        INNER JOIN T_LOG_PEDIDO_ANULADO_LPN lpal ON lpal.NU_LOG_PEDIDO_ANULADO = lpa.NU_LOG_PEDIDO_ANULADO
                        INNER JOIN T_DET_PEDIDO_SAIDA_ATRIB_DET da ON da.NU_DET_PED_SAI_ATRIB = lpal.NU_DET_PED_SAI_ATRIB
                        INNER JOIN T_ATRIBUTO a ON da.ID_ATRIBUTO = a.ID_ATRIBUTO
                        WHERE da.FL_CABEZAL = 'N' 
                            AND lpa.NU_PEDIDO = :nuPedido 
                            AND lpa.CD_CLIENTE = :cliente 
                            AND lpa.CD_EMPRESA = :empresa 
                            AND lpa.NU_INTERFAZ_EJECUCION = -1";

            return _dapper.Query<DetallePedidoAtributoDefinicion>(connection, sql, param: new
            {
                nuPedido = pedido.Id,
                cliente = pedido.Cliente,
                empresa = pedido.Empresa,
            }, commandType: CommandType.Text, transaction: tran).ToList();

        }

        public virtual async Task InsertDuplicados(PedidoAnuladoBulkOperationContext context, DbConnection connection, DbTransaction tran)
        {
            await BulkInsertNewDuplicados(connection, context.NewDuplicados, tran);
        }

        public virtual async Task UpdateDuplicados(PedidoAnuladoBulkOperationContext context, DbConnection connection, DbTransaction tran)
        {
            await BulkUpdateDuplicados(connection, context.UpdateDuplicados, tran);
        }

        public virtual async Task BulkInsertNewDuplicados(DbConnection connection, List<object> duplicados, DbTransaction tran)
        {
            var sql = @" INSERT INTO T_DET_PEDIDO_SAIDA_DUP
                        (NU_PEDIDO,
                        CD_CLIENTE,
                        CD_EMPRESA,
                        CD_PRODUTO, 
                        CD_FAIXA,
                        NU_IDENTIFICADOR,                       
                        ID_ESPECIFICA_IDENTIFICADOR,
                        ID_LINEA_SISTEMA_EXTERNO,
                        TP_LINEA,
                        QT_PEDIDO,
                        QT_ANULADO,
                        DT_ADDROW,
                        DT_UPDROW,
                        NU_TRANSACCION,
                        VL_SERIALIZADO_1) 
                        VALUES(
                        :nuPedido,
                        :cliente,
                        :empresa,                     
                        :producto,                      
                        :faixa,                       
                        :identificador, 
                        :especificaLote, 
                        :idLineaSistemaExterno,
                        :TipoLinea,
                        :CantidadPedida,               
                        :CantidadAnulada,
                        :FechaAlta,
                        :FechaModificacion,
                        :FechaAlta,
                        :FechaAlta,
                        :Transaccion,
                        :DatosSerializados)";
            await _dapper.ExecuteAsync(connection, sql, duplicados, transaction: tran);
        }

        public virtual async Task BulkUpdateDuplicados(DbConnection connection, List<object> duplicados, DbTransaction tran)
        {
            string sql = @"UPDATE T_DET_PEDIDO_SAIDA_DUP SET QT_ANULADO = :cantidadAnulada, QT_PEDIDO = :CantidadPedida, NU_TRANSACCION = :Transaccion, DT_UPDROW = :FechaModificacion
                           WHERE NU_PEDIDO = :nuPedido AND CD_CLIENTE = :cliente AND CD_EMPRESA = :empresa
                           AND CD_PRODUTO = :producto AND CD_FAIXA = :faixa AND NU_IDENTIFICADOR = :identificador AND ID_ESPECIFICA_IDENTIFICADOR = :especificaLote
                           AND ID_LINEA_SISTEMA_EXTERNO = :idLineaSistemaExterno ";

            await _dapper.ExecuteAsync(connection, sql, duplicados, transaction: tran);
        }

        public virtual async Task BulkUpdateCantDuplicados(DbConnection connection, List<object> duplicados, DbTransaction tran)
        {
            string sql = @"UPDATE T_DET_PEDIDO_SAIDA_DUP SET QT_ANULADO = :cantidadAnulada, NU_TRANSACCION = :Transaccion, DT_UPDROW = :FechaModificacion
                           WHERE NU_PEDIDO = :nuPedido AND CD_CLIENTE = :cliente AND CD_EMPRESA = :empresa
                           AND CD_PRODUTO = :producto AND CD_FAIXA = :faixa AND NU_IDENTIFICADOR =:identificador AND ID_ESPECIFICA_IDENTIFICADOR = :especificaLote
                           AND ID_LINEA_SISTEMA_EXTERNO = :idLineaSistemaExterno ";

            await _dapper.ExecuteAsync(connection, sql, duplicados, transaction: tran);
        }

        public static object GetDuplicadosSalidaEntity(DetallePedidoDuplicado dup)
        {
            return new
            {
                nuPedido = dup.Pedido,
                cliente = dup.Cliente,
                empresa = dup.Empresa,
                faixa = dup.Faixa,
                producto = dup.Producto,
                identificador = dup.Identificador,
                especificaLote = dup.IdEspecificaIdentificador,
                idLineaSistemaExterno = dup.IdLineaSistemaExterno,
                cantidadAnulada = dup.CantidadAnulada,
                TipoLinea = dup.TipoLinea,
                DatosSerializados = dup.DatosSerializados,
                CantidadExpedida = dup.CantidadExpedida,
                CantidadFacturada = dup.CantidadFacturada,
                CantidadPedida = dup.CantidadPedida,
                Transaccion = dup.Transaccion,
                FechaAlta = dup.FechaAlta ?? DateTime.Now,
                FechaModificacion = DateTime.Now,
            };
        }

        public virtual List<object> GetLogPedidoAnuladoObject(List<long> nuLogsPedidoAnulado, long nuEjecucion)
        {
            var result = new List<object>();

            foreach (var id in nuLogsPedidoAnulado)
            {
                result.Add(new
                {
                    Id = id,
                    InterfazEjecucion = nuEjecucion
                });
            }

            return result;
        }

        #endregion

        #region Delete
        public virtual async Task BulkDeleteDetalleLpn(DbConnection connection, DbTransaction tran, List<DetallePedidoLpn> detalleLpn)
        {
            var sql = @"
                DELETE T_DET_PEDIDO_SAIDA_LPN
                WHERE NU_PEDIDO = :Pedido 
                    AND CD_CLIENTE = :Cliente 
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto 
                    AND CD_FAIXA = :Faixa 
                    AND NU_IDENTIFICADOR = :Identificador 
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador 
                    AND ID_LPN_EXTERNO = :IdLpnExterno
                    AND TP_LPN_TIPO = :Tipo";

            await _dapper.ExecuteAsync(connection, sql, detalleLpn, transaction: tran);
        }
        #endregion

        #region Tracking

        public virtual IEnumerable<Pedido> GetPedidosPendientes(IEnumerable<Agente> agentes, string puntoEntrega)
        {
            IEnumerable<Pedido> resultado = new List<Pedido>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PEDIDO_SAIDA_TEMP (CD_EMPRESA, CD_CLIENTE) VALUES (:Empresa, :CodigoInterno)";
                    _dapper.Execute(connection, sql, agentes, transaction: tran);

                    sql = GetSqlSelectPedido() +
                        @"  INNER JOIN V_API_PEDIDOS_CON_PEND PSP ON P.NU_PEDIDO = PSP.NU_PEDIDO AND P.CD_EMPRESA = PSP.CD_EMPRESA AND P.CD_CLIENTE = PSP.CD_CLIENTE
                            INNER JOIN T_PEDIDO_SAIDA_TEMP T ON P.CD_EMPRESA = T.CD_EMPRESA AND P.CD_CLIENTE = T.CD_CLIENTE
                            WHERE P.CD_PUNTO_ENTREGA= :puntoEntrega ";

                    resultado = _dapper.Query<Pedido>(connection, sql, param: new { puntoEntrega = puntoEntrega }, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual async Task ActualizarPedidosPuntoEntrega(PuntoEntregaAgentes puntoEntrega, IPuntoEntregaServiceContext context, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    var bulkContext = await GetBulkOperationContext(puntoEntrega, context, connection, tran);

                    await BulkUpdateRutaPedidos(connection, tran, bulkContext.UpdPedidos);
                    tran.Commit();
                }
            }
        }

        public virtual async Task<PedidoUpdateBulkOperationContext> GetBulkOperationContext(PuntoEntregaAgentes puntoEntrega, IPuntoEntregaServiceContext serviceContext, DbConnection connection, DbTransaction tran)
        {
            var context = new PedidoUpdateBulkOperationContext();

            var ruta = serviceContext.RutaZona;
            var pedidos = serviceContext.PedidosPendientes;
            var nuTransaccion = await _transaccionRepository.CreateTransaction("ActualizarPedidosPuntoEntrega", connection, tran, app: "UpdatePedidosPuntoEntrega");

            foreach (var pedido in pedidos)
            {
                pedido.Ruta = ruta?.Id;
                pedido.Transaccion = nuTransaccion;
                pedido.FechaModificacion = DateTime.Now;
                context.UpdPedidos.Add(GetPedidoEntityUpdate(pedido));
            }

            return context;
        }

        public virtual async Task BulkUpdateRutaPedidos(DbConnection connection, DbTransaction tran, List<object> pedidos)
        {
            string sql = @"UPDATE T_PEDIDO_SAIDA 
                           SET CD_ROTA = :Ruta,
                           NU_TRANSACCION = :Transaccion,
                           DT_UPDROW = :FechaModificacion 
                           WHERE NU_PEDIDO = :Id AND CD_CLIENTE = :Cliente AND CD_EMPRESA = :Empresa";

            await _dapper.ExecuteAsync(connection, sql, pedidos, transaction: tran);
        }

        #endregion

        #region PRE100

        public virtual void AsociarLpn(Pedido pedido, List<Lpn> lpns, long nuTransaccion)
        {
            var detallesLpn = GetDetallesLpnParaPedido(lpns);

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                var bulkContext = GetBulkOperationContext(pedido, detallesLpn, connection, nuTransaccion);

                using (var tran = connection.BeginTransaction())
                {
                    AsociarLpnInsertDetallesPedido(connection, tran, bulkContext.NewDetalles.Values);
                    AsociarLpnUpdateDetallesPedido(connection, tran, bulkContext.UpdateDetalles.Values);
                    AsociarLpnInsertDetallesPedidoLpn(connection, tran, bulkContext.NewDetallesLpn.Values);
                    AsociarLpnUpdateDetallesPedidoLpn(connection, tran, bulkContext.UpdateDetallesLpn.Values);

                    tran.Commit();
                }
            }

        }

        public virtual IEnumerable<LpnDetalle> GetDetallesLpnParaPedido(List<Lpn> lpns)
        {
            IEnumerable<LpnDetalle> detallesLpn = new List<LpnDetalle>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_LPN_TEMP (ID_LPN_EXTERNO, TP_LPN_TIPO) VALUES (:IdExterno, :Tipo)";
                    _dapper.Execute(connection, sql, lpns, transaction: tran);

                    sql = @"SELECT 
                                LD.NU_LPN as NumeroLPN,
	                            L.ID_LPN_EXTERNO as IdExterno,
	                            L.TP_LPN_TIPO as Tipo,
	                            LD.CD_EMPRESA as Empresa,
	                            LD.CD_PRODUTO as CodigoProducto,
	                            LD.CD_FAIXA as Faixa,
	                            LD.NU_IDENTIFICADOR as Lote,
	                            SUM(COALESCE(LD.QT_ESTOQUE,0) - COALESCE(LD.QT_RESERVA_SAIDA,0)) as Cantidad
                            FROM T_LPN_DET LD 
                            INNER JOIN T_LPN L ON LD.NU_LPN = L.NU_LPN
                            INNER JOIN T_LPN_TEMP T ON L.ID_LPN_EXTERNO = T.ID_LPN_EXTERNO AND L.TP_LPN_TIPO = T.TP_LPN_TIPO 
                            WHERE COALESCE(LD.QT_ESTOQUE,0) - COALESCE(LD.QT_RESERVA_SAIDA,0) > 0 AND L.ID_ESTADO = 'LPNACT'
                            GROUP BY
	                            LD.NU_LPN,
	                            L.ID_LPN_EXTERNO,
	                            L.TP_LPN_TIPO,
	                            LD.CD_EMPRESA, 
	                            LD.CD_PRODUTO, 
	                            LD.CD_FAIXA, 
	                            LD.NU_IDENTIFICADOR ";

                    detallesLpn = _dapper.Query<LpnDetalle>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return detallesLpn;
        }

        public virtual PedidoGenerarDetalleLpnBulkOperationContext GetBulkOperationContext(Pedido pedido, IEnumerable<LpnDetalle> detallesLpn, DbConnection connection, long nuTransaccion)
        {
            var context = new PedidoGenerarDetalleLpnBulkOperationContext();

            var param = new DynamicParameters(new
            {
                nuPedido = pedido.Id,
                empresa = pedido.Empresa,
                cliente = pedido.Cliente,
            });

            string sql = GetSqlSelectPedidoDetalle() +
                @" WHERE DP.NU_PEDIDO = :nuPedido AND DP.CD_EMPRESA = :empresa AND DP.CD_CLIENTE = :cliente ";

            var detallesPedido = _dapper.Query<DetallePedido>(connection, sql, param: param, commandType: CommandType.Text);

            sql = GetSqlSelectDetallePedidoLpn() +
                @" WHERE P.NU_PEDIDO = :nuPedido AND P.CD_EMPRESA = :empresa AND P.CD_CLIENTE = :cliente ";

            var detallesPedidoLpn = _dapper.Query<DetallePedidoLpn>(connection, sql, param: param, commandType: CommandType.Text);

            foreach (var det in detallesLpn)
            {
                var detExistente = detallesPedido
                    .FirstOrDefault(d => d.Id == pedido.Id
                        && d.Cliente == pedido.Cliente
                        && d.Empresa == pedido.Empresa
                        && d.Producto == det.CodigoProducto
                        && d.Faixa == det.Faixa
                        && d.Identificador == det.Lote
                        && d.EspecificaIdentificadorId == "S");

                var keyDetallePedido = $"{pedido.Id}.{pedido.Cliente}.{pedido.Empresa}.{det.CodigoProducto}.{det.Faixa.ToString("#.###")}.{det.Lote}.S";

                if (detExistente == null)
                {
                    if (!context.NewDetalles.ContainsKey(keyDetallePedido))
                    {
                        context.NewDetalles[keyDetallePedido] = new DetallePedido()
                        {
                            Id = pedido.Id,
                            Cliente = pedido.Cliente,
                            Empresa = pedido.Empresa,
                            Producto = det.CodigoProducto,
                            Faixa = det.Faixa,
                            Identificador = det.Lote,
                            EspecificaIdentificadorId = "S",
                            Agrupacion = Agrupacion.Pedido,
                            Cantidad = det.Cantidad,
                            CantidadAnulada = 0,
                            CantidadLiberada = 0,
                            CantidadOriginal = det.Cantidad,
                            FechaAlta = DateTime.Now,
                            Transaccion = nuTransaccion
                        };
                    }
                    else
                    {
                        context.NewDetalles[keyDetallePedido].Cantidad += det.Cantidad;
                        context.NewDetalles[keyDetallePedido].CantidadOriginal += det.Cantidad;
                    }
                }
                else
                {
                    if (!context.UpdateDetalles.ContainsKey(keyDetallePedido))
                    {
                        context.UpdateDetalles[keyDetallePedido] = new DetallePedido()
                        {
                            Id = pedido.Id,
                            Cliente = pedido.Cliente,
                            Empresa = pedido.Empresa,
                            Producto = det.CodigoProducto,
                            Faixa = det.Faixa,
                            Identificador = det.Lote,
                            EspecificaIdentificadorId = "S",
                            Cantidad = det.Cantidad,
                            Transaccion = nuTransaccion,
                            FechaModificacion = DateTime.Now
                        };

                    }
                    else
                        context.UpdateDetalles[keyDetallePedido].Cantidad += det.Cantidad;
                }

                var detLpnExistente = detallesPedidoLpn
                    .FirstOrDefault(d => d.Pedido == pedido.Id
                        && d.Cliente == pedido.Cliente
                        && d.Empresa == pedido.Empresa
                        && d.Producto == det.CodigoProducto
                        && d.Faixa == det.Faixa
                        && d.Identificador == det.Lote
                        && d.IdEspecificaIdentificador == "S"
                        && d.Tipo == det.Tipo
                        && d.IdLpnExterno == det.IdExterno);

                var keyDetallePedidoLpn = $"{pedido.Id}.{pedido.Cliente}.{pedido.Empresa}.{det.CodigoProducto}.{det.Faixa.ToString("#.###")}.{det.Lote}.S.{det.Tipo}.{det.IdExterno}";

                if (detLpnExistente == null)
                {
                    if (!context.NewDetallesLpn.ContainsKey(keyDetallePedidoLpn))
                    {
                        context.NewDetallesLpn[keyDetallePedidoLpn] = new DetallePedidoLpn()
                        {
                            Pedido = pedido.Id,
                            Cliente = pedido.Cliente,
                            Empresa = pedido.Empresa,
                            Producto = det.CodigoProducto,
                            Faixa = det.Faixa,
                            Identificador = det.Lote,
                            IdEspecificaIdentificador = "S",
                            CantidadPedida = det.Cantidad,
                            CantidadAnulada = 0,
                            CantidadLiberada = 0,
                            FechaAlta = DateTime.Now,
                            Transaccion = nuTransaccion,
                            IdLpnExterno = det.IdExterno,
                            Tipo = det.Tipo,
                            NumeroLpn = det.NumeroLPN
                        };
                    }
                    else
                    {
                        context.NewDetallesLpn[keyDetallePedidoLpn].CantidadPedida += det.Cantidad;
                    }
                }
                else
                {
                    if (!context.UpdateDetallesLpn.ContainsKey(keyDetallePedidoLpn))
                    {
                        context.UpdateDetallesLpn[keyDetallePedidoLpn] = new DetallePedidoLpn()
                        {
                            Pedido = pedido.Id,
                            Cliente = pedido.Cliente,
                            Empresa = pedido.Empresa,
                            Producto = det.CodigoProducto,
                            Faixa = det.Faixa,
                            Identificador = det.Lote,
                            IdEspecificaIdentificador = "S",
                            CantidadPedida = det.Cantidad,
                            FechaModificacion = DateTime.Now,
                            Transaccion = nuTransaccion,
                            IdLpnExterno = det.IdExterno,
                            Tipo = det.Tipo,
                            NumeroLpn = det.NumeroLPN
                        };
                    }
                    else
                        context.UpdateDetallesLpn[keyDetallePedidoLpn].CantidadPedida += det.Cantidad;
                }

            }

            return context;
        }

        public virtual void AsociarLpnInsertDetallesPedido(DbConnection connection, DbTransaction tran, IEnumerable<DetallePedido> detalles)
        {
            var sql = @" INSERT INTO T_DET_PEDIDO_SAIDA
                        (NU_PEDIDO,
                        CD_CLIENTE,
                        CD_EMPRESA,
                        CD_PRODUTO,
                        CD_FAIXA,
                        NU_IDENTIFICADOR,
                        ID_ESPECIFICA_IDENTIFICADOR,
                        ID_AGRUPACION,
                        QT_PEDIDO,
                        QT_ANULADO,
                        QT_LIBERADO,
                        QT_PEDIDO_ORIGINAL,
                        DT_ADDROW,
                        NU_TRANSACCION) 
                        VALUES(
                        :Id,                          
                        :Cliente,                     
                        :Empresa,                     
                        :Producto,                    
                        :Faixa,                       
                        :Identificador,               
                        :EspecificaIdentificadorId, 
                        :Agrupacion,                  
                        :Cantidad,                    
                        :CantidadAnulada,             
                        :CantidadLiberada,            
                        :CantidadOriginal,            
                        :FechaAlta,                 
                        :Transaccion)";

            _dapper.Execute(connection, sql, detalles, transaction: tran);
        }

        public virtual void AsociarLpnUpdateDetallesPedido(DbConnection connection, DbTransaction tran, IEnumerable<DetallePedido> detalles)
        {
            string sql = @"
                UPDATE T_DET_PEDIDO_SAIDA 
                SET QT_PEDIDO = QT_PEDIDO + :Cantidad, 
                    NU_TRANSACCION = :Transaccion, 
                    DT_UPDROW = :FechaModificacion
                WHERE NU_PEDIDO = :Id 
                    AND CD_CLIENTE = :Cliente 
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto 
                    AND CD_FAIXA = :Faixa 
                    AND NU_IDENTIFICADOR = :Identificador 
                    AND ID_ESPECIFICA_IDENTIFICADOR = :EspecificaIdentificadorId";

            _dapper.Execute(connection, sql, detalles, transaction: tran);
        }

        public virtual void AsociarLpnInsertDetallesPedidoLpn(DbConnection connection, DbTransaction tran, IEnumerable<DetallePedidoLpn> detallesLpn)
        {
            var sql = @"INSERT INTO T_DET_PEDIDO_SAIDA_LPN 
                        (
                        NU_PEDIDO,
                        CD_CLIENTE,
                        CD_EMPRESA,
                        CD_PRODUTO,
                        CD_FAIXA,
                        NU_IDENTIFICADOR,
                        ID_ESPECIFICA_IDENTIFICADOR,
                        QT_PEDIDO,
                        DT_ADDROW,
                        NU_TRANSACCION,
                        ID_LPN_EXTERNO,
                        TP_LPN_TIPO,
                        QT_LIBERADO,
                        QT_ANULADO,
                        NU_LPN) 
                        VALUES (
                        :Pedido,
                        :Cliente,
                        :Empresa,
                        :Producto,
                        :Faixa,
                        :Identificador,
                        :IdEspecificaIdentificador,
                        :CantidadPedida,
                        :FechaAlta,
                        :Transaccion,
                        :IdLpnExterno,
                        :Tipo,
                        :CantidadLiberada,
                        :CantidadAnulada,
                        :NumeroLpn)";

            _dapper.Execute(connection, sql, detallesLpn, transaction: tran);
        }

        public virtual void AsociarLpnUpdateDetallesPedidoLpn(DbConnection connection, DbTransaction tran, IEnumerable<DetallePedidoLpn> detallesLpn)
        {
            string sql = @"
                UPDATE T_DET_PEDIDO_SAIDA_LPN 
                SET QT_PEDIDO = QT_PEDIDO + :CantidadPedida, 
                    NU_TRANSACCION = :Transaccion, 
                    DT_UPDROW = :FechaModificacion
                WHERE NU_PEDIDO = :Pedido 
                    AND CD_CLIENTE = :Cliente 
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto 
                    AND CD_FAIXA = :Faixa 
                    AND NU_IDENTIFICADOR = :Identificador 
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                    AND ID_LPN_EXTERNO = :IdLpnExterno 
                    AND TP_LPN_TIPO = :Tipo ";

            _dapper.Execute(connection, sql, detallesLpn, transaction: tran);
        }


        public virtual void DesasociarLpn(Pedido pedido, List<Lpn> lpns, long nuTransaccion)
        {
            var detallesPedidoLpn = GetDetallesPedidoLpnUtiizados(pedido, lpns);

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                var bulkContext = GetBulkOperationContext(pedido, detallesPedidoLpn, connection, nuTransaccion);

                using (var tran = connection.BeginTransaction())
                {
                    DesasociarLpnDeleteDefinicionAtributos(connection, tran, bulkContext.UpdateDefinicionAtributos);
                    DesasociarLpnDeleteDetallesPedidoLpnAtributo(connection, tran, bulkContext.UpdateDetallesLpnAtributo);
                    DesasociarLpnDeleteDetallesPedidoLpn(connection, tran, bulkContext.UpdateDetallesLpn.Values);
                    DesasociarLpnUpdateDetallesPedido(connection, tran, bulkContext.UpdateDetalles.Values);

                    tran.Commit();
                }
            }
        }

        public virtual IEnumerable<DetallePedidoLpn> GetDetallesPedidoLpnUtiizados(Pedido pedido, List<Lpn> lpns)
        {
            IEnumerable<DetallePedidoLpn> detallesLpn = new List<DetallePedidoLpn>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_LPN_TEMP (ID_LPN_EXTERNO, TP_LPN_TIPO) VALUES (:IdExterno, :Tipo)";
                    _dapper.Execute(connection, sql, lpns, transaction: tran);

                    var param = new DynamicParameters(new
                    {
                        nuPedido = pedido.Id,
                        empresa = pedido.Empresa,
                        cliente = pedido.Cliente,
                    });

                    sql = @"SELECT 
                                DPSL.ID_LPN_EXTERNO as IdLpnExterno,
                                DPSL.TP_LPN_TIPO as Tipo,
	                            DPSL.NU_PEDIDO as Pedido,
	                            DPSL.CD_CLIENTE as Cliente,
	                            DPSL.CD_EMPRESA as Empresa,
	                            DPSL.CD_PRODUTO as Producto,
	                            DPSL.CD_FAIXA as Faixa,
	                            DPSL.NU_IDENTIFICADOR as Identificador,
	                            DPSL.ID_ESPECIFICA_IDENTIFICADOR as IdEspecificaIdentificador,
	                            SUM(COALESCE(DPSL.QT_PEDIDO,0)) as CantidadPedida
                            FROM T_DET_PEDIDO_SAIDA_LPN  DPSL
                            INNER JOIN T_LPN_TEMP T ON DPSL.ID_LPN_EXTERNO = T.ID_LPN_EXTERNO AND DPSL.TP_LPN_TIPO = T.TP_LPN_TIPO
                            WHERE DPSL.NU_PEDIDO = :nuPedido AND DPSL.CD_EMPRESA = :empresa AND DPSL.CD_CLIENTE = :cliente
                            GROUP BY
	                            DPSL.ID_LPN_EXTERNO,
                                DPSL.TP_LPN_TIPO,
	                            DPSL.NU_PEDIDO,
	                            DPSL.CD_CLIENTE,
	                            DPSL.CD_EMPRESA,
	                            DPSL.CD_PRODUTO,
	                            DPSL.CD_FAIXA,
	                            DPSL.NU_IDENTIFICADOR,
	                            DPSL.ID_ESPECIFICA_IDENTIFICADOR ";

                    detallesLpn = _dapper.Query<DetallePedidoLpn>(connection, sql, param: param, transaction: tran);

                    tran.Rollback();
                }
            }

            return detallesLpn;
        }

        public virtual PedidoGenerarDetalleLpnBulkOperationContext GetBulkOperationContext(Pedido pedido, IEnumerable<DetallePedidoLpn> detallesPedidoLpn, DbConnection connection, long nuTransaccion)
        {
            var context = new PedidoGenerarDetalleLpnBulkOperationContext();

            foreach (var det in detallesPedidoLpn)
            {
                var keyDetallePedido = $"{pedido.Id}.{pedido.Cliente}.{pedido.Empresa}.{det.Producto}.{det.Faixa.ToString("#.###")}.{det.Identificador}.{det.IdEspecificaIdentificador}";

                if (!context.UpdateDetalles.ContainsKey(keyDetallePedido))
                {
                    context.UpdateDetalles[keyDetallePedido] = new DetallePedido()
                    {
                        Id = pedido.Id,
                        Cliente = pedido.Cliente,
                        Empresa = pedido.Empresa,
                        Producto = det.Producto,
                        Faixa = det.Faixa,
                        Identificador = det.Identificador,
                        EspecificaIdentificadorId = det.IdEspecificaIdentificador,
                        Cantidad = det.CantidadPedida,
                        Transaccion = nuTransaccion,
                        TransaccionDelete = nuTransaccion,
                        FechaModificacion = DateTime.Now
                    };

                }
                else
                    context.UpdateDetalles[keyDetallePedido].Cantidad += det.CantidadPedida;

                var keyDetallePedidoLpn = $"{pedido.Id}.{pedido.Cliente}.{pedido.Empresa}.{det.Producto}.{det.Faixa.ToString("#.###")}.{det.Identificador}.{det.IdEspecificaIdentificador}.{det.Tipo}.{det.IdLpnExterno}";

                if (!context.UpdateDetallesLpn.ContainsKey(keyDetallePedidoLpn))
                {
                    context.UpdateDetallesLpn[keyDetallePedidoLpn] = new DetallePedidoLpn()
                    {
                        Pedido = pedido.Id,
                        Cliente = pedido.Cliente,
                        Empresa = pedido.Empresa,
                        Producto = det.Producto,
                        Faixa = det.Faixa,
                        Identificador = det.Identificador,
                        IdEspecificaIdentificador = det.IdEspecificaIdentificador,
                        IdLpnExterno = det.IdLpnExterno,
                        Tipo = det.Tipo,
                        NumeroLpn = det.NumeroLpn,
                        Transaccion = nuTransaccion,
                        TransaccionDelete = nuTransaccion,
                        FechaModificacion = DateTime.Now,
                    };

                    var param = new DynamicParameters(new
                    {
                        Pedido = pedido.Id,
                        Cliente = pedido.Cliente,
                        Empresa = pedido.Empresa,
                        Producto = det.Producto,
                        Faixa = det.Faixa,
                        Identificador = det.Identificador,
                        IdEspecificaIdentificador = det.IdEspecificaIdentificador,
                        Tipo = det.Tipo,
                        IdLpnExterno = det.IdLpnExterno,
                        Transaccion = nuTransaccion,
                        TransaccionDelete = nuTransaccion,
                        FechaModificacion = DateTime.Now,
                    });

                    context.UpdateDetallesLpnAtributo.AddRange(GetDetallesPedidoLpnAtributos(connection, param));
                    context.UpdateDefinicionAtributos.AddRange(GetDefinicionAtributoslpn(connection, param));
                }
            }

            return context;
        }

        public virtual void DesasociarLpnDeleteDefinicionAtributos(DbConnection connection, DbTransaction tran, IEnumerable<DetallePedidoAtributoDefinicion> atributos)
        {
            string sql = @"
                UPDATE T_DET_PEDIDO_SAIDA_ATRIB_DET
                SET NU_TRANSACCION = :Transaccion,
                    NU_TRANSACCION_DELETE = :Transaccion, 
                    DT_UPDROW = :FechaModificacion
                WHERE NU_DET_PED_SAI_ATRIB = :IdConfiguracion 
                    AND ID_ATRIBUTO = :IdAtributo 
                    AND FL_CABEZAL = :IdCabezal ";

            _dapper.Execute(connection, sql, atributos, transaction: tran);

            sql = @"DELETE T_DET_PEDIDO_SAIDA_ATRIB_DET
                    WHERE NU_DET_PED_SAI_ATRIB = :IdConfiguracion 
                    AND ID_ATRIBUTO = :IdAtributo 
                    AND FL_CABEZAL = :IdCabezal ";

            _dapper.Execute(connection, sql, atributos, transaction: tran);
        }

        public virtual void DesasociarLpnDeleteDetallesPedidoLpnAtributo(DbConnection connection, DbTransaction tran, IEnumerable<DetallePedidoLpnAtributo> detallesLpnAtributo)
        {
            string sql = @"
                UPDATE T_DET_PEDIDO_SAIDA_LPN_ATRIB 
                SET NU_TRANSACCION = :Transaccion,
                    NU_TRANSACCION_DELETE = :Transaccion, 
                    DT_UPDROW = :FechaModificacion
                WHERE NU_PEDIDO = :Pedido 
                    AND CD_CLIENTE = :Cliente 
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto 
                    AND CD_FAIXA = :Faixa 
                    AND NU_IDENTIFICADOR = :Identificador 
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                    AND ID_LPN_EXTERNO = :IdLpnExterno
                    AND TP_LPN_TIPO = :Tipo ";

            _dapper.Execute(connection, sql, detallesLpnAtributo, transaction: tran);

            sql = @"DELETE T_DET_PEDIDO_SAIDA_LPN_ATRIB
                    WHERE NU_PEDIDO = :Pedido 
                    AND CD_CLIENTE = :Cliente 
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto 
                    AND CD_FAIXA = :Faixa 
                    AND NU_IDENTIFICADOR = :Identificador 
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador 
                    AND ID_LPN_EXTERNO = :IdLpnExterno
                    AND TP_LPN_TIPO = :Tipo ";

            _dapper.Execute(connection, sql, detallesLpnAtributo, transaction: tran);
        }

        public virtual void DesasociarLpnDeleteDetallesPedidoLpn(DbConnection connection, DbTransaction tran, IEnumerable<DetallePedidoLpn> detallesLpn)
        {
            string sql = @"
                UPDATE T_DET_PEDIDO_SAIDA_LPN 
                SET NU_TRANSACCION = :Transaccion,
                    NU_TRANSACCION_DELETE = :Transaccion, 
                    DT_UPDROW = :FechaModificacion
                WHERE NU_PEDIDO = :Pedido 
                    AND CD_CLIENTE = :Cliente 
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto 
                    AND CD_FAIXA = :Faixa 
                    AND NU_IDENTIFICADOR =:Identificador 
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                    AND ID_LPN_EXTERNO = :IdLpnExterno
                    AND TP_LPN_TIPO = :Tipo ";

            _dapper.Execute(connection, sql, detallesLpn, transaction: tran);

            sql = @"DELETE T_DET_PEDIDO_SAIDA_LPN
                    WHERE NU_PEDIDO = :Pedido 
                    AND CD_CLIENTE = :Cliente 
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto 
                    AND CD_FAIXA = :Faixa 
                    AND NU_IDENTIFICADOR = :Identificador 
                    AND ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador 
                    AND ID_LPN_EXTERNO = :IdLpnExterno
                    AND TP_LPN_TIPO = :Tipo ";

            _dapper.Execute(connection, sql, detallesLpn, transaction: tran);
        }

        public virtual void DesasociarLpnUpdateDetallesPedido(DbConnection connection, DbTransaction tran, IEnumerable<DetallePedido> detalles)
        {
            string sql = @"
                UPDATE T_DET_PEDIDO_SAIDA 
                SET QT_PEDIDO = QT_PEDIDO - :Cantidad, 
                    NU_TRANSACCION = :Transaccion, 
                    DT_UPDROW = :FechaModificacion
                WHERE NU_PEDIDO = :Id 
                    AND CD_CLIENTE = :Cliente 
                    AND CD_EMPRESA = :Empresa
                    AND CD_PRODUTO = :Producto 
                    AND CD_FAIXA = :Faixa 
                    AND NU_IDENTIFICADOR = :Identificador 
                    AND ID_ESPECIFICA_IDENTIFICADOR = :EspecificaIdentificadorId";

            _dapper.Execute(connection, sql, detalles, transaction: tran);
        }

        public virtual IEnumerable<DetallePedidoLpnAtributo> GetDetallesPedidoLpnAtributos(DbConnection connection, DynamicParameters parameters)
        {
            string sql = @"SELECT 
	                            DPSLA.NU_PEDIDO as Pedido,
	                            DPSLA.CD_CLIENTE as Cliente,
	                            DPSLA.CD_EMPRESA as Empresa,
	                            DPSLA.CD_PRODUTO as Producto,
	                            DPSLA.CD_FAIXA as Faixa,
	                            DPSLA.NU_IDENTIFICADOR as Identificador,
	                            DPSLA.ID_ESPECIFICA_IDENTIFICADOR as IdEspecificaIdentificador,
                                DPSLA.TP_LPN_TIPO as Tipo,
                                DPSLA.ID_LPN_EXTERNO as IdLpnExterno,
	                            DPSLA.NU_DET_PED_SAI_ATRIB as IdConfiguracion,
                                :Transaccion as Transaccion,
                                :TransaccionDelete as TransaccionDelete,
                                :FechaModificacion as FechaModificacion
                            FROM T_DET_PEDIDO_SAIDA_LPN_ATRIB  DPSLA
                            WHERE DPSLA.NU_PEDIDO = :Pedido 
                            AND DPSLA.CD_CLIENTE = :Cliente
                            AND DPSLA.CD_EMPRESA = :Empresa 
                            AND DPSLA.CD_PRODUTO = :Producto
                            AND DPSLA.CD_FAIXA = :Faixa
                            AND DPSLA.NU_IDENTIFICADOR = :Identificador
                            AND DPSLA.ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                            AND DPSLA.TP_LPN_TIPO = :Tipo
                            AND DPSLA.ID_LPN_EXTERNO = :IdLpnExterno";

            return _dapper.Query<DetallePedidoLpnAtributo>(connection, sql, param: parameters).ToList();
        }

        public virtual IEnumerable<DetallePedidoAtributoDefinicion> GetDefinicionAtributoslpn(DbConnection connection, DynamicParameters parameters)
        {
            string sql = @" SELECT 
	                            DPSAD.NU_DET_PED_SAI_ATRIB as IdConfiguracion,
	                            DPSAD.ID_ATRIBUTO as IdAtributo,
	                            DPSAD.FL_CABEZAL as IdCabezal,
                                :Transaccion as Transaccion,
                                :TransaccionDelete as TransaccionDelete,
                                :FechaModificacion as FechaModificacion
                            FROM T_DET_PEDIDO_SAIDA_LPN_ATRIB  DPSLA
                            INNER JOIN T_DET_PEDIDO_SAIDA_ATRIB_DET DPSAD 
                                ON DPSLA.NU_DET_PED_SAI_ATRIB = DPSAD.NU_DET_PED_SAI_ATRIB 
                            WHERE DPSLA.NU_PEDIDO = :Pedido 
                            AND DPSLA.CD_CLIENTE = :Cliente
                            AND DPSLA.CD_EMPRESA = :Empresa 
                            AND DPSLA.CD_PRODUTO = :Producto
                            AND DPSLA.CD_FAIXA = :Faixa
                            AND DPSLA.NU_IDENTIFICADOR = :Identificador
                            AND DPSLA.ID_ESPECIFICA_IDENTIFICADOR = :IdEspecificaIdentificador
                            AND DPSLA.TP_LPN_TIPO = :Tipo
                            AND DPSLA.ID_LPN_EXTERNO = :IdLpnExterno";

            return _dapper.Query<DetallePedidoAtributoDefinicion>(connection, sql, param: parameters).ToList();
        }

        #endregion

        #region PRE052

        public virtual void InsertPedidos(DbConnection connection, DbTransaction tran, Pedido pedido)
        {
            var sql = @"INSERT INTO T_PEDIDO_SAIDA 
                        (NU_PEDIDO, 
                        CD_EMPRESA,
                        CD_CLIENTE,
                        CD_CONDICION_LIBERACION,
                        CD_ORIGEN,
                        CD_PUNTO_ENTREGA,
                        CD_ROTA,
                        CD_SITUACAO,
                        CD_TRANSPORTADORA,
                        CD_ZONA,
                        DS_ANEXO1,
                        DS_ANEXO2,
                        DS_ANEXO3,
                        DS_ANEXO4,
                        DS_ENDERECO,
                        DS_MEMO,
                        DS_MEMO_1,
                        DT_ADDROW,
                        DT_EMITIDO,
                        DT_ENTREGA,
                        DT_GENERICO_1,
                        DT_LIBERAR_DESDE,
                        DT_LIBERAR_HASTA,
                        FL_SYNC_REALIZADA,
                        ID_AGRUPACION,
                        ID_MANUAL,
                        NU_GENERICO_1,
                        NU_ORDEN_ENTREGA,
                        NU_PREDIO,
                        TP_EXPEDICION,
                        TP_PEDIDO,
                        VL_COMPARTE_CONTENEDOR_ENTREGA,
                        VL_COMPARTE_CONTENEDOR_PICKING,
                        VL_GENERICO_1,
                        NU_TELEFONE,
                        NU_TELEFONE2,
                        VL_LONGITUD,
                        VL_LATITUD, 
                        NU_TRANSACCION,
                        VL_SERIALIZADO_1,
                        ND_ACTIVIDAD) 
                        VALUES (
                        :Id,                          
                        :Empresa,                     
                        :Cliente,                     
                        :CondicionLiberacion,         
                        :Origen,                      
                        :PuntoEntrega,                
                        :Ruta,                        
                        :Estado,               
                        :CodigoTransportadora,        
                        :Zona,                        
                        :Anexo,                       
                        :Anexo2,                      
                        :Anexo3,                      
                        :Anexo4,                      
                        :DireccionEntrega,            
                        :Memo,                        
                        :Memo1,                       
                        :FechaAlta,                           
                        :FechaEmision,                
                        :FechaEntrega,                
                        :FechaGenerica_1,             
                        :FechaLiberarDesde,           
                        :FechaLiberarHasta,           
                        :SincronizacionRealizadaId, 
                        :Agrupacion,                  
                        :ManualId,                
                        :NuGenerico_1,                
                        :OrdenEntrega,                
                        :Predio,                      
                        :TipoExpedicionId,          
                        :Tipo,                        
                        :ComparteContenedorEntrega,   
                        :ComparteContenedorPicking,   
                        :VlGenerico_1,                
                        :Telefono,
                        :TelefonoSecundario,
                        :Longitud,
                        :Latitud,
                        :Transaccion,
                        :VlSerealizado_1,
                        :Actividad)";
            _dapper.Execute(connection, sql, pedido, transaction: tran);
        }
        public virtual void BulkInsertLineasTemp(DbConnection connection, DbTransaction tran, List<DetallePedido> detallesPedido)
        {
            _dapper.BulkInsert(connection, tran, detallesPedido, "T_DET_PEDIDO_SAIDA_TEMP", new Dictionary<string, Func<DetallePedido, ColumnInfo>>
            {
                { "NU_PEDIDO", x => new ColumnInfo(x.Id)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Identificador)},
                { "QT_PEDIDO", x => new ColumnInfo(x.Cantidad)},
                { "ID_ESPECIFICA_IDENTIFICADOR", x => new ColumnInfo(x.EspecificaIdentificadorId)},
            });
        }

        public virtual void InsertDetallePedidoInexistente(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            string sql = @"INSERT INTO T_DET_PEDIDO_SAIDA
                        (NU_PEDIDO,
                        CD_EMPRESA,
                        CD_CLIENTE,
                        CD_FAIXA,
                        CD_PRODUTO,
                        DT_ADDROW,
                        ID_AGRUPACION,
                        ID_ESPECIFICA_IDENTIFICADOR,
                        NU_IDENTIFICADOR,
                        QT_ANULADO,
                        QT_LIBERADO,
                        QT_PEDIDO,
                        QT_PEDIDO_ORIGINAL,
                        NU_TRANSACCION) 
                        (SELECT  
                            sdt.NU_PEDIDO,                          
                            sdt.CD_EMPRESA,                     
                            sdt.CD_CLIENTE,                     
                            sdt.CD_FAIXA,                       
                            sdt.CD_PRODUTO,                                           
                            :FechaAlta,                             
                            :Agrupacion,                  
                            sdt.ID_ESPECIFICA_IDENTIFICADOR,         
                            sdt.NU_IDENTIFICADOR,               
                            :CantidadAnulada,             
                            :CantidadLiberada,            
                            sdt.QT_PEDIDO,                    
                            sdt.QT_PEDIDO,            
                            :Transaccion
                        FROM T_DET_PEDIDO_SAIDA_TEMP sdt
                        LEFT JOIN T_DET_PEDIDO_SAIDA sd on   sd.NU_PEDIDO = sdt.NU_PEDIDO AND                          
                          sd.CD_EMPRESA = sdt.CD_EMPRESA AND                   
                          sd.CD_CLIENTE = sdt.CD_CLIENTE AND                     
                          sd.CD_FAIXA = sdt.CD_FAIXA AND                       
                          sd.CD_PRODUTO = sdt.CD_PRODUTO AND  
                          sd.ID_ESPECIFICA_IDENTIFICADOR = sdt.ID_ESPECIFICA_IDENTIFICADOR
                        WHERE sd.NU_PEDIDO is null
                        )";

            _dapper.Execute(connection, sql, param: new { FechaAlta = DateTime.Now, Agrupacion = Agrupacion.Pedido, CantidadAnulada = 0, CantidadLiberada = 0, Transaccion = nuTransaccion }, transaction: tran);
        }

        public virtual void BulkUpdateLineas(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var alias = "dp";
            var from = $@"
                T_DET_PEDIDO_SAIDA dp
                INNER JOIN (
                    SELECT 
                         dps.NU_PEDIDO,
                         dps.CD_EMPRESA,
                         dps.CD_CLIENTE,
                         dps.CD_FAIXA,
                         dps.CD_PRODUTO,
                         dps.ID_ESPECIFICA_IDENTIFICADOR,
                         SUM(dpt.QT_PEDIDO) QT_PEDIDO_DET
                    FROM  T_DET_PEDIDO_SAIDA_TEMP  dpt
                    INNER JOIN T_DET_PEDIDO_SAIDA dps on 
                          dps.NU_PEDIDO =dpt.NU_PEDIDO AND                          
                          dps.CD_EMPRESA = dpt.CD_EMPRESA AND                   
                          dps.CD_CLIENTE = dpt.CD_CLIENTE AND                     
                          dps.CD_FAIXA = dpt.CD_FAIXA AND                       
                          dps.CD_PRODUTO = dpt.CD_PRODUTO AND  
                          dps.ID_ESPECIFICA_IDENTIFICADOR = dpt.ID_ESPECIFICA_IDENTIFICADOR
                    GROUP by dps.NU_PEDIDO,
                         dps.CD_EMPRESA,
                         dps.CD_CLIENTE,
                         dps.CD_FAIXA,
                         dps.CD_PRODUTO,
                         dps.ID_ESPECIFICA_IDENTIFICADOR
                ) dpt ON   dp.NU_PEDIDO =dpt.NU_PEDIDO AND                          
                          dp.CD_EMPRESA = dpt.CD_EMPRESA AND                   
                          dp.CD_CLIENTE = dpt.CD_CLIENTE AND                     
                          dp.CD_FAIXA = dpt.CD_FAIXA AND                       
                          dp.CD_PRODUTO = dpt.CD_PRODUTO AND  
                          dp.ID_ESPECIFICA_IDENTIFICADOR = dpt.ID_ESPECIFICA_IDENTIFICADOR";
            var set = @"
                DT_UPDROW = :FechaModificacion,
                NU_TRANSACCION = :Transaccion,
                QT_PEDIDO = QT_PEDIDO + QT_PEDIDO_DET";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new { FechaModificacion = DateTime.Now, Transaccion = nuTransaccion }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }
        public virtual void BulkUpdateLineasCantidadLiberada(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var alias = "dp";
            var from = $@"
                T_DET_PEDIDO_SAIDA dp
                INNER JOIN (
                    SELECT 
                         dps.NU_PEDIDO,
                         dps.CD_EMPRESA,
                         dps.CD_CLIENTE,
                         dps.CD_FAIXA,
                         dps.CD_PRODUTO,
                         dps.ID_ESPECIFICA_IDENTIFICADOR,
                         dps.NU_IDENTIFICADOR,
                         SUM(dpt.QT_PEDIDO) QT_PEDIDO_DET
                    FROM  T_DET_PEDIDO_SAIDA_TEMP  dpt
                    INNER JOIN T_DET_PEDIDO_SAIDA dps on 
                          dps.NU_PEDIDO =dpt.NU_PEDIDO AND                          
                          dps.CD_EMPRESA = dpt.CD_EMPRESA AND                   
                          dps.CD_CLIENTE = dpt.CD_CLIENTE AND                     
                          dps.CD_FAIXA = dpt.CD_FAIXA AND                       
                          dps.CD_PRODUTO = dpt.CD_PRODUTO AND  
                          dps.NU_IDENTIFICADOR = dpt.NU_IDENTIFICADOR AND  
                          dps.ID_ESPECIFICA_IDENTIFICADOR = dpt.ID_ESPECIFICA_IDENTIFICADOR
                    GROUP by dps.NU_PEDIDO,
                         dps.CD_EMPRESA,
                         dps.CD_CLIENTE,
                         dps.CD_FAIXA,
                         dps.CD_PRODUTO,
                         dps.ID_ESPECIFICA_IDENTIFICADOR,
                         dps.NU_IDENTIFICADOR
                ) dpt ON   dp.NU_PEDIDO =dpt.NU_PEDIDO AND                          
                          dp.CD_EMPRESA = dpt.CD_EMPRESA AND                   
                          dp.CD_CLIENTE = dpt.CD_CLIENTE AND                     
                          dp.CD_FAIXA = dpt.CD_FAIXA AND                       
                          dp.CD_PRODUTO = dpt.CD_PRODUTO AND  
                          dp.NU_IDENTIFICADOR = dpt.NU_IDENTIFICADOR AND 
                          dp.ID_ESPECIFICA_IDENTIFICADOR = dpt.ID_ESPECIFICA_IDENTIFICADOR";
            var set = @"
                DT_UPDROW = :FechaModificacion,
                NU_TRANSACCION = :Transaccion,
                QT_LIBERADO = QT_LIBERADO + QT_PEDIDO_DET";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new { FechaModificacion = DateTime.Now, Transaccion = nuTransaccion }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }
        public virtual void BulkInsertDetallesLpn(DbConnection connection, DbTransaction tran, List<DetallePedidoLpn> detallesLpn)
        {
            string sql = GetSqlInsertDetallesLpn();

            _dapper.ExecuteAsync(connection, sql, detallesLpn, transaction: tran);
        }

        public virtual void BulkUpdatePedidoUltimaPreparacion(DbConnection connection, DbTransaction tran, List<Pedido> pedido, int preparacion, long nuTransaccion)
        {
            _dapper.BulkUpdate(connection, tran, pedido, "T_PEDIDO_SAIDA", new Dictionary<string, Func<Pedido, ColumnInfo>>
            {
                { "NU_ULT_PREPARACION", x => new ColumnInfo(preparacion)},
                { "NU_TRANSACCION", x => new ColumnInfo(nuTransaccion)},
            }, new Dictionary<string, Func<Pedido, ColumnInfo>>
            {
                { "NU_PEDIDO", x => new ColumnInfo(x.Id)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
            });
        }

        public virtual void BulkUpdateLineasCantidadLiberadaLpn(DbConnection connection, DbTransaction tran, List<DetallePreparacionLpn> detallesLpnsPreparacion)
        {
            _dapper.BulkUpdate(connection, tran, detallesLpnsPreparacion, "T_DET_PEDIDO_SAIDA_LPN", new Dictionary<string, Func<DetallePreparacionLpn, ColumnInfo>>
            {
                { "QT_LIBERADO", x => new ColumnInfo(x.CantidadReservada,OperacionDb.OperacionMas)},
                { "NU_TRANSACCION", x => new ColumnInfo(x.Transaccion)},
            }, new Dictionary<string, Func<DetallePreparacionLpn, ColumnInfo>>
            {
                { "NU_PEDIDO", x => new ColumnInfo(x.Pedido)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "TP_LPN_TIPO", x => new ColumnInfo(x.TipoLpn)},
                { "ID_LPN_EXTERNO", x => new ColumnInfo(x.IdExternoLpn)},
                { "NU_LPN", x => new ColumnInfo(x.NroLpn)},
                { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Lote)},
            });
        }

        #endregion

        #endregion
    }
}
