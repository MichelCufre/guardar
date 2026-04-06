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
using WIS.Domain.Expedicion;
using WIS.Domain.Expedicion.Enums;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.General.API;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Interfaces;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Persistence.Database;
using WIS.Persistence.General;

namespace WIS.Domain.DataModel.Repositories
{
    public class CamionRepository
    {
        protected readonly WISDB _context;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly string _application;
        protected readonly CamionMapper _mapper;
        protected readonly CargaCamionMapper _cargaMapper;
        protected readonly TransaccionRepository _transaccionRepository;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly CargaRepository _cargaRepository;
        protected readonly ParametroRepository _parametroRepository;

        public CamionRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            _dapper = dapper;
            _userId = userId;
            _context = context;
            _application = application;
            _mapper = new CamionMapper();
            _cargaMapper = new CargaCamionMapper();
            _cargaRepository = new CargaRepository(_context, application, userId, _dapper);
            _transaccionRepository = new TransaccionRepository(_context, application, _userId, _dapper);
            _parametroRepository = new ParametroRepository(_context, application, userId, dapper);

        }

        #region Any
        public virtual bool AnyCamionConRutaAsociada(short ruta)
        {
            List<short> situaciones = new List<short>
            {
                SituacionDb.CamionAguardandoCarga,
                SituacionDb.CamionCargando,
                SituacionDb.CamionSinOrdenDeTrabajo,
            };

            return this._context.T_CAMION.Any(d => d.CD_ROTA == ruta && situaciones.Contains(d.CD_SITUACAO));
        }

        public virtual bool AnyCamionConRutaAsociada(short ruta, int camionExcluir)
        {
            List<short> situaciones = new List<short>
            {
                SituacionDb.CamionAguardandoCarga,
                SituacionDb.CamionCargando,
                SituacionDb.CamionSinOrdenDeTrabajo,
            };

            return this._context.T_CAMION.Any(d => d.CD_ROTA == ruta && situaciones.Contains(d.CD_SITUACAO) && d.CD_CAMION != camionExcluir);
        }

        public virtual bool AnyCamionConVehiculo(List<int> vehiculos)
        {
            return this._context.T_CAMION.Any(d => vehiculos.Contains(d.CD_VEICULO ?? -1));
        }

        public virtual bool AnyCamion(int camion)
        {
            return this._context.T_CAMION.Any(x => x.CD_CAMION == camion);
        }

        public virtual bool ExistenCargasNoAsociadas(int nuPreparacion, int nuContenedor, int cdCamion)
        {
            var cargasContenedor = new ContenedorRepository(this._context, this._application, this._userId, this._dapper)
                .GetCargasContenedor(nuPreparacion, nuContenedor);

            if (cargasContenedor != null)
            {
                foreach (var carga in cargasContenedor)
                {
                    if (!_context.T_CLIENTE_CAMION.Any(w => w.NU_CARGA == carga && w.CD_CAMION == cdCamion))
                        return true;
                }
            }

            return false;
        }

        public virtual bool AnyClienteAsociadoCamion(int cdCamion)
        {
            return this._context.T_CLIENTE_CAMION.Any(x => x.CD_CAMION == cdCamion);
        }

        public virtual bool CamionVacio(int cdCamion)
        {
            if (this._context.T_CLIENTE_CAMION.Any(x => x.CD_CAMION == cdCamion))
                return false;
            else if (this._context.T_CONTENEDOR.Any(x => x.CD_CAMION == cdCamion))
                return false;

            return true;
        }

        public virtual bool ExistenContenedoresCompartidos(int nroPreparacion, int nroContenedor)
        {
            var situacionesCamion = new List<short>
            {
                SituacionDb.CamionAguardandoCarga,
                SituacionDb.CamionCargando
            };

            var query3 = _context.V_QT_CAMIONES_CONTENEDOR
                .Where(qcc => qcc.NU_PREPARACION == nroPreparacion
                    && qcc.NU_CONTENEDOR == nroContenedor
                    && (qcc.CD_SITUACAO == null || situacionesCamion.Contains(qcc.CD_SITUACAO ?? SituacionDb.CamionAguardandoCarga)))
                .GroupBy(qcc => qcc.CD_CAMION);

            var qtCamiones = query3.Count();

            return qtCamiones > 1;
        }

        public virtual bool AnyCargaOtraEmpresa(int camion, int empresa)
        {
            return this._context.T_CLIENTE_CAMION
                .AsNoTracking()
                .Any(cc => cc.CD_EMPRESA != empresa && cc.CD_CAMION == camion && cc.ID_CARGAR == "S");
        }

        public virtual bool AnyCargaAsociada(int camion)
        {
            return this._context.V_EGRESO_CAMION_WEXP
                .AsNoTracking()
                .Any(s => s.CD_CAMION == camion);
        }

        #endregion

        #region Get

        public virtual List<int> GetCamionesPendienteCierre()
        {
            var camiones = (from c in _context.T_CAMION
                            join e in _context.V_EMPRESA_DOCUMENTAL on c.CD_EMPRESA equals e.CD_EMPRESA
                            where c.CD_SITUACAO == SituacionDb.CamionIniciandoCierre &&
                                ((e.FL_DOCUMENTAL == "S" && c.FL_HABILITADO_CIERRE == "S") || e.FL_DOCUMENTAL == "N")
                            select c.CD_CAMION)
                            .ToList();
            return camiones;
        }

        public virtual int GetNextCdCamion()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_CD_CAMION);
        }

        public virtual Camion GetCamion(int cdCamion)
        {
            T_CAMION entity = _context.T_CAMION
                .AsNoTracking()
                .FirstOrDefault(f => f.CD_CAMION == cdCamion);

            return _mapper.MapToObject(entity);
        }

        public virtual Camion GetCamionWithCargas(int camionId)
        {
            var entities = _context.T_CAMION
                .GroupJoin(
                    _context.T_CLIENTE_CAMION,
                    c => c.CD_CAMION,
                    cc => cc.CD_CAMION,
                    (c, cc) => new { Camion = c, CargaCamion = cc })
                .AsNoTracking()
                .SelectMany(d =>
                    d.CargaCamion.DefaultIfEmpty(),
                    (d, e) => new { d.Camion, CargaCamion = e })
                .Where(f => f.Camion.CD_CAMION == camionId)
                .ToList();

            Camion camion = _mapper.MapToObject(entities.FirstOrDefault().Camion);

            if (camion != null)
            {
                foreach (var entity in entities)
                {
                    if (entity.CargaCamion != null)
                        camion.Cargas.Add(this._cargaMapper.MapToObject(entity.CargaCamion));
                }
            }

            return camion;
        }

        public virtual List<Camion> GetCamionesSinPorteriaByMatricula(string matricula)
        {
            return (from p in _context.T_CAMION
                    join co in _context.T_PORTERIA_VEHICULO_CAMION on p.CD_CAMION equals co.CD_CAMION into CamionPorteria
                    from pco in CamionPorteria.DefaultIfEmpty()
                    where p.CD_PLACA_CARRO == matricula && pco == null
                    select p).AsNoTracking()
             .ToList()
             .Select(w => _mapper.MapToObject(w))
             .ToList();
        }

        public virtual CamionDescripcion GetCamionDescripcion(int cdCamion)
        {
            return _mapper.MapToObject(
                _context.V_CAMION_EXP050
                .AsNoTracking()
                .FirstOrDefault(f => f.CD_CAMION == cdCamion));
        }

        public virtual List<int> GetEmpresasCamion(int cdCamion)
        {
            var q = _context.T_DET_PEDIDO_EXPEDIDO.Where(d => d.CD_CAMION == cdCamion).GroupBy(e => e.CD_EMPRESA).Select(e => e.Key);
            return q.ToList();
        }

        public virtual List<string> GetClientesCamion(int cdCamion)
        {
            var q = _context.T_DET_PEDIDO_EXPEDIDO.Where(d => d.CD_CAMION == cdCamion).GroupBy(e => e.CD_CLIENTE).Select(e => e.Key);
            return q.ToList();
        }

        public virtual List<int[]> GetContenedoresConProblemas(int cdCamion)
        {
            var contenedoresConProblemas = new List<int[]>();

            var listaContenedores = _context.T_CLIENTE_CAMION
                                .Where(cc => cc.CD_CAMION == cdCamion)
                                .Join(
                                    _context.T_DET_PICKING.Include("T_CONTENEDOR"),
                                    cc => new { cc.CD_CLIENTE, cc.NU_CARGA, cc.CD_EMPRESA },
                                    dp => new { dp.CD_CLIENTE, NU_CARGA = dp.NU_CARGA ?? -1, dp.CD_EMPRESA },
                                    (cc, dp) => new { Picking = dp, CliCarga = cc }
                                )
                                .Where(dp => dp.Picking.NU_CONTENEDOR != null && dp.Picking.T_CONTENEDOR.CD_SITUACAO == SituacionDb.ContenedorEnPreparacion)
                                .Select(dp => new { NU_CONTENEDOR = dp.Picking.T_CONTENEDOR.NU_CONTENEDOR, NU_PREPARACION = dp.Picking.NU_PREPARACION, dp.Picking.ID_AGRUPACION })
                                .Distinct()
                                .ToList();

            foreach (var cont in listaContenedores)
            {
                if (ExistenCargasNoAsociadas(cont.NU_PREPARACION, cont.NU_CONTENEDOR, cdCamion))
                    contenedoresConProblemas.Add(new int[] { cont.NU_PREPARACION, cont.NU_CONTENEDOR });

                if (cont.ID_AGRUPACION != null && (cont.ID_AGRUPACION == "P" || cont.ID_AGRUPACION == "C"))
                {
                    if (ExistenContenedoresCompartidos(cont.NU_PREPARACION, cont.NU_CONTENEDOR))
                        contenedoresConProblemas.Add(new int[] { cont.NU_PREPARACION, cont.NU_CONTENEDOR });
                }
            }

            return contenedoresConProblemas;
        }

        public virtual HashSet<int> GetEgresosAMarcar()
        {
            return _context.V_EGRESOS_A_MARCAR.AsNoTracking().Select(e => e.CD_CAMION).ToHashSet();
        }

        public virtual HashSet<string> GetIdentificadoresExternos()
        {
            return _context.T_CAMION.AsNoTracking().Where(c => c.ID_EXTERNO != null).Select(c => $"{c.ID_EXTERNO}.{c.CD_EMPRESA_EXTERNA}").ToHashSet();
        }

        public virtual Camion GetCamionAsignado(long NuCarga)
        {
            T_CAMION entity = _context.T_CLIENTE_CAMION
                .Include("T_CAMION")
                .AsNoTracking()
                .FirstOrDefault(w => w.NU_CARGA == NuCarga)?.T_CAMION;

            return _mapper.MapToObject(entity);
        }

        #endregion

        #region Add

        public virtual void AddCamion(Camion obj)
        {
            if (obj.Id == 0)
                obj.Id = this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_CD_CAMION);

            T_CAMION entity = this._mapper.MapToEntity(obj);

            this._context.T_CAMION.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateCamion(Camion camion)
        {
            T_CAMION entity = this._mapper.MapToEntity(camion);
            T_CAMION attachedEntity = _context.T_CAMION.Local.FirstOrDefault(w => w.CD_CAMION == entity.CD_CAMION);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_CAMION.Attach(entity);
                _context.Entry<T_CAMION>(entity).State = EntityState.Modified;
            }
        }

        public virtual void DesAsociarPedido(int cdCamion, string nuPedido, string cdCliente, int cdEmpresa)
        {
            var cargas = _context.T_DET_PICKING
                .Where(s => s.CD_CLIENTE == cdCliente && s.CD_EMPRESA == cdEmpresa && s.NU_PEDIDO == nuPedido)
                .Select(a => a.NU_CARGA).Distinct().ToList();

            foreach (var nuCarga in cargas)
            {
                var carga = _context.T_CLIENTE_CAMION.FirstOrDefault(s => s.CD_CAMION == cdCamion && s.CD_CLIENTE == cdCliente && s.CD_EMPRESA == cdEmpresa && s.NU_CARGA == nuCarga);
                if (carga != null)
                    _context.T_CLIENTE_CAMION.Remove(carga);
            }
        }

        #endregion

        #region Remove

        public virtual void RemoveCamion(Camion obj)
        {
            T_CAMION entity = this._mapper.MapToEntity(obj);
            T_CAMION attachedEntity = _context.T_CAMION.Local.FirstOrDefault(x => x.CD_CAMION == entity.CD_CAMION);

            if (attachedEntity != null)
            {
                _context.T_CAMION.Remove(attachedEntity);
            }
            else
            {
                _context.T_CAMION.Attach(entity);
                _context.T_CAMION.Remove(entity);
            }
        }

        public virtual void RemoveCargasCamion(int cdCamion, List<CargaCamion> cargas)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            RemoveCargasCamion(connection, tran, cdCamion, cargas);
        }

        #endregion

        #region Dapper

        public virtual async Task<Camion> GetCamionOrNull(int cdcamion, bool mapearCargas = false, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var model = GetCamion(new Camion
                {
                    Id = cdcamion
                }, connection);

                Fill(connection, model, mapearCargas);
                return model;
            }
        }

        public virtual async Task<Camion> GetCamionByIdExterno(string idExterno, int empExterna, bool mapearCargas = false, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var model = GetCamion(new Camion
                {
                    Id = -1,
                    IdExterno = idExterno,
                    EmpresaExterna = empExterna
                }, connection);

                Fill(connection, model, mapearCargas);
                return model;
            }
        }

        public virtual Camion GetCamion(Camion model, DbConnection connection)
        {
            string where = @" WHERE c.CD_CAMION = :cdcamion ";

            if (model.Id == -1)
                where = @" WHERE c.ID_EXTERNO = :idExterno AND c.CD_EMPRESA_EXTERNA = :empExterna ";


            string sql = GetSqlSelectCamion() + where;
            var query = _dapper.Query<Camion>(connection, sql, param: new
            {
                cdcamion = model.Id,
                idExterno = model.IdExterno,
                empExterna = model.EmpresaExterna
            }, commandType: CommandType.Text).FirstOrDefault();

            return query;
        }

        public static string GetSqlSelectCamion()
        {
            return @"SELECT 
                        c.CD_CAMION as Id,
                        c.CD_EMPRESA as Empresa,
                        c.CD_FUNC_CIERRE as FuncionarioCierre,
                        c.CD_PLACA_CARRO as Matricula,
                        c.CD_PORTA as Puerta,
                        c.CD_ROTA as Ruta,
                        c.CD_SITUACAO as EstadoId,
                        c.CD_TRANSPORTADORA as Transportista,
                        c.CD_VEICULO as Vehiculo,
                        c.DS_CAMION as Descripcion,
                        c.DS_DOCUMENTO as Documento,
                        c.DT_ADDROW as FechaCreacion,
                        c.DT_ARRIBO_CAMION as FechaArriboCamion,
                        c.DT_CIERRE as FechaCierre,
                        c.DT_FACTURACION as FechaFacturacion,
                        c.DT_PROGRAMADO as FechaProgramado,
                        c.DT_UPDROW as FechaModificacion,
                        c.FL_AUTO_CARGA as CargaAutomaticaHabilitadaId,
                        c.FL_CIERRE_AUTO as CierreAutomaticoHabilitadoId,
                        c.FL_CIERRE_PARCIAL as CierreParcialHabilitadoId,
                        c.FL_CONF_VIAJE_REALIZADA as ConfirmacionViajeRealizadaId,
                        c.FL_CTRL_CONTENEDORES as ControlContenedoresHabilitadoId,
                        c.FL_HABILITADO_ARMADO as ArmadoHabilitadoId,
                        c.FL_HABILITADO_CARGA as CargaHabilitadaId,
                        c.FL_HABILITADO_CIERRE as CierreHabilitadoId,
                        c.FL_RUTEO as RuteoHabilitadoId,
                        c.FL_SYNC_REALIZADA as SincronizacionRealizadaId,
                        c.FL_TRACKING as TrackingHabilitadoId,
                        c.ID_RESPETA_ORD_CARGA as RespetaOrdenCargaId,
                        c.ND_ARMADO_EGRESO as ArmadoEgreso,
                        c.NU_INTERFAZ_EJECUCION as NumeroInterfazEjecucionCierre,
                        c.NU_INTERFAZ_EJECUCION_FACT as NumeroInterfazEjecucionFactura,
                        c.NU_ORT_ORDEN as NumeroOrtOrden,
                        c.NU_PREDIO as Predio,
                        c.NU_TRANSACCION as NumeroTransaccion,
                        c.TP_ARMADO_EGRESO as TipoArmadoEgreso,
                        c.ID_EXTERNO as IdExterno,
                        c.CD_EMPRESA_EXTERNA as EmpresaExterna
                FROM T_CAMION c ";
        }

        public virtual Camion MapInternal(Camion camion)
        {
            if (camion != null)
            {
                switch (camion.EstadoId)
                {
                    case SituacionDb.CamionAguardandoCarga: camion.Estado = CamionEstado.AguardandoCarga; break;
                    case SituacionDb.CamionCargando: camion.Estado = CamionEstado.Cargando; break;
                    case SituacionDb.CamionCerrado: camion.Estado = CamionEstado.Cerrado; break;
                    case SituacionDb.CamionIniciandoCierre: camion.Estado = CamionEstado.IniciandoCierre; break;
                    case SituacionDb.CamionSinOrdenDeTrabajo: camion.Estado = CamionEstado.SinOrdenDeTrabajo; break;
                    default: camion.Estado = CamionEstado.Unknown; break;
                }

                camion.RespetaOrdenCarga = !string.IsNullOrEmpty(camion.RespetaOrdenCargaId) && camion.RespetaOrdenCargaId == "S";
                camion.IsTrackingHabilitado = !string.IsNullOrEmpty(camion.TrackingHabilitadoId) && camion.TrackingHabilitadoId == "S";
                camion.IsRuteoHabilitado = !string.IsNullOrEmpty(camion.RuteoHabilitadoId) && camion.RuteoHabilitadoId == "S";
                camion.IsSincronizacionRealizada = !string.IsNullOrEmpty(camion.SincronizacionRealizadaId) && camion.SincronizacionRealizadaId == "S";
                camion.ConfirmacionViajeRealizada = !string.IsNullOrEmpty(camion.ConfirmacionViajeRealizadaId) && camion.ConfirmacionViajeRealizadaId == "S";
                camion.IsCargaHabilitada = !string.IsNullOrEmpty(camion.CargaHabilitadaId) && camion.CargaHabilitadaId == "S";
                camion.IsCierreHabilitado = !string.IsNullOrEmpty(camion.CierreHabilitadoId) && camion.CierreHabilitadoId == "S";
                camion.ArmadoHabilitado = !string.IsNullOrEmpty(camion.ArmadoHabilitadoId) && camion.ArmadoHabilitadoId == "S";
                camion.IsCierreParcialHabilitado = !string.IsNullOrEmpty(camion.CierreParcialHabilitadoId) && camion.CierreParcialHabilitadoId == "S";
                camion.IsCierreAutomaticoHabilitado = !string.IsNullOrEmpty(camion.CierreAutomaticoHabilitadoId) && camion.CierreAutomaticoHabilitadoId == "S";
                camion.IsCargaAutomaticaHabilitada = !string.IsNullOrEmpty(camion.CargaAutomaticaHabilitadaId) && camion.CargaAutomaticaHabilitadaId == "S";
                camion.IsControlContenedoresHabilitado = !string.IsNullOrEmpty(camion.ControlContenedoresHabilitadoId) && camion.ControlContenedoresHabilitadoId == "S";
            }
            return camion;
        }

        public virtual void Fill(DbConnection connection, Camion model, bool mapearCargas)
        {
            if (model != null)
            {
                if (mapearCargas)
                    model.Cargas = GetCargas(connection, model);

                model = MapInternal(model);
            }
        }

        public virtual List<CargaCamion> GetCargas(DbConnection connection, Camion model, bool excluirSobrantes = false)
        {
            string sql = @"SELECT 
                            CD_CAMION as Camion,
                            CD_CLIENTE as Cliente,
                            CD_EMPRESA as Empresa,
                            DT_ADDROW as FechaAlta,
                            DT_UPDROW as FechaModificacion,
                            ID_CARGAR as IdCargar,
                            NU_CARGA as Carga,
                            TP_MODALIDAD as TipoModalidad,
                            FL_SYNC_REALIZADA as SincronizacionRealizadaId
                        FROM
                            T_CLIENTE_CAMION
                        WHERE CD_CAMION = :cdCamion";

            return _dapper.Query<CargaCamion>(connection, sql, param: new { cdCamion = model.Id }, commandType: CommandType.Text).ToList();
        }

        public virtual async Task AddEgresos(List<Camion> egresos, IEgresoServiceContext context, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                long nuTransaccion = await _transaccionRepository.CreateTransaction($"AddEgresos", connection, app: _application, userId: _userId);

                var bulkContext = GetBulkOperationContext(egresos, context, connection, nuTransaccion);

                using (var tran = connection.BeginTransaction())
                {
                    await BulkInsertEgresos(connection, tran, bulkContext.NewEgresos);
                    await BulkInsertCargas(connection, tran, bulkContext.NewCargas);
                    await BulkUpdatePedidos(connection, tran, bulkContext.UpdatePedidos);
                    await BulkUpdateDetallesPicking(connection, tran, bulkContext.UpdateDetallesPickingPedido, bulkContext.UpdateDetallesPickingContenedor);

                    //Para el movimiento de cargas entre egresos elimino las asignaciones para Carga-Cliente - Empresa de otros camiones previamente a crear las nuevas.
                    //Esta acción es controlada con anterioridad en las validaciones de la petición.
                    //Si no está habilitado el uso de cargas asignadas y la carga esta en otro camión, no se llega a este punto del procedimiento.
                    await BulkDeleteCargasCamion(connection, tran, bulkContext.NewCargasCamion.Values);
                    await BulkInsertCargasCamion(connection, tran, bulkContext.NewCargasCamion.Values);

                    tran.Commit();
                }
            }
        }

        public virtual EgresoBulkOperationContext GetBulkOperationContext(List<Camion> egresos, IEgresoServiceContext context, DbConnection connection, long nuTransaccion)
        {
            var bulkContext = new EgresoBulkOperationContext();
            var egresoIds = GetNewEgresosIds(egresos.Count, connection);
            var cargasIds = GetNewCargasIds(context, connection);

            for (int i = 0; i < egresos.Count; i++)
            {
                var egreso = Map(egresos[i], egresoIds[i], context);
                egreso.NumeroTransaccion = nuTransaccion;

                bulkContext.NewEgresos.Add(GetCamionEntity(egreso));

                if (egreso.DetalleArmadoEgreso != null)
                {
                    BulkPedidos(egreso, context, bulkContext, cargasIds, nuTransaccion);
                    BulkCargas(egreso, context, bulkContext);
                    BulkContenedores(egreso, context, bulkContext, cargasIds, nuTransaccion);
                }
            }

            return bulkContext;
        }

        public virtual List<int> GetNewEgresosIds(int count, DbConnection connection)
        {
            return _dapper.GetNextSequenceValues<int>(connection, Secuencias.S_CD_CAMION, count).ToList();
        }

        public virtual List<long> GetNewCargasIds(IEgresoServiceContext serviceContext, DbConnection connection)
        {
            var result = new List<long>();
            int count = 0;

            count = serviceContext.PedidosConPendientes.Values.Where(p => !p.NuCarga.HasValue).ToList().Count;//Para que genere numero de carga solo para los que se vayan a crear

            count += serviceContext.CargasCompartidasPedidos.Count();
            count += serviceContext.CargasCompartidasContenedores.Count();

            if (count > 0)
                result = _dapper.GetNextSequenceValues<long>(connection, Secuencias.S_CARGA, count).ToList();

            return result;
        }

        public virtual Camion Map(Camion request, int cdCamion, IEgresoServiceContext context)
        {
            var predio = !string.IsNullOrEmpty(request.Predio) ? request.Predio :
                context.Predios.FirstOrDefault(x => x.IdExterno == request.PredioExterno)?.Numero;

            Camion camion = new Camion
            {
                Id = request.Id = cdCamion,
                Ruta = request.Ruta,
                Puerta = request.Puerta,
                Predio = predio,
                Empresa = request.Empresa,
                EstadoId = request.EstadoId,
                Vehiculo = request.Vehiculo,
                Documento = request.Documento,
                IdExterno = request.IdExterno,
                Matricula = request.Matricula,
                Descripcion = request.Descripcion,
                ArmadoEgreso = request.ArmadoEgreso,
                FechaCreacion = request.FechaCreacion,
                Transportista = request.Transportista,
                EmpresaExterna = request.EmpresaExterna,
                FechaProgramado = request.FechaProgramado,
                TipoArmadoEgreso = request.TipoArmadoEgreso,

                RuteoHabilitadoId = request.RuteoHabilitadoId,
                CargaHabilitadaId = request.CargaHabilitadaId,
                CierreHabilitadoId = request.CierreHabilitadoId,
                ArmadoHabilitadoId = request.ArmadoHabilitadoId,
                RespetaOrdenCargaId = request.RespetaOrdenCargaId,
                TrackingHabilitadoId = request.TrackingHabilitadoId,
                CierreParcialHabilitadoId = request.CierreParcialHabilitadoId,
                SincronizacionRealizadaId = request.SincronizacionRealizadaId,
                CargaAutomaticaHabilitadaId = request.CargaAutomaticaHabilitadaId,
                CierreAutomaticoHabilitadoId = request.CierreAutomaticoHabilitadoId,
                ConfirmacionViajeRealizadaId = request.ConfirmacionViajeRealizadaId,
                ControlContenedoresHabilitadoId = request.ControlContenedoresHabilitadoId,
                DetalleArmadoEgreso = request.DetalleArmadoEgreso,

                //Sin persistir
                HabilitarUsoCargaAsignada = request.HabilitarUsoCargaAsignada,
                ProgramacionHoraInicio = request.ProgramacionHoraInicio,
                ProgramacionHoraFin = request.ProgramacionHoraFin,
                Necesidades = request.Necesidades
            };

            return camion;
        }

        public virtual void BulkPedidos(Camion egreso, IEgresoServiceContext context, EgresoBulkOperationContext bulkContext, List<long> cargasIds, long nuTransaccion)
        {
            if (egreso.DetalleArmadoEgreso.Pedidos != null && egreso.DetalleArmadoEgreso.Pedidos.Count > 0)
            {
                foreach (var p in egreso.DetalleArmadoEgreso.Pedidos.Where(p => p.Existe))
                {
                    var keyAgente = $"{p.CodigoAgente}.{p.TipoAgente}.{p.Empresa}";
                    var agente = context.Agentes.GetValueOrDefault(keyAgente, null);

                    var keyPedido = $"{p.NroPedido}.{p.Empresa}.{agente?.CodigoInterno}";

                    Cargas(egreso, context, bulkContext, keyPedido, cargasIds, nuTransaccion);
                    CargaPredefinida(egreso, context, bulkContext, keyPedido, cargasIds, nuTransaccion);
                }
            }
        }

        public virtual void Cargas(Camion egreso, IEgresoServiceContext context, EgresoBulkOperationContext bulkContext, string keyPedido, List<long> cargasIds, long nuTransaccion)
        {
            var cargasLiberadasPedido = context.PedidosCargasLiberadas.GetValueOrDefault(keyPedido, null);
            if (cargasLiberadasPedido != null && cargasLiberadasPedido.Count > 0)
            {
                foreach (var c in cargasLiberadasPedido)
                {
                    var nuCargaFinal = (long)c.Carga;
                    var cargaCompartida = context.CargasCompartidasPedidos.FirstOrDefault(x => x.Id == c.Carga);

                    if (cargaCompartida != null)
                    {
                        var nuevaCarga = cargaCompartida.Copiar();
                        nuevaCarga.Id = cargasIds.FirstOrDefault();
                        nuevaCarga.Descripcion = $"Generada por el Api de egreso para el Pedido:{c.Pedido} - Cliente:{c.Cliente} - Empresa:{c.Empresa}";

                        bulkContext.NewCargas.Add(GetCargaEntity(nuevaCarga));
                        nuCargaFinal = nuevaCarga.Id;
                        cargasIds.Remove(nuevaCarga.Id);

                        bulkContext.UpdateDetallesPickingPedido.Add(new DetallePreparacion()
                        {
                            Pedido = c.Pedido,
                            Empresa = c.Empresa,
                            Cliente = c.Cliente,
                            Carga = nuCargaFinal,
                            Transaccion = nuTransaccion,
                            FechaModificacion = DateTime.Now
                        });
                    }

                    var cargaCamion = new CargaCamion
                    {
                        Camion = egreso.Id,
                        Carga = nuCargaFinal,
                        Cliente = c.Cliente,
                        Empresa = c.Empresa,
                        IdCargar = "S",
                        TipoModalidad = TipoModalidadArmado.Pedido,
                        SincronizacionRealizadaId = egreso.SincronizacionRealizadaId
                    };
                    var keyCargaCamion = $"{cargaCamion.Camion}.{cargaCamion.Carga}.{cargaCamion.Cliente}";
                    if (!bulkContext.NewCargasCamion.ContainsKey(keyCargaCamion))
                        bulkContext.NewCargasCamion[keyCargaCamion] = GetClienteCamionEntity(cargaCamion);
                }
            }
        }

        public virtual void CargaPredefinida(Camion egreso, IEgresoServiceContext context, EgresoBulkOperationContext bulkContext, string keyPedido, List<long> cargasIds, long nuTransaccion)
        {
            var pedido = context.PedidosConPendientes.GetValueOrDefault(keyPedido, null);

            if (pedido != null)
            {
                if (pedido.NuCarga == null)
                {
                    var cargaPredefinida = new Carga()
                    {
                        Id = cargasIds.FirstOrDefault(),
                        Ruta = (short)pedido.Ruta,
                        Descripcion = $"Generada por el Api de egreso para el Pedido {pedido.Id} - Cliente: {pedido.Cliente} - Empresa: {pedido.Empresa}",
                        Preparacion = null,
                        FechaAlta = DateTime.Now
                    };

                    bulkContext.NewCargas.Add(GetCargaEntity(cargaPredefinida));

                    pedido.Transaccion = nuTransaccion;
                    pedido.NuCarga = cargaPredefinida.Id;
                    pedido.FechaModificacion = DateTime.Now;
                    bulkContext.UpdatePedidos.Add(pedido);

                    cargasIds.Remove(cargaPredefinida.Id);
                }

                var cargaCamion = new CargaCamion
                {
                    Camion = egreso.Id,
                    Carga = (long)pedido.NuCarga,
                    Cliente = pedido.Cliente,
                    Empresa = pedido.Empresa,
                    IdCargar = "S",
                    TipoModalidad = TipoModalidadArmado.Pedido,
                    SincronizacionRealizadaId = egreso.SincronizacionRealizadaId,
                    FechaAlta = DateTime.Now
                };

                var keyCargaCamion = $"{cargaCamion.Camion}.{cargaCamion.Carga}.{cargaCamion.Cliente}";
                if (!bulkContext.NewCargasCamion.ContainsKey(keyCargaCamion))
                    bulkContext.NewCargasCamion[keyCargaCamion] = GetClienteCamionEntity(cargaCamion);
            }
        }

        public virtual void BulkCargas(Camion egreso, IEgresoServiceContext context, EgresoBulkOperationContext bulkContext)
        {
            if (egreso.DetalleArmadoEgreso.Cargas != null && egreso.DetalleArmadoEgreso.Cargas.Count > 0)
            {
                foreach (var ca in egreso.DetalleArmadoEgreso.Cargas.Where(ca => ca.Existe))
                {
                    var keyAgente = $"{ca.CodigoAgente}.{ca.TipoAgente}.{ca.Empresa}";
                    var agente = context.Agentes.GetValueOrDefault(keyAgente, null);

                    var cargaCamion = new CargaCamion
                    {
                        Camion = egreso.Id,
                        Carga = ca.Carga,
                        Cliente = agente?.CodigoInterno,
                        Empresa = ca.Empresa,
                        IdCargar = "S",
                        TipoModalidad = TipoModalidadArmado.Carga,
                        SincronizacionRealizadaId = egreso.SincronizacionRealizadaId
                    };
                    var keyCargaCamion = $"{cargaCamion.Camion}.{cargaCamion.Carga}.{cargaCamion.Cliente}";
                    if (!bulkContext.NewCargasCamion.ContainsKey(keyCargaCamion))
                        bulkContext.NewCargasCamion[keyCargaCamion] = GetClienteCamionEntity(cargaCamion);
                }
            }
        }

        public virtual void BulkContenedores(Camion egreso, IEgresoServiceContext context, EgresoBulkOperationContext bulkContext, List<long> cargasIds, long nuTransaccion)
        {
            if (egreso.DetalleArmadoEgreso.Contenedores != null && egreso.DetalleArmadoEgreso.Contenedores.Count > 0)
            {
                foreach (var co in egreso.DetalleArmadoEgreso.Contenedores.Where(co => co.Existe))
                {
                    var keyContenedor = $"{co.IdExternoContenedor}.{co.TipoContenedor}.{co.Empresa}";
                    var cargasContenedor = context.ContenedoresCargasLiberadas.GetValueOrDefault(keyContenedor, null);

                    if (cargasContenedor != null && cargasContenedor.Count > 0)
                    {
                        foreach (var c in cargasContenedor)
                        {
                            var nuCargaFinal = c.Carga;
                            var cargaCompartida = context.CargasCompartidasContenedores.FirstOrDefault(x => x.Id == c.Carga);

                            if (cargaCompartida != null)
                            {
                                var nuevaCarga = cargaCompartida.Copiar();
                                nuevaCarga.Id = cargasIds.FirstOrDefault();

                                bulkContext.NewCargas.Add(GetCargaEntity(nuevaCarga));

                                nuCargaFinal = nuevaCarga.Id;

                                cargasIds.Remove(nuevaCarga.Id);

                                var contenedor = context.ContenedoresHabilitados.GetValueOrDefault(keyContenedor, new Contenedor());

                                bulkContext.UpdateDetallesPickingContenedor.Add(new DetallePreparacion()
                                {
                                    NumeroPreparacion = contenedor.NumeroPreparacion,
                                    NroContenedor = contenedor.Numero,
                                    Carga = nuCargaFinal,
                                    Transaccion = nuTransaccion,
                                    FechaModificacion = DateTime.Now
                                });
                            }

                            var cargaCamion = new CargaCamion
                            {
                                Camion = egreso.Id,
                                Carga = nuCargaFinal,
                                Cliente = c.Cliente,
                                Empresa = c.Empresa,
                                IdCargar = "S",
                                TipoModalidad = TipoModalidadArmado.Contenedor,
                                SincronizacionRealizadaId = egreso.SincronizacionRealizadaId,
                            };

                            var keyCargaCamion = $"{cargaCamion.Camion}.{cargaCamion.Carga}.{cargaCamion.Cliente}";
                            if (!bulkContext.NewCargasCamion.ContainsKey(keyCargaCamion))
                                bulkContext.NewCargasCamion[keyCargaCamion] = GetClienteCamionEntity(cargaCamion);
                        }
                    }
                }
            }
        }

        public virtual async Task BulkInsertEgresos(DbConnection connection, DbTransaction tran, List<object> egresos)
        {
            string sql = @"INSERT INTO T_CAMION 
                    (CD_CAMION, 
                    CD_ROTA,
                    CD_PORTA,
                    NU_PREDIO,
                    CD_EMPRESA,
                    CD_SITUACAO,
                    CD_VEICULO,
                    DS_DOCUMENTO,
                    ID_EXTERNO,
                    CD_PLACA_CARRO,
                    DS_CAMION,
                    ND_ARMADO_EGRESO,
                    DT_ADDROW,
                    CD_TRANSPORTADORA,
                    CD_EMPRESA_EXTERNA,
                    DT_PROGRAMADO,
                    TP_ARMADO_EGRESO,
                    FL_RUTEO,
                    FL_HABILITADO_CARGA,
                    FL_HABILITADO_CIERRE,
                    FL_HABILITADO_ARMADO,
                    ID_RESPETA_ORD_CARGA,
                    FL_TRACKING,
                    FL_CIERRE_PARCIAL,
                    FL_SYNC_REALIZADA,
                    FL_AUTO_CARGA,
                    FL_CIERRE_AUTO,
                    FL_CONF_VIAJE_REALIZADA,
                    FL_CTRL_CONTENEDORES) 
                    VALUES (
                    :Id,
                    :Ruta,
                    :Puerta,
                    :Predio,
                    :Empresa,
                    :EstadoId,
                    :Vehiculo,
                    :Documento,
                    :IdExterno,
                    :Matricula,
                    :Descripcion,
                    :ArmadoEgreso,
                    :FechaCreacion,
                    :Transportista,
                    :EmpresaExterna,
                    :FechaProgramado,
                    :TipoArmadoEgreso,
                    :RuteoHabilitadoId,
                    :CargaHabilitadaId,
                    :CierreHabilitadoId,
                    :ArmadoHabilitadoId,
                    :RespetaOrdenCargaId,
                    :TrackingHabilitadoId,
                    :CierreParcialHabilitadoId,
                    :SincronizacionRealizadaId,
                    :CargaAutomaticaHabilitadaId,
                    :CierreAutomaticoHabilitadoId,
                    :ConfirmacionViajeRealizadaId,
                    :ControlContenedoresHabilitadoId)";

            await _dapper.ExecuteAsync(connection, sql, egresos, transaction: tran);
        }

        public virtual async Task BulkInsertCargas(DbConnection connection, DbTransaction tran, List<object> cargas)
        {
            string sql = @"INSERT INTO T_CARGA 
                    (NU_CARGA, 
                    DS_CARGA,
                    CD_ROTA,
                    NU_PREPARACION,
                    DT_ADDROW) 
                    VALUES (
                    :Id,
                    :Descripcion,
                    :Ruta,
                    :Preparacion,
                    :FechaAlta)";

            await _dapper.ExecuteAsync(connection, sql, cargas, transaction: tran);
        }

        public virtual async Task BulkInsertCargasCamion(DbConnection connection, DbTransaction tran, IEnumerable<object> cargasCamion)
        {
            string sql = @"INSERT INTO T_CLIENTE_CAMION 
                    (CD_CAMION, 
                    NU_CARGA,
                    CD_CLIENTE,
                    CD_EMPRESA,
                    ID_CARGAR,
                    TP_MODALIDAD,
                    DT_ADDROW,
                    FL_SYNC_REALIZADA
                    ) 
                    VALUES (
                    :Camion,
                    :Carga,
                    :Cliente,
                    :Empresa,
                    :IdCargar,
                    :TipoModalidad,
                    :FechaAlta,
                    :SincronizacionRealizadaId   
                    )";

            await _dapper.ExecuteAsync(connection, sql, cargasCamion, transaction: tran);
        }

        public virtual async Task BulkDeleteCargasCamion(DbConnection connection, DbTransaction tran, IEnumerable<object> cargasCamion)
        {
            var sql = @$"DELETE FROM T_CLIENTE_CAMION
                        WHERE NU_CARGA = :Carga AND CD_CLIENTE = :Cliente AND CD_EMPRESA = :Empresa";

            await _dapper.ExecuteAsync(connection, sql, cargasCamion, transaction: tran);
        }

        public virtual async Task BulkUpdatePedidos(DbConnection connection, DbTransaction tran, List<Pedido> pedidos)
        {
            var sql = @"UPDATE T_PEDIDO_SAIDA 
                        SET NU_CARGA = :NuCarga, NU_TRANSACCION = :Transaccion, DT_UPDROW = :FechaModificacion
                        WHERE NU_PEDIDO = :Id AND CD_CLIENTE = :Cliente AND CD_EMPRESA = :Empresa ";

            await _dapper.ExecuteAsync(connection, sql, pedidos, transaction: tran);
        }

        public virtual async Task BulkUpdateDetallesPicking(DbConnection connection, DbTransaction tran, List<DetallePreparacion> cargasPedidos, List<DetallePreparacion> cargasContenedores)
        {
            var sql = @"UPDATE T_DET_PICKING 
                        SET NU_CARGA = :Carga, NU_TRANSACCION = :Transaccion, DT_UPDROW = :FechaModificacion
                        WHERE NU_PEDIDO = :Pedido AND CD_CLIENTE = :Cliente AND CD_EMPRESA = :Empresa ";

            await _dapper.ExecuteAsync(connection, sql, cargasPedidos, transaction: tran);

            sql = @"UPDATE T_DET_PICKING 
                        SET NU_CARGA = :Carga, NU_TRANSACCION = :Transaccion, DT_UPDROW = :FechaModificacion
                        WHERE NU_PREPARACION = :NumeroPreparacion AND NU_CONTENEDOR = :NroContenedor ";

            await _dapper.ExecuteAsync(connection, sql, cargasContenedores, transaction: tran);
        }

        public static object GetCamionEntity(Camion camion)
        {
            return new
            {
                Id = camion.Id,
                Ruta = camion.Ruta,
                Puerta = camion.Puerta,
                Predio = camion.Predio,
                Empresa = camion.Empresa,
                EstadoId = camion.EstadoId,
                Vehiculo = camion.Vehiculo,
                Documento = camion.Documento,
                IdExterno = camion.IdExterno,
                Matricula = camion.Matricula,
                Descripcion = camion.Descripcion,
                ArmadoEgreso = camion.ArmadoEgreso,
                FechaCreacion = camion.FechaCreacion,
                Transportista = camion.Transportista,
                EmpresaExterna = camion.EmpresaExterna,
                FechaProgramado = camion.FechaProgramado,
                TipoArmadoEgreso = camion.TipoArmadoEgreso,

                RuteoHabilitadoId = camion.RuteoHabilitadoId,
                CargaHabilitadaId = camion.CargaHabilitadaId,
                CierreHabilitadoId = camion.CierreHabilitadoId,
                ArmadoHabilitadoId = camion.ArmadoHabilitadoId,
                RespetaOrdenCargaId = camion.RespetaOrdenCargaId,
                TrackingHabilitadoId = camion.TrackingHabilitadoId,
                CierreParcialHabilitadoId = camion.CierreParcialHabilitadoId,
                SincronizacionRealizadaId = camion.SincronizacionRealizadaId,
                CargaAutomaticaHabilitadaId = camion.CargaAutomaticaHabilitadaId,
                CierreAutomaticoHabilitadoId = camion.CierreAutomaticoHabilitadoId,
                ConfirmacionViajeRealizadaId = camion.ConfirmacionViajeRealizadaId,
                ControlContenedoresHabilitadoId = camion.ControlContenedoresHabilitadoId,
                DetalleArmadoEgreso = camion.DetalleArmadoEgreso,

                //Sin persistir
                HabilitarUsoCargaAsignada = camion.HabilitarUsoCargaAsignada,
                ProgramacionHoraInicio = camion.ProgramacionHoraInicio,
                ProgramacionHoraFin = camion.ProgramacionHoraFin,
                Necesidades = camion.Necesidades
            };
        }

        public static object GetCargaEntity(Carga carga)
        {
            return new
            {
                Id = carga.Id,
                Ruta = carga.Ruta,
                Descripcion = carga.Descripcion,
                Preparacion = carga.Preparacion,
                FechaAlta = DateTime.Now
            };
        }

        public static object GetClienteCamionEntity(CargaCamion cargaCamion)
        {
            return new
            {
                Camion = cargaCamion.Camion,
                Carga = cargaCamion.Carga,
                Cliente = cargaCamion.Cliente,
                Empresa = cargaCamion.Empresa,
                IdCargar = cargaCamion.IdCargar,
                TipoModalidad = cargaCamion.TipoModalidad,
                SincronizacionRealizadaId = cargaCamion.SincronizacionRealizadaId,
                FechaAlta = DateTime.Now
            };
        }

        public static object GetDeleteClienteCamionEntity(long carga, string cliente, int empresa)
        {
            return new
            {
                Carga = carga,
                Cliente = cliente,
                Empresa = empresa,
            };
        }

        #region Api Salida
        public virtual async Task<List<APITask>> GetCamionesPendientesConfirmarCierre(CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);
                return GetCamionesPendientes(connection);
            }
        }

        public virtual async Task<List<APITask>> GetCamionesPendientesDeFacturacion(CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);
                return GetCamionesPendientes(connection, true);
            }
        }

        public virtual List<APITask> GetCamionesPendientes(DbConnection connection, bool facturacion = false)
        {
            var sql = @"SELECT 
                            ID_OPERACION AS Id,
                            DT_OPERACION AS Fecha
                        FROM V_CONFIRMACIONES_PENDIENTES
                        WHERE CD_INTERFAZ_EXTERNA = :cdInterfazExterna 
                            AND FL_HABILITADA= 'S'
                        ORDER BY 
                            DT_OPERACION ASC, 
                            ID_OPERACION ASC";

            return _dapper.Query<APITask>(connection, sql, param: new
            {
                cdInterfazExterna = facturacion ? CInterfazExterna.Facturacion : CInterfazExterna.ConfirmacionDePedido
            }, commandType: CommandType.Text).ToList();
        }

        public virtual async Task<List<long>> GenerarInterfacesFacturacion(int cdCamion, Func<int?, string> GetGrupoConsulta, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                logger.Debug($"Facturación. Camión: {cdCamion}");

                var nuEjecuciones = new List<long>();
                long nuTransaccion = await _transaccionRepository.CreateTransaction($"Generación de interfaz de facturación. Camión: {cdCamion}", connection, app: _application, userId: _userId);

                try
                {
                    await RegularizarDatosSerializados(connection, cdCamion, nuTransaccion, facturacion: true);
                }
                catch (Exception ex) { }

                try
                {
                    using (var tran = connection.BeginTransaction())
                    {
                        var finalizarEmpaquetado = false;
                        var camionEjecContext = new BulkCamionEjecucionContext();
                        var empresas = GetEmpresasCamion(cdCamion, connection, tran, true);

                        foreach (var empresa in empresas)
                        {
                            var interfazHabilitada = _parametroRepository.GetParamInterfazHabilitada(CInterfazExterna.Facturacion, empresa, connection, tran);

                            if (!interfazHabilitada)
                            {
                                logger.Debug($"La interfaz {CInterfazExterna.Facturacion} no esta habilitada para la empresa {empresa}.");
                                continue;
                            }

                            logger.Debug($"Procesamiento de datos para la empresa {empresa}");

                            var dsReferencia = $"Interfaz de salida de facturación. Camión: {cdCamion}, Empresa: {empresa}";
                            var grupoConsulta = GetGrupoConsulta(empresa);
                            var ejecucion = await CrearEjecucion(cdCamion, empresa, grupoConsulta, CInterfazExterna.Facturacion, connection, tran, dsReferencia);
                            var camion = MapFacturacion(cdCamion, empresa, ejecucion.Id, nuTransaccion, connection, tran, out BulkUpdatePedidosContext context, out BulkUpdateDuplicadosContext contextDup, out finalizarEmpaquetado);

                            await CrearEjecucionData(ejecucion.Id, camion, empresa, connection, tran);

                            camionEjecContext.Datos.Add(GetCamionEjecucionEntity(cdCamion, ejecucion.Id, "S"));
                            nuEjecuciones.Add(ejecucion.Id);

                            await InsertDuplicados(contextDup, connection, tran, true);
                            await UpdateDuplicados(contextDup, connection, tran, true);
                            await UpdatePedidos(context, connection, tran);
                        }

                        if (nuEjecuciones.Any())
                        {
                            await InsertCamionEjecucion(camionEjecContext, connection, tran);
                            await UpdateCamion(cdCamion, nuEjecuciones.LastOrDefault(), nuTransaccion, connection, tran, true);

                            if (finalizarEmpaquetado)
                                await FinalizarEmpaquetado(cdCamion, nuTransaccion, connection, tran);
                        }
                        else
                            await UpdateCamion(cdCamion, 0, nuTransaccion, connection, tran, true);

                        tran.Commit();
                        logger.Debug($"Interfaz de facturación terminada.");
                    }
                }
                catch (Exception ex)
                {
                    nuEjecuciones.Clear();
                    logger.Error($"Error al facturar. Camión: {cdCamion} - Error: {ex}");
                    await UpdateCamion(cdCamion, -2, nuTransaccion, connection, null, true);
                }

                return nuEjecuciones;
            }
        }

        public virtual async Task<List<long>> GenerarInterfacesCierre(int cdCamion, Func<int?, string> GetGrupoConsulta, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                logger.Debug($"Confirmación de Pedidos. Camión: {cdCamion}");

                var nuEjecuciones = new List<long>();
                long nuTransaccion = await _transaccionRepository.CreateTransaction($"Generación de interfaz de salida. Camión: {cdCamion}", connection, app: _application, userId: _userId);

                try
                {
                    await RegularizarDatosSerializados(connection, cdCamion, nuTransaccion, facturacion: false);
                }
                catch (Exception ex) { }

                try
                {
                    using (var tran = connection.BeginTransaction())
                    {
                        var camionEjecContext = new BulkCamionEjecucionContext();
                        var empresas = GetEmpresasCamion(cdCamion, connection, tran);

                        if (empresas != null && empresas.Count > 0)
                        {
                            foreach (var empresa in empresas)
                            {
                                var interfazHabilitada = _parametroRepository.GetParamInterfazHabilitada(CInterfazExterna.ConfirmacionDePedido, empresa, connection, tran);

                                if (!interfazHabilitada)
                                {
                                    logger.Debug($"La interfaz {CInterfazExterna.ConfirmacionDePedido} no esta habilitada para la empresa {empresa}.");
                                    continue;
                                }

                                logger.Debug($"Procesamiento de datos para la empresa {empresa}");

                                var dsReferencia = $"Interfaz de salida de confirmación de pedidos. Camión: {cdCamion}, Empresa: {empresa}";
                                var grupoConsulta = GetGrupoConsulta(empresa);
                                var ejecucion = await CrearEjecucion(cdCamion, empresa, grupoConsulta, CInterfazExterna.ConfirmacionDePedido, connection, tran, dsReferencia);
                                var camion = Map(cdCamion, empresa, ejecucion.Id, nuTransaccion, connection, tran, out BulkUpdateEntregasContext context, out BulkUpdateDuplicadosContext contextDup);

                                await CrearEjecucionData(ejecucion.Id, camion, empresa, connection, tran);

                                camionEjecContext.Datos.Add(GetCamionEjecucionEntity(cdCamion, ejecucion.Id, "N"));
                                nuEjecuciones.Add(ejecucion.Id);

                                await InsertDuplicados(contextDup, connection, tran);
                                await UpdateDuplicados(contextDup, connection, tran);
                                await UpdateEntregas(context, connection, tran);
                            }

                            if (nuEjecuciones.Any())
                            {
                                await InsertCamionEjecucion(camionEjecContext, connection, tran);
                                await UpdateCamion(cdCamion, nuEjecuciones.LastOrDefault(), nuTransaccion, connection, tran);
                            }
                        }
                        else
                        {
                            await UpdateCamion(cdCamion, 0, nuTransaccion, connection, tran);
                        }

                        tran.Commit();

                        logger.Debug($"Interfaz de confirmación de pedido terminada.");
                    }
                }
                catch (Exception ex)
                {
                    nuEjecuciones.Clear();
                    logger.Error($"Error en la confirmación de pedidos. Camión: {cdCamion} - Error: {ex}");
                    await UpdateCamion(cdCamion, -2, nuTransaccion, connection, null);
                }

                return nuEjecuciones;
            }
        }

        public virtual ConfirmacionPedidoResponse Map(int cdcamion, int empresa, long nuEjecucion, long nuTransaccion, DbConnection connection, DbTransaction tran, out BulkUpdateEntregasContext context, out BulkUpdateDuplicadosContext contextDup)
        {
            context = new BulkUpdateEntregasContext();
            contextDup = new BulkUpdateDuplicadosContext();

            var agenteRepository = new AgenteRepository(_context, _application, _userId, _dapper);
            var lpnRepository = new ManejoLpnRepository(_context, _application, _userId, _dapper);

            var camion = GetCamionOrNull(cdcamion).Result;

            var model = new ConfirmacionPedidoResponse()
            {
                Camion = camion.Id.ToString(),
                DescripcionCamion = camion.Descripcion,
                Ruta = camion.Ruta,
                Vehiculo = camion.Vehiculo,
                Matricula = camion.Matricula,
                Transportadora = camion.Transportista,
                FechaCierre = camion.FechaCierre?.ToString(CDateFormats.DATE_ONLY),
                FechaFacturacion = camion.FechaFacturacion?.ToString(CDateFormats.DATE_ONLY),
                Predio = camion.Predio,
                PuertaExpedicion = camion.Puerta
            };

            var pedidos = GetPedidosCamion(cdcamion, empresa, connection, tran);

            foreach (var pedido in pedidos)
            {
                var agente = agenteRepository.GetAgenteOrNull(empresa, pedido.Cliente).Result;
                var modelPedido = new PedidoSalidaResponse()
                {
                    Pedido = pedido.Id,
                    Empresa = pedido.Empresa,
                    CodigoAgente = agente?.Codigo,
                    TipoAgente = agente?.Tipo,
                    CodigoOrigen = pedido.Origen,
                    Memo = pedido.Memo,
                    Memo1 = pedido.Memo1,
                    CondicionLiberacion = pedido.CondicionLiberacion,
                    Predio = pedido.Predio,
                    TipoPedido = pedido.Tipo,
                    TipoExpedicion = pedido.TipoExpedicionId,
                    Direccion = pedido.DireccionEntrega,
                    PuntoEntrega = pedido.PuntoEntrega,
                    Serializado = pedido.VlSerealizado_1,
                };

                var detalles = GetDetallesPedidoExpedido(cdcamion, pedido, connection, tran);
                var duplicados = GetAllDetallesDuplicadosExp(detalles);

                foreach (var detalle in detalles)
                {
                    var modelPedidoDetalle = new DetallePedidoSalidaResponse()
                    {
                        Producto = detalle.Producto,
                        Identificador = detalle.Identificador,
                        EspecificaIdentificador = detalle.EspecificaLoteId,
                        CantidadProducto = detalle.Cantidad ?? 0,
                        Memo = detalle.Memo,
                        Serializado = detalle.Serializado,
                    };

                    DetallePedidoDuplicado auxDup = null;
                    var saldoExPedido = detalle.Cantidad ?? 0;
                    var flujoAuto = (detalle.EspecificaLoteId == "N") ? true : false;
                    var detallesDuplicado = FiltroDuplicados(duplicados, detalle, flujoAuto);

                    foreach (var detalleDup in detallesDuplicado.Where(d => (d.CantidadFacturada != null && d.CantidadFacturada > 0)).OrderByDescending(d => d.CantidadFacturada).ThenBy(d => d.IdLineaSistemaExterno))
                    {
                        var modelDetalleDuplicado = new DetallePedidoSalidaDuplicadoResponse()
                        {
                            IdLineaSistemaExterno = detalleDup.IdLineaSistemaExterno,
                            TipoLinea = detalleDup.TipoLinea,
                            Serializado = detalleDup.DatosSerializados,
                        };

                        detalleDup.Transaccion = nuTransaccion;

                        if (saldoExPedido > 0)
                        {
                            if (detalleDup.Identificador != ManejoIdentificadorDb.IdentificadorAuto)
                            {
                                var saldoLinea = detalleDup.CantidadPedida - (detalleDup.CantidadExpedida ?? 0) - (detalleDup.CantidadAnulada ?? 0);

                                if (saldoLinea > 0)
                                {
                                    decimal cantExpedida = 0;

                                    if (saldoExPedido >= saldoLinea)
                                    {
                                        cantExpedida = saldoLinea;
                                        detalleDup.CantidadExpedida = (detalleDup.CantidadExpedida ?? 0) + saldoLinea;
                                        saldoExPedido -= saldoLinea;
                                    }
                                    else
                                    {
                                        cantExpedida = saldoExPedido;
                                        detalleDup.CantidadExpedida = (detalleDup.CantidadExpedida ?? 0) + saldoExPedido;
                                        saldoExPedido = 0;
                                    }

                                    modelDetalleDuplicado.CantidadProducto = cantExpedida;
                                    modelPedidoDetalle.Duplicados.Add(modelDetalleDuplicado);
                                    AddBulkUpdate(contextDup, detalleDup);

                                }
                            }
                            else
                            {
                                var saldoLinea = detalleDup.CantidadPedida - (detalleDup.CantidadAnulada ?? 0);

                                if (saldoLinea > 0)
                                {
                                    decimal cantExpedida = 0;
                                    if (saldoExPedido >= saldoLinea)
                                    {
                                        cantExpedida = saldoLinea;
                                        detalleDup.CantidadPedida = detalleDup.CantidadPedida - saldoLinea;
                                        saldoExPedido -= saldoLinea;
                                    }
                                    else
                                    {
                                        cantExpedida = saldoExPedido;
                                        detalleDup.CantidadPedida = detalleDup.CantidadPedida - saldoExPedido;
                                        saldoExPedido = 0;
                                    }
                                    AddBulkUpdate(contextDup, detalleDup);

                                    if (auxDup != null)
                                    {
                                        auxDup.CantidadPedida = auxDup.CantidadPedida + cantExpedida;
                                        auxDup.CantidadExpedida = (auxDup.CantidadExpedida ?? 0) + cantExpedida;
                                        AddBulkUpdate(contextDup, auxDup);
                                    }
                                    else
                                    {
                                        var newDuplicado = new DetallePedidoDuplicado()
                                        {
                                            Pedido = detalle.Pedido,
                                            Empresa = detalle.Empresa,
                                            Cliente = detalle.Cliente,
                                            Producto = detalle.Producto,
                                            Identificador = detalle.Identificador,
                                            Faixa = detalle.Faixa,
                                            IdEspecificaIdentificador = detalle.EspecificaLoteId,
                                            IdLineaSistemaExterno = detalleDup.IdLineaSistemaExterno,
                                            TipoLinea = detalleDup.TipoLinea,
                                            CantidadAnulada = null,
                                            CantidadPedida = cantExpedida,
                                            CantidadExpedida = cantExpedida,
                                            CantidadFacturada = null,
                                            DatosSerializados = detalleDup.DatosSerializados,
                                            Transaccion = nuTransaccion,
                                        };

                                        contextDup.NewDuplicados.Add(GetDuplicadosEntity(newDuplicado));
                                    }

                                    modelDetalleDuplicado.CantidadProducto = cantExpedida;
                                    modelPedidoDetalle.Duplicados.Add(modelDetalleDuplicado);
                                }
                            }
                        }
                        else break;
                    }


                    var detallesDuplicadoAux = detallesDuplicado.Where(d => (d.CantidadFacturada == null || d.CantidadFacturada == 0)).ToList();
                    if (!flujoAuto)
                        detallesDuplicadoAux = detallesDuplicadoAux.OrderByDescending(d => d.CantidadAnulada).ThenBy(d => d.IdLineaSistemaExterno).ToList();

                    foreach (var detalleDup in detallesDuplicadoAux)
                    {
                        var modelDetalleDuplicado = new DetallePedidoSalidaDuplicadoResponse()
                        {
                            IdLineaSistemaExterno = detalleDup.IdLineaSistemaExterno,
                            TipoLinea = detalleDup.TipoLinea,
                            Serializado = detalleDup.DatosSerializados,
                        };

                        detalleDup.Transaccion = nuTransaccion;

                        if (saldoExPedido > 0)
                        {
                            if (detalleDup.Identificador != ManejoIdentificadorDb.IdentificadorAuto)
                            {
                                var saldoLinea = detalleDup.CantidadPedida - (detalleDup.CantidadExpedida ?? 0) - (detalleDup.CantidadAnulada ?? 0); ;

                                if (saldoLinea > 0 && !flujoAuto)
                                {
                                    decimal cantExpedida = 0;

                                    if (saldoExPedido >= saldoLinea)
                                    {
                                        cantExpedida = saldoLinea;
                                        detalleDup.CantidadExpedida = (detalleDup.CantidadExpedida ?? 0) + saldoLinea;
                                        saldoExPedido -= saldoLinea;
                                    }
                                    else
                                    {
                                        cantExpedida = saldoExPedido;
                                        detalleDup.CantidadExpedida = (detalleDup.CantidadExpedida ?? 0) + saldoExPedido;
                                        saldoExPedido = 0;
                                    }

                                    modelDetalleDuplicado.CantidadProducto = cantExpedida;
                                    modelPedidoDetalle.Duplicados.Add(modelDetalleDuplicado);
                                    AddBulkUpdate(contextDup, detalleDup);
                                }
                                else if (flujoAuto)
                                {
                                    auxDup = detalleDup;
                                    continue;
                                }
                            }
                            else
                            {
                                var saldoLinea = detalleDup.CantidadPedida - (detalleDup.CantidadAnulada ?? 0);

                                if (saldoLinea > 0)
                                {
                                    decimal cantExpedida = 0;
                                    if (saldoExPedido >= saldoLinea)
                                    {
                                        cantExpedida = saldoLinea;
                                        detalleDup.CantidadPedida = detalleDup.CantidadPedida - saldoLinea;
                                        saldoExPedido -= saldoLinea;
                                    }
                                    else
                                    {
                                        cantExpedida = saldoExPedido;
                                        detalleDup.CantidadPedida = detalleDup.CantidadPedida - saldoExPedido;
                                        saldoExPedido = 0;
                                    }
                                    AddBulkUpdate(contextDup, detalleDup);

                                    if (auxDup != null)
                                    {
                                        auxDup.CantidadPedida = auxDup.CantidadPedida + cantExpedida;
                                        auxDup.CantidadExpedida = (auxDup.CantidadExpedida ?? 0) + cantExpedida;
                                        AddBulkUpdate(contextDup, auxDup);
                                    }
                                    else
                                    {
                                        var newDuplicado = new DetallePedidoDuplicado()
                                        {
                                            Pedido = detalle.Pedido,
                                            Empresa = detalle.Empresa,
                                            Cliente = detalle.Cliente,
                                            Producto = detalle.Producto,
                                            Identificador = detalle.Identificador,
                                            Faixa = detalle.Faixa,
                                            IdEspecificaIdentificador = detalle.EspecificaLoteId,
                                            IdLineaSistemaExterno = detalleDup.IdLineaSistemaExterno,
                                            TipoLinea = detalleDup.TipoLinea,
                                            CantidadAnulada = null,
                                            CantidadPedida = cantExpedida,
                                            CantidadExpedida = cantExpedida,
                                            CantidadFacturada = null,
                                            DatosSerializados = detalleDup.DatosSerializados,
                                            Transaccion = nuTransaccion,
                                        };

                                        contextDup.NewDuplicados.Add(GetDuplicadosEntity(newDuplicado));
                                    }

                                    modelDetalleDuplicado.CantidadProducto = cantExpedida;
                                    modelPedidoDetalle.Duplicados.Add(modelDetalleDuplicado);
                                }
                            }
                        }
                        else break;
                    }
                    modelPedido.Detalles.Add(modelPedidoDetalle);
                }
                model.Pedidos.Add(modelPedido);
            }

            #region Contenedores

            var contenedores = GetContenedoresCamion(cdcamion, empresa, connection, tran);

            foreach (var contenedor in contenedores)
            {
                var detallesPicking = GetLineasPicking(contenedor.Numero, contenedor.NumeroPreparacion, empresa, connection, tran);

                if (detallesPicking.Count == 0)
                    continue; //Si no hay lineas en el contenedor para la empresa, omito el contenedor

                var modelContenedor = new ContenedorResponse()
                {
                    Preparacion = contenedor.NumeroPreparacion,
                    NumeroContenedor = contenedor.Numero,
                    IdExternoContenedor = contenedor.IdExterno,
                    TipoContenedor = contenedor.TipoContenedor,
                    SubClase = contenedor.CodigoSubClase,
                    FechaExpedicion = contenedor.FechaExpedido?.ToString(CDateFormats.DATE_ONLY),
                    PesoReal = contenedor.PesoReal,
                    Altura = contenedor.Altura,
                    Largo = contenedor.Largo,
                    Volumen = contenedor.ValorCubagem,
                    Profundidad = contenedor.Profundidad,
                    CodigoUnidadBulto = contenedor.CodigoUnidadBulto,
                    CantidadBultos = contenedor.CantidadBulto,
                    DescripcionContenedor = contenedor.DescripcionContenedor,
                    Precinto1 = contenedor.Precinto1,
                    Precinto2 = contenedor.Precinto2,
                    CodigoBarras = contenedor.CodigoBarras,
                    NroLpn = contenedor.NroLpn,
                };

                foreach (var detalle in detallesPicking)
                {
                    var agente = agenteRepository.GetAgenteOrNull(empresa, detalle.Cliente).Result;

                    modelContenedor.Detalles.Add(new ContenedorDetalleResponse()
                    {
                        Pedido = detalle.Pedido,
                        CodigoAgente = agente?.Codigo,
                        TipoAgente = agente?.Tipo,
                        Producto = detalle.Producto,
                        Identificador = detalle.Lote,
                        CantidadPreparada = detalle.CantidadPreparada ?? 0,
                        FechaVencimientoPickeo = detalle.VencimientoPickeo?.ToString(CDateFormats.DATE_ONLY),
                        AveriaPickeo = detalle.AveriaPickeo,
                    });

                }

                model.Contenedores.Add(modelContenedor);

                if (contenedor.NroLpn != null)
                {
                    var lpn = lpnRepository.GetLpn(contenedor.NroLpn.Value);
                    var atributosCabezal = lpnRepository.GetAllLpnAtributo(contenedor.NroLpn.Value);
                    var atributosDetalle = lpnRepository.GetAllLpnDetalleAtributo(contenedor.NroLpn.Value);

                    model.Lpns.Add(Map(lpn, atributosCabezal, atributosDetalle, expedicion: true));
                }
            }

            #endregion

            #region Entregas
            /*var entregas = GetEntregas(cdcamion, empresa, connection, tran);

            foreach (var entrega in entregas)
            {
                var agente = agenteRepository.GetAgenteOrNull(empresa, entrega.Cliente).Result;

                model.Entregas.Add(new EntregaResponse()
                {
                    NumeroEntrega = int.Parse(entrega.Id),
                    TipoEntrega = entrega.TipoEntrega,
                    DescripcionEntrega = entrega.DescripcionEntrega,
                    CodigoAgente = agente.Codigo,
                    TipoAgente = agente.Tipo,
                    Empresa = entrega.Empresa,
                    CodigoBarras = entrega.CodigoBarras,
                    NumeroContenedor = entrega.Contenedor.Value,
                    NumeroPreparacion = entrega.Preparacion,
                    Agrupacion = entrega.AgrupacionEntrega,
                    PuntoEntrega = entrega.PuntoEntrega,
                    Anexo = entrega.Anexo,
                });

                context.Entregas.Add(GetEntregaEntity(entrega, nuEjecucion));
            }*/
            #endregion

            return model;
        }

        public virtual void AddBulkUpdate(BulkUpdateDuplicadosContext contextDup, DetallePedidoDuplicado detalleDup)
        {
            string key = $"{detalleDup.Pedido}.{detalleDup.Cliente}.{detalleDup.Empresa}.{detalleDup.Producto}.{detalleDup.Faixa.ToString("#.###")}.{detalleDup.Identificador}.{detalleDup.IdEspecificaIdentificador}.{detalleDup.IdLineaSistemaExterno}";

            if (contextDup.UpdateDuplicados.ContainsKey(key))
                contextDup.UpdateDuplicados.Remove(key);

            contextDup.UpdateDuplicados[key] = GetDuplicadosEntity(detalleDup);
        }

        public virtual FacturacionResponse MapFacturacion(int cdcamion, int empresa, long nuEjecucion, long nuTransaccion, DbConnection connection, DbTransaction tran, out BulkUpdatePedidosContext context, out BulkUpdateDuplicadosContext contextDup, out bool finalizarEmpaquetado)
        {
            context = new BulkUpdatePedidosContext();
            contextDup = new BulkUpdateDuplicadosContext();
            finalizarEmpaquetado = false;

            var agenteRepository = new AgenteRepository(_context, _application, _userId, _dapper);
            var lpnRepository = new ManejoLpnRepository(_context, _application, _userId, _dapper);

            var camion = GetCamionOrNull(cdcamion).Result;

            if (camion.TipoArmadoEgreso == TipoArmadoEgreso.Empaque)
                finalizarEmpaquetado = true;

            var model = new FacturacionResponse()
            {
                CodigoCamion = camion.Id,
                DescripcionCamion = camion.Descripcion,
                Ruta = camion.Ruta,
                Vehiculo = camion.Vehiculo,
                MatriculaCamion = camion.Matricula,
                Transportadora = camion.Transportista,
                FechaFacturacion = camion.FechaFacturacion?.ToString(CDateFormats.DATE_ONLY),
                PuertaExpedicion = camion.Puerta,
                Predio = camion.Predio
            };

            #region Pedidos

            var pedidos = GetPedidosCamion(cdcamion, empresa, connection, tran, true);

            foreach (var pedido in pedidos)
            {
                pedido.Transaccion = nuTransaccion;

                var agente = agenteRepository.GetAgenteOrNull(empresa, pedido.Cliente).Result;
                var modelPedido = new PedidoSalidaResponse()
                {
                    Pedido = pedido.Id,
                    Empresa = pedido.Empresa,
                    CodigoAgente = agente?.Codigo,
                    TipoAgente = agente?.Tipo,
                    CodigoOrigen = pedido.Origen,
                    Memo = pedido.Memo,
                    Memo1 = pedido.Memo1,
                    CondicionLiberacion = pedido.CondicionLiberacion,
                    Predio = pedido.Predio,
                    TipoPedido = pedido.Tipo,
                    TipoExpedicion = pedido.TipoExpedicionId,
                    Direccion = pedido.DireccionEntrega,
                    PuntoEntrega = pedido.PuntoEntrega,
                    Serializado = pedido.VlSerealizado_1,
                };

                var detalles = GetDetallesPedidoFacturacion(cdcamion, pedido, connection, tran);
                var duplicados = GetAllDetallesDuplicadosFact(detalles);

                foreach (var detalle in detalles)
                {
                    var cantidadesPicking = GetCantidadesPicking(cdcamion, detalle, connection, tran);

                    var modelPedidoDetalle = new DetallePedidoSalidaResponse()
                    {
                        Producto = detalle.Producto,
                        Identificador = detalle.Identificador,
                        EspecificaIdentificador = detalle.EspecificaIdentificadorId,
                        CantidadProducto = (cantidadesPicking.CantidadPreparada > 0) ? cantidadesPicking.CantidadPreparada : cantidadesPicking.CantidadProducto,
                        Memo = detalle.Memo,
                        Serializado = detalle.DatosSerializados,
                    };

                    DetallePedidoDuplicado auxDup = null;
                    var saldoFacturado = modelPedidoDetalle.CantidadProducto;
                    var flujoAuto = (detalle.EspecificaIdentificadorId == "N") ? true : false;
                    var detallesDuplicado = FiltroDuplicados(duplicados, detalle, flujoAuto);

                    foreach (var detalleDup in detallesDuplicado.Where(x => (x.CantidadAnulada != null && x.CantidadAnulada > 0)).OrderByDescending(x => x.CantidadAnulada).ThenBy(d => d.IdLineaSistemaExterno))
                    {
                        var modelDetalleDuplicado = new DetallePedidoSalidaDuplicadoResponse()
                        {
                            IdLineaSistemaExterno = detalleDup.IdLineaSistemaExterno,
                            TipoLinea = detalleDup.TipoLinea,
                            Serializado = detalleDup.DatosSerializados,
                        };

                        detalleDup.Transaccion = nuTransaccion;

                        if (saldoFacturado > 0)
                        {
                            if (detalleDup.Identificador != ManejoIdentificadorDb.IdentificadorAuto)
                            {
                                var saldoLinea = detalleDup.CantidadPedida - (detalleDup.CantidadFacturada ?? 0) - (detalleDup.CantidadAnulada ?? 0);

                                if (saldoLinea > 0 && !flujoAuto)
                                {
                                    decimal cantFacturada = 0;

                                    if (saldoFacturado >= saldoLinea)
                                    {
                                        cantFacturada = saldoLinea;
                                        detalleDup.CantidadFacturada = (detalleDup.CantidadFacturada ?? 0) + saldoLinea;
                                        saldoFacturado -= saldoLinea;
                                    }
                                    else
                                    {
                                        cantFacturada = saldoFacturado;
                                        detalleDup.CantidadFacturada = (detalleDup.CantidadFacturada ?? 0) + saldoFacturado;
                                        saldoFacturado = 0;
                                    }
                                    modelDetalleDuplicado.CantidadProducto = cantFacturada;
                                    modelPedidoDetalle.Duplicados.Add(modelDetalleDuplicado);
                                    AddBulkUpdate(contextDup, detalleDup);
                                }
                                else if (flujoAuto)
                                {
                                    auxDup = detalleDup;
                                    continue;
                                }
                            }
                            else
                            {
                                var saldoLinea = detalleDup.CantidadPedida - (detalleDup.CantidadAnulada ?? 0);

                                if (saldoLinea > 0)
                                {
                                    decimal cantFacturada = 0;
                                    if (saldoFacturado >= saldoLinea)
                                    {
                                        cantFacturada = saldoLinea;
                                        detalleDup.CantidadPedida = detalleDup.CantidadPedida - saldoLinea;
                                        saldoFacturado -= saldoLinea;
                                    }
                                    else
                                    {
                                        cantFacturada = saldoFacturado;
                                        detalleDup.CantidadPedida = detalleDup.CantidadPedida - saldoFacturado;
                                        saldoFacturado = 0;
                                    }
                                    AddBulkUpdate(contextDup, detalleDup);

                                    if (auxDup != null)
                                    {
                                        auxDup.CantidadPedida = auxDup.CantidadPedida + cantFacturada;
                                        auxDup.CantidadFacturada = (auxDup.CantidadFacturada ?? 0) + cantFacturada;
                                        AddBulkUpdate(contextDup, auxDup);
                                    }
                                    else
                                    {
                                        var newDuplicado = new DetallePedidoDuplicado()
                                        {
                                            Pedido = detalle.Id,
                                            Empresa = detalle.Empresa,
                                            Cliente = detalle.Cliente,
                                            Producto = detalle.Producto,
                                            Identificador = detalle.Identificador,
                                            Faixa = detalle.Faixa,
                                            IdEspecificaIdentificador = detalle.EspecificaIdentificadorId,
                                            IdLineaSistemaExterno = detalleDup.IdLineaSistemaExterno,
                                            TipoLinea = detalleDup.TipoLinea,
                                            CantidadAnulada = null,
                                            CantidadPedida = cantFacturada,
                                            CantidadExpedida = null,
                                            CantidadFacturada = cantFacturada,
                                            DatosSerializados = detalleDup.DatosSerializados,
                                            Transaccion = detalleDup.Transaccion,
                                        };

                                        contextDup.NewDuplicados.Add(GetDuplicadosEntity(newDuplicado));
                                    }
                                    modelDetalleDuplicado.CantidadProducto = cantFacturada;
                                    modelPedidoDetalle.Duplicados.Add(modelDetalleDuplicado);
                                }
                            }
                        }
                        else break;
                    }

                    var detallesDuplicadoAux = detallesDuplicado.Where(d => (d.CantidadAnulada == null || d.CantidadAnulada == 0)).ToList();
                    if (!flujoAuto)
                        detallesDuplicadoAux = detallesDuplicadoAux.OrderBy(d => d.IdLineaSistemaExterno).ToList();

                    foreach (var detalleDup in detallesDuplicadoAux)
                    {
                        var modelDetalleDuplicado = new DetallePedidoSalidaDuplicadoResponse()
                        {
                            IdLineaSistemaExterno = detalleDup.IdLineaSistemaExterno,
                            TipoLinea = detalleDup.TipoLinea,
                            Serializado = detalleDup.DatosSerializados,
                        };

                        detalleDup.Transaccion = nuTransaccion;

                        if (saldoFacturado > 0)
                        {
                            if (detalleDup.Identificador != ManejoIdentificadorDb.IdentificadorAuto)
                            {
                                var saldoLinea = detalleDup.CantidadPedida - (detalleDup.CantidadFacturada ?? 0) - (detalleDup.CantidadAnulada ?? 0);

                                if (saldoLinea > 0 && !flujoAuto)
                                {
                                    decimal cantFacturada = 0;

                                    if (saldoFacturado >= saldoLinea)
                                    {
                                        cantFacturada = saldoLinea;
                                        detalleDup.CantidadFacturada = (detalleDup.CantidadFacturada ?? 0) + saldoLinea;
                                        saldoFacturado -= saldoLinea;
                                    }
                                    else
                                    {
                                        cantFacturada = saldoFacturado;
                                        detalleDup.CantidadFacturada = (detalleDup.CantidadFacturada ?? 0) + saldoFacturado;
                                        saldoFacturado = 0;
                                    }
                                    modelDetalleDuplicado.CantidadProducto = cantFacturada;
                                    modelPedidoDetalle.Duplicados.Add(modelDetalleDuplicado);
                                    AddBulkUpdate(contextDup, detalleDup);
                                }
                                else if (flujoAuto)
                                {
                                    auxDup = detalleDup;
                                    continue;
                                }
                            }
                            else
                            {
                                var saldoLinea = detalleDup.CantidadPedida - (detalleDup.CantidadAnulada ?? 0);

                                if (saldoLinea > 0)
                                {
                                    decimal cantFacturada = 0;
                                    if (saldoFacturado >= saldoLinea)
                                    {
                                        cantFacturada = saldoLinea;
                                        detalleDup.CantidadPedida = detalleDup.CantidadPedida - saldoLinea;
                                        saldoFacturado -= saldoLinea;
                                    }
                                    else
                                    {
                                        cantFacturada = saldoFacturado;
                                        detalleDup.CantidadPedida = detalleDup.CantidadPedida - saldoFacturado;
                                        saldoFacturado = 0;
                                    }

                                    AddBulkUpdate(contextDup, detalleDup);

                                    if (auxDup != null)
                                    {
                                        auxDup.CantidadPedida = auxDup.CantidadPedida + cantFacturada;
                                        auxDup.CantidadFacturada = (auxDup.CantidadFacturada ?? 0) + cantFacturada;
                                        AddBulkUpdate(contextDup, auxDup);
                                    }
                                    else
                                    {
                                        var newDuplicado = new DetallePedidoDuplicado()
                                        {
                                            Pedido = detalle.Id,
                                            Empresa = detalle.Empresa,
                                            Cliente = detalle.Cliente,
                                            Producto = detalle.Producto,
                                            Identificador = detalle.Identificador,
                                            Faixa = detalle.Faixa,
                                            IdEspecificaIdentificador = detalle.EspecificaIdentificadorId,
                                            IdLineaSistemaExterno = detalleDup.IdLineaSistemaExterno,
                                            TipoLinea = detalleDup.TipoLinea,
                                            CantidadAnulada = null,
                                            CantidadPedida = cantFacturada,
                                            CantidadExpedida = null,
                                            CantidadFacturada = cantFacturada,
                                            DatosSerializados = detalleDup.DatosSerializados,
                                            Transaccion = detalleDup.Transaccion,
                                        };

                                        contextDup.NewDuplicados.Add(GetDuplicadosEntity(newDuplicado));
                                    }
                                    modelDetalleDuplicado.CantidadProducto = cantFacturada;
                                    modelPedidoDetalle.Duplicados.Add(modelDetalleDuplicado);
                                }
                            }
                        }
                        else break;
                    }
                    modelPedido.Detalles.Add(modelPedidoDetalle);
                }

                context.Pedidos.Add(GetPedidoEntity(pedido, nuEjecucion));
                model.Pedidos.Add(modelPedido);
            }
            #endregion

            #region Contenedores

            var contenedores = GetContenedoresCamion(cdcamion, empresa, connection, tran, true);

            foreach (var contenedor in contenedores)
            {
                var detallesPicking = GetLineasPicking(contenedor.Numero, contenedor.NumeroPreparacion, empresa, connection, tran);

                if (detallesPicking.Count == 0)
                    continue; //Si no hay lineas en el contenedor para la empresa, omito el contenedor

                var modelContenedor = new ContenedorResponse()
                {
                    Preparacion = contenedor.NumeroPreparacion,
                    NumeroContenedor = contenedor.Numero,
                    IdExternoContenedor = contenedor.IdExterno,
                    TipoContenedor = contenedor.TipoContenedor,
                    SubClase = contenedor.CodigoSubClase,
                    FechaExpedicion = contenedor.FechaExpedido?.ToString(CDateFormats.DATE_ONLY),
                    PesoReal = contenedor.PesoReal,
                    Altura = contenedor.Altura,
                    Largo = contenedor.Largo,
                    Volumen = contenedor.ValorCubagem,
                    Profundidad = contenedor.Profundidad,
                    CodigoUnidadBulto = contenedor.CodigoUnidadBulto,
                    CantidadBultos = contenedor.CantidadBulto,
                    DescripcionContenedor = contenedor.DescripcionContenedor,
                    Precinto1 = contenedor.Precinto1,
                    Precinto2 = contenedor.Precinto2,
                    CodigoBarras = contenedor.CodigoBarras,
                    NroLpn = contenedor.NroLpn
                };

                foreach (var detalle in detallesPicking)
                {
                    var agente = agenteRepository.GetAgenteOrNull(empresa, detalle.Cliente).Result;

                    modelContenedor.Detalles.Add(new ContenedorDetalleResponse()
                    {
                        Pedido = detalle.Pedido,
                        CodigoAgente = agente?.Codigo,
                        TipoAgente = agente?.Tipo,
                        Producto = detalle.Producto,
                        Identificador = detalle.Lote,
                        CantidadPreparada = detalle.CantidadPreparada ?? 0,
                        FechaVencimientoPickeo = detalle.VencimientoPickeo?.ToString(CDateFormats.DATE_ONLY),
                        AveriaPickeo = detalle.AveriaPickeo,
                    });
                }

                model.Contenedores.Add(modelContenedor);

                if (contenedor.NroLpn != null)
                {
                    var lpn = lpnRepository.GetLpn(contenedor.NroLpn.Value);
                    var atributosCabezal = lpnRepository.GetAllLpnAtributo(contenedor.NroLpn.Value);
                    var atributosDetalle = lpnRepository.GetAllLpnDetalleAtributo(contenedor.NroLpn.Value);

                    model.Lpns.Add(Map(lpn, atributosCabezal, atributosDetalle, expedicion: false));
                }
            }

            #endregion

            return model;
        }

        protected virtual LpnSalidaResponse Map(Lpn lpn, List<LpnAtributo> atributosCabezal, List<LpnDetalleAtributo> atributosDetalle, bool expedicion)
        {
            var detalleAtributos = new Dictionary<string, List<LpnDetalleAtributo>>();

            foreach (var atributo in atributosDetalle)
            {
                var lpnDetKey = $"{atributo.IdLpnDetalle}.{atributo.NumeroLpn}.{atributo.Producto}.{atributo.Faixa.ToString("#.###")}.{atributo.Empresa}.{atributo.Lote}";

                if (!detalleAtributos.ContainsKey(lpnDetKey))
                    detalleAtributos[lpnDetKey] = new List<LpnDetalleAtributo>();

                detalleAtributos[lpnDetKey].Add(atributo);
            }

            return new LpnSalidaResponse
            {
                Atributos = atributosCabezal.Select(a => Map(a)).ToList(),
                Detalles = lpn.Detalles.Select(d => Map(d, detalleAtributos, expedicion)).ToList(),
                Empresa = lpn.Empresa,
                IdExterno = lpn.IdExterno,
                IdPacking = lpn.IdPacking,
                Numero = lpn.NumeroLPN,
                Tipo = lpn.Tipo,
            };
        }

        protected virtual LpnSalidaDetalleResponse Map(LpnDetalle detalle, Dictionary<string, List<LpnDetalleAtributo>> detalleAtributos, bool expedicion)
        {
            var lpnDetKey = $"{detalle.Id}.{detalle.NumeroLPN}.{detalle.CodigoProducto}.{detalle.Faixa.ToString("#.###")}.{detalle.Empresa}.{detalle.Lote}";
            var atributos = new List<AtributoResponse>();

            if (detalleAtributos.ContainsKey(lpnDetKey))
                atributos.AddRange(detalleAtributos[lpnDetKey].Select(a => Map(a)));

            return new LpnSalidaDetalleResponse
            {
                Atributos = atributos,
                Cantidad = expedicion ? (detalle.CantidadExpedida ?? 0) : detalle.Cantidad,
                CodigoProducto = detalle.CodigoProducto,
                Faixa = detalle.Faixa,
                Id = detalle.Id,
                IdLineaSistemaExterno = detalle.IdLineaSistemaExterno,
                Lote = detalle.Lote,
                Vencimiento = detalle.Vencimiento?.ToString(CDateFormats.DATE_ONLY),
            };
        }

        protected virtual AtributoResponse Map(LpnAtributo atributo)
        {
            return new AtributoResponse
            {
                Nombre = atributo.Nombre,
                Valor = atributo.Valor,
            };
        }

        protected virtual AtributoResponse Map(LpnDetalleAtributo atributo)
        {
            return new AtributoResponse
            {
                Nombre = atributo.NombreAtributo,
                Valor = atributo.ValorAtributo,
            };
        }

        public virtual async Task UpdatePedidos(BulkUpdatePedidosContext context, DbConnection connection, DbTransaction tran)
        {
            await BulkUpdatePedidos(connection, context.Pedidos, tran);
        }

        public virtual async Task BulkUpdatePedidos(DbConnection connection, List<object> pedidos, DbTransaction tran)
        {
            string sql = @"UPDATE T_PEDIDO_SAIDA 
                SET NU_INTERFAZ_FACTURACION = :nroEjecucion,
                    NU_TRANSACCION = :transaccion,
                    DT_UPDROW = :Updrow
                WHERE NU_PEDIDO = :nuPedido 
                    AND CD_CLIENTE = :cdCliente
                    AND CD_EMPRESA = :empresa ";

            await _dapper.ExecuteAsync(connection, sql, pedidos, transaction: tran);
        }

        public virtual async Task UpdateEntregas(BulkUpdateEntregasContext context, DbConnection connection, DbTransaction tran)
        {
            await BulkUpdateEntregas(connection, context.Entregas, tran);
        }

        public virtual async Task BulkUpdateEntregas(DbConnection connection, List<object> entregas, DbTransaction tran)
        {
            string sql = @"UPDATE T_ENTREGA 
                SET NU_INTERFAZ_EJECUCION = :nroEjecucion
                WHERE NU_ENTREGA = :nuEntrega 
                    AND TP_ENTREGA = :tpEntrega";

            await _dapper.ExecuteAsync(connection, sql, entregas, transaction: tran);
        }

        public virtual async Task UpdateDuplicados(BulkUpdateDuplicadosContext context, DbConnection connection, DbTransaction tran, bool facturacion = false)
        {
            await BulkUpdateDuplicados(connection, context.UpdateDuplicados.Values, tran, facturacion);
        }

        public virtual async Task InsertDuplicados(BulkUpdateDuplicadosContext context, DbConnection connection, DbTransaction tran, bool facturacion = false)
        {
            await BulkInsertNewDuplicados(connection, context.NewDuplicados, tran, facturacion);
        }

        public virtual async Task BulkUpdateDuplicados(DbConnection connection, IEnumerable<object> duplicados, DbTransaction tran, bool facturacion = false)
        {
            string set = "QT_EXPEDIDO = :CantidadExpedida";
            if (facturacion)
                set = "QT_FACTURADO = :CantidadFacturada";

            string sql = @"UPDATE T_DET_PEDIDO_SAIDA_DUP SET " + set + @" , QT_PEDIDO = :CantidadPedida, NU_TRANSACCION = :Transaccion, DT_UPDROW = :FechaModificacion
                           WHERE NU_PEDIDO = :nuPedido AND CD_CLIENTE = :cliente AND CD_EMPRESA = :empresa
                           AND CD_PRODUTO = :producto AND CD_FAIXA = :faixa AND NU_IDENTIFICADOR =:identificador AND ID_ESPECIFICA_IDENTIFICADOR = :especificaLote
                           AND ID_LINEA_SISTEMA_EXTERNO = :idLineaSistemaExterno ";

            await _dapper.ExecuteAsync(connection, sql, duplicados, transaction: tran);
        }

        public virtual async Task BulkInsertNewDuplicados(DbConnection connection, List<object> duplicados, DbTransaction tran, bool facturacion = false)
        {
            string field = "QT_EXPEDIDO";
            string set = ":CantidadExpedida";

            if (facturacion)
            {
                field = "QT_FACTURADO";
                set = ":CantidadFacturada";
            }

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
                        QT_PEDIDO," + field +
                        @",DT_ADDROW,
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
                        :CantidadPedida, " + set +
                        @", :FechaAlta,
                        :FechaModificacion,
                        :Transaccion,
                        :DatosSerializados)";

            await _dapper.ExecuteAsync(connection, sql, duplicados, transaction: tran);
        }

        public virtual async Task InsertCamionEjecucion(BulkCamionEjecucionContext context, DbConnection connection, DbTransaction tran)
        {
            await BulkInsertCamionEjecucion(connection, context.Datos, tran);
        }

        public virtual async Task BulkInsertCamionEjecucion(DbConnection connection, List<object> datos, DbTransaction tran)
        {
            string sql = @"INSERT INTO T_CAMION_INTERFAZ_EJECUCION (CD_CAMION, NU_INTERFAZ_EJECUCION,FL_FACTURACION, DT_ADDROW) 
                         VALUES (:cdCamion, :nuEjecucion, :facturacion, :FechaAlta)";
            await _dapper.ExecuteAsync(connection, sql, datos, transaction: tran);
        }

        public static object GetPedidoEntity(Pedido pedido, long nuEjecucion)
        {
            return new
            {
                nuPedido = pedido.Id,
                empresa = pedido.Empresa,
                cdCliente = pedido.Cliente,
                nroEjecucion = nuEjecucion,
                Updrow = DateTime.Now,
                transaccion = pedido.Transaccion,
            };
        }

        public static object GetEntregaEntity(Entrega entrega, long nuEjecucion)
        {
            return new
            {
                nuEntrega = entrega.Id,
                tpEntrega = entrega.TipoEntrega,
                nroEjecucion = nuEjecucion,
            };
        }

        public static object GetDuplicadosEntity(DetallePedidoDuplicado dup)
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

        public static object GetCamionEjecucionEntity(int cdCamion, long nuEjecucion, string facturacion)
        {
            return new
            {
                cdCamion = cdCamion,
                nuEjecucion = nuEjecucion,
                facturacion = facturacion,
                FechaAlta = DateTime.Now
            };
        }

        public virtual List<int> GetEmpresasCamion(int cdcamion, DbConnection connection, DbTransaction tran, bool facturacion = false)
        {
            string sql = @"SELECT CD_EMPRESA 
                FROM T_DET_PEDIDO_EXPEDIDO 
                WHERE CD_CAMION = :cdcamion 
                GROUP BY CD_EMPRESA";

            if (facturacion)
            {
                sql = @"SELECT dp.CD_EMPRESA 
                FROM T_CLIENTE_CAMION cc 
                INNER JOIN T_DET_PICKING DP ON cc.CD_CLIENTE = dp.CD_CLIENTE 
                    AND cc.NU_CARGA = dp.NU_CARGA
                INNER JOIN T_CONTENEDOR co ON dp.NU_PREPARACION = co.NU_PREPARACION 
                    AND dp.NU_CONTENEDOR = co.NU_CONTENEDOR
                WHERE cc.CD_CAMION = :cdcamion 
                    AND co.CD_CAMION_FACTURADO = :cdcamion
                GROUP BY dp.CD_EMPRESA";
            }

            return _dapper.Query<int>(connection, sql, param: new
            {
                cdcamion = cdcamion
            }, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<Pedido> GetPedidosCamion(int cdcamion, int empresa, DbConnection connection, DbTransaction tran, bool facturacion = false)
        {
            string sqlFromWhere = @" FROM T_PEDIDO_SAIDA pe 
                        INNER JOIN T_DET_PEDIDO_EXPEDIDO pex ON pe.CD_CLIENTE = pex.CD_CLIENTE 
                            AND pe.NU_PEDIDO = pex.NU_PEDIDO 
                            AND pe.CD_EMPRESA = pex.CD_EMPRESA
                        WHERE pex.CD_CAMION = :cdcamion 
                            AND pex.CD_EMPRESA = :empresa ";

            if (facturacion)
            {
                sqlFromWhere = @" FROM T_CLIENTE_CAMION cc 
                        INNER JOIN T_DET_PICKING dp ON cc.CD_CLIENTE = dp.CD_CLIENTE 
                            AND cc.NU_CARGA = dp.NU_CARGA
                        INNER JOIN T_CONTENEDOR co ON dp.NU_PREPARACION = co.NU_PREPARACION 
                            AND dp.NU_CONTENEDOR = co.NU_CONTENEDOR
                        INNER JOIN T_PEDIDO_SAIDA pe ON dp.NU_PEDIDO = pe.NU_PEDIDO 
                            AND dp.CD_EMPRESA = pe.CD_EMPRESA 
                            AND dp.CD_CLIENTE = pe.CD_CLIENTE
                        WHERE cc.CD_CAMION = :cdcamion 
                            AND co.CD_CAMION_FACTURADO = :cdcamion 
                            AND cc.CD_EMPRESA = :empresa ";
            }

            string sql = @"SELECT 
                        pe.NU_PEDIDO as Id,
                        pe.CD_CLIENTE as Cliente,
                        pe.CD_EMPRESA as Empresa,
                        pe.CD_CONDICION_LIBERACION as CondicionLiberacion,
                        pe.CD_FUN_RESP as FuncionarioResponsable,
                        pe.CD_ORIGEN as Origen,
                        pe.CD_PUNTO_ENTREGA as PuntoEntrega,
                        pe.CD_ROTA as Ruta,
                        pe.CD_SITUACAO as EstadoId,
                        pe.CD_TRANSPORTADORA as CodigoTransportadora,
                        pe.CD_UF as CodigoUF,
                        pe.CD_ZONA as Zona,
                        pe.DS_ANEXO1 as Anexo,
                        pe.DS_ANEXO2 as Anexo2,
                        pe.DS_ANEXO3 as Anexo3,
                        pe.DS_ANEXO4 as Anexo4,
                        pe.DS_ENDERECO as DireccionEntrega,
                        pe.DS_MEMO as Memo,
                        pe.DS_MEMO_1 as Memo1,
                        pe.DT_ADDROW as FechaAlta,
                        pe.DT_EMITIDO as FechaEmision,
                        pe.DT_ENTREGA as FechaEntrega,
                        pe.DT_FUN_RESP as FechaFuncionarioResponsable,
                        pe.DT_GENERICO_1 as FechaGenerica_1,
                        pe.DT_LIBERAR_DESDE as FechaLiberarDesde,
                        pe.DT_LIBERAR_HASTA as FechaLiberarHasta,
                        pe.DT_ULT_PREPARACION as FechaUltimaPreparacion,
                        pe.DT_UPDROW as FechaModificacion,
                        pe.FL_SYNC_REALIZADA as SincronizacionRealizadaId,
                        pe.ID_AGRUPACION as Agrupacion,
                        pe.ID_MANUAL as ManualId,
                        pe.ND_ACTIVIDAD as Actividad,
                        pe.NU_GENERICO_1 as NuGenerico_1,
                        pe.NU_INTERFAZ_FACTURACION as NroIntzFacturacion,
                        pe.NU_ORDEN_ENTREGA as OrdenEntrega,
                        pe.NU_ORDEN_LIBERACION as NumeroOrdenLiberacion,
                        pe.NU_PRDC_INGRESO as IngresoProduccion,
                        pe.NU_PREDIO as Predio,
                        pe.NU_PREPARACION_MANUAL as NroPrepManual,
                        pe.NU_PREPARACION_PROGRAMADA as PreparacionProgramada,
                        pe.NU_TRANSACCION as Transaccion,
                        pe.NU_ULT_PREPARACION as NumeroUltimaPreparacion,
                        pe.TP_EXPEDICION as TipoExpedicionId,
                        pe.TP_PEDIDO as Tipo,
                        pe.VL_COMPARTE_CONTENEDOR_ENTREGA as ComparteContenedorEntrega,
                        pe.VL_COMPARTE_CONTENEDOR_PICKING as ComparteContenedorPicking,
                        pe.VL_GENERICO_1 as VlGenerico_1,
                        pe.VL_SERIALIZADO_1 as VlSerealizado_1 " +
                sqlFromWhere +
                @" GROUP BY pe.CD_CLIENTE,
                        pe.CD_CONDICION_LIBERACION,
                        pe.CD_EMPRESA,
                        pe.CD_FUN_RESP,
                        pe.CD_ORIGEN,
                        pe.CD_PUNTO_ENTREGA,
                        pe.CD_ROTA,
                        pe.CD_SITUACAO,
                        pe.CD_TRANSPORTADORA,
                        pe.CD_UF,
                        pe.CD_ZONA,
                        pe.DS_ANEXO1,
                        pe.DS_ANEXO2,
                        pe.DS_ANEXO3,
                        pe.DS_ANEXO4,
                        pe.DS_ENDERECO,
                        pe.DS_MEMO,
                        pe.DS_MEMO_1,
                        pe.DT_ADDROW,
                        pe.DT_EMITIDO,
                        pe.DT_ENTREGA,
                        pe.DT_FUN_RESP,
                        pe.DT_GENERICO_1,
                        pe.DT_LIBERAR_DESDE,
                        pe.DT_LIBERAR_HASTA,
                        pe.DT_ULT_PREPARACION,
                        pe.DT_UPDROW,
                        pe.FL_SYNC_REALIZADA,
                        pe.ID_AGRUPACION,
                        pe.ID_MANUAL,
                        pe.ND_ACTIVIDAD,
                        pe.NU_GENERICO_1,
                        pe.NU_INTERFAZ_FACTURACION,
                        pe.NU_ORDEN_ENTREGA,
                        pe.NU_ORDEN_LIBERACION,
                        pe.NU_PEDIDO,
                        pe.NU_PRDC_INGRESO,
                        pe.NU_PREDIO,
                        pe.NU_PREPARACION_MANUAL,
                        pe.NU_PREPARACION_PROGRAMADA,
                        pe.NU_TRANSACCION,
                        pe.NU_ULT_PREPARACION,
                        pe.TP_EXPEDICION,
                        pe.TP_PEDIDO,
                        pe.VL_COMPARTE_CONTENEDOR_ENTREGA,
                        pe.VL_COMPARTE_CONTENEDOR_PICKING,
                        pe.VL_GENERICO_1,
                        pe.VL_SERIALIZADO_1 
                ORDER BY pe.NU_PEDIDO";

            return _dapper.Query<Pedido>(connection, sql, param: new
            {
                cdcamion = cdcamion,
                empresa = empresa
            }, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<DetallePedido> GetDetallesPedidoFacturacion(int cdcamion, Pedido pedido, DbConnection connection, DbTransaction tran)
        {
            var param = new DynamicParameters(new
            {
                cdcamion = cdcamion,
                empresa = pedido.Empresa,
                cliente = pedido.Cliente,
                nuPedido = pedido.Id
            });

            string sql = @"SELECT 
                        pe.NU_PEDIDO as Id,
                        pe.CD_CLIENTE as Cliente,
                        pe.CD_EMPRESA as Empresa,
                        pe.CD_FAIXA as Faixa,
                        pe.CD_PRODUTO as Producto,
                        pe.DS_MEMO as Memo,
                        pe.DT_ADDROW as FechaAlta,
                        pe.DT_GENERICO_1 as FechaGenerica_1,
                        pe.DT_UPDROW as FechaModificacion,
                        pe.ID_AGRUPACION as Agrupacion,
                        pe.ID_ESPECIFICA_IDENTIFICADOR as EspecificaIdentificadorId,
                        pe.NU_GENERICO_1 as NuGenerico_1,
                        pe.NU_IDENTIFICADOR as Identificador,
                        pe.NU_TRANSACCION as Transaccion,
                        pe.QT_ABASTECIDO as CantidadAbastecida,
                        pe.QT_ANULADO as CantidadAnulada,
                        pe.QT_ANULADO_FACTURA as CantidadAnuladaFactura,
                        pe.QT_CARGADO as CantidadCargada,
                        pe.QT_CONTROLADO as CantidadControlada,
                        pe.QT_CROSS_DOCK as CantidadCrossDocking,
                        pe.QT_EXPEDIDO as CantidadExpedida,
                        pe.QT_FACTURADO as CantidadFacturada,
                        pe.QT_LIBERADO as CantidadLiberada,
                        pe.QT_PEDIDO as Cantidad,
                        pe.QT_PEDIDO_ORIGINAL as CantidadOriginal,
                        pe.QT_PREPARADO as CantidadPreparada,
                        pe.QT_TRANSFERIDO as CantidadTransferida,
                        pe.QT_UND_ASOCIADO_CAMION as CantUndAsociadoCamion,
                        pe.VL_GENERICO_1 as VlGenerico_1,
                        pe.VL_PORCENTAJE_TOLERANCIA as PorcentajeTolerancia,
                        pe.VL_SERIALIZADO_1 as DatosSerializados 
                    FROM T_CLIENTE_CAMION cc 
                    INNER JOIN T_DET_PICKING dp ON cc.CD_CLIENTE = dp.CD_CLIENTE 
                        AND cc.NU_CARGA = dp.NU_CARGA
                    INNER JOIN T_CONTENEDOR co ON dp.NU_PREPARACION = co.NU_PREPARACION 
                        AND dp.NU_CONTENEDOR = co.NU_CONTENEDOR
                    INNER JOIN T_DET_PEDIDO_SAIDA pe ON dp.NU_PEDIDO = pe.NU_PEDIDO 
                        AND dp.CD_EMPRESA = pe.CD_EMPRESA 
                        AND dp.CD_CLIENTE = pe.CD_CLIENTE
                        AND dp.cd_produto = pe.cd_produto 
                        AND dp.cd_faixa = pe.cd_faixa 
                        AND dp.nu_identificador = pe.nu_identificador 
                        AND dp.id_especifica_identificador = pe.id_especifica_identificador
                    WHERE cc.CD_CAMION = :cdcamion 
                        AND co.CD_CAMION_FACTURADO = :cdcamion 
                        AND cc.CD_EMPRESA = :empresa 
                        AND pe.NU_PEDIDO = :nuPedido 
                        AND pe.CD_CLIENTE = :cliente 
                    GROUP BY
                        pe.CD_CLIENTE,
                        pe.CD_EMPRESA,
                        pe.CD_FAIXA,
                        pe.CD_PRODUTO,
                        pe.DS_MEMO,
                        pe.DT_ADDROW,
                        pe.DT_GENERICO_1,
                        pe.DT_UPDROW,
                        pe.ID_AGRUPACION,
                        pe.ID_ESPECIFICA_IDENTIFICADOR,
                        pe.NU_GENERICO_1,
                        pe.NU_IDENTIFICADOR,
                        pe.NU_PEDIDO,
                        pe.NU_TRANSACCION,
                        pe.QT_ABASTECIDO,
                        pe.QT_ANULADO,
                        pe.QT_ANULADO_FACTURA,
                        pe.QT_CARGADO,
                        pe.QT_CONTROLADO,
                        pe.QT_CROSS_DOCK,
                        pe.QT_EXPEDIDO,
                        pe.QT_FACTURADO,
                        pe.QT_LIBERADO,
                        pe.QT_PEDIDO,
                        pe.QT_PEDIDO_ORIGINAL,
                        pe.QT_PREPARADO,
                        pe.QT_TRANSFERIDO,
                        pe.QT_UND_ASOCIADO_CAMION,
                        pe.VL_GENERICO_1,
                        pe.VL_PORCENTAJE_TOLERANCIA,
                        pe.VL_SERIALIZADO_1";

            return _dapper.Query<DetallePedido>(connection, sql, param: param, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual DTOPikcingCantidades GetCantidadesPicking(int cdcamion, DetallePedido detallePedido, DbConnection connection, DbTransaction tran)
        {
            var param = new DynamicParameters(new
            {
                cdcamion = cdcamion,
                empresa = detallePedido.Empresa,
                cliente = detallePedido.Cliente,
                nuPedido = detallePedido.Id,
                producto = detallePedido.Producto,
                faixa = detallePedido.Faixa,
                identificador = detallePedido.Identificador
            });

            string sql = @"SELECT 
                            SUM(dp.qt_produto) as CantidadProducto,
                            SUM(COALESCE(dp.qt_preparado,0)) as CantidadPreparada
                        FROM T_DET_PICKING dp 
                        INNER JOIN T_CONTENEDOR co ON dp.NU_PREPARACION = co.NU_PREPARACION 
                            AND dp.NU_CONTENEDOR = co.NU_CONTENEDOR
                        WHERE co.CD_CAMION_FACTURADO = :cdcamion 
                            AND dp.CD_EMPRESA = :empresa 
                            AND dp.NU_PEDIDO = :nuPedido 
                            AND dp.CD_CLIENTE = :cliente 
                            AND dp.CD_PRODUTO = :producto
                            AND dp.NU_IDENTIFICADOR= :identificador 
                            AND dp.CD_FAIXA = :faixa
                        GROUP BY dp.NU_PEDIDO";

            return _dapper.Query<DTOPikcingCantidades>(connection, sql, param: param, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual List<DetallePedidoExpedido> GetDetallesPedidoExpedido(int cdcamion, Pedido pedido, DbConnection connection, DbTransaction tran)
        {
            var param = new DynamicParameters(new
            {
                cdcamion = cdcamion,
                empresa = pedido.Empresa,
                cliente = pedido.Cliente,
                nuPedido = pedido.Id
            });

            string sql = @"SELECT 
                            pex.CD_CAMION as Camion,
                            pex.NU_PEDIDO as Pedido,
                            pex.CD_CLIENTE as Cliente,
                            pex.CD_EMPRESA as Empresa,
                            pex.CD_PRODUTO as Producto,
                            pex.NU_IDENTIFICADOR as Identificador,
                            pex.CD_FAIXA as Faixa,
                            pex.QT_PRODUTO as Cantidad,
                            pex.DT_EXPEDICION as FechaExpedicion,
                            pex.ID_ESPECIFICA_IDENTIFICADOR as EspecificaLoteId,
                            MIN(dp.DS_MEMO) as Memo,
                            MIN(dp.VL_SERIALIZADO_1) as Serializado
                        FROM T_DET_PEDIDO_EXPEDIDO pex 
                        INNER JOIN T_DET_PEDIDO_SAIDA dp ON pex.NU_PEDIDO = dp.NU_PEDIDO 
                            AND pex.CD_EMPRESA = dp.CD_EMPRESA 
                            AND pex.CD_CLIENTE = dp.CD_CLIENTE 
                            AND pex.CD_PRODUTO = dp.CD_PRODUTO 
                            AND pex.CD_FAIXA = dp.CD_FAIXA 
                            AND pex.NU_IDENTIFICADOR = dp.NU_IDENTIFICADOR 
                            AND pex.ID_ESPECIFICA_IDENTIFICADOR = dp.ID_ESPECIFICA_IDENTIFICADOR
                        WHERE pex.CD_CAMION = :cdcamion AND pex.CD_EMPRESA = :empresa 
                            AND pex.NU_PEDIDO = :nuPedido 
                            AND pex.CD_CLIENTE = :cliente 
                        GROUP BY
                            pex.CD_CAMION,
                            pex.NU_PEDIDO,
                            pex.CD_CLIENTE,
                            pex.CD_EMPRESA,
                            pex.CD_PRODUTO,
                            pex.NU_IDENTIFICADOR,
                            pex.CD_FAIXA,
                            pex.QT_PRODUTO,
                            pex.DT_EXPEDICION,
                            pex.ID_ESPECIFICA_IDENTIFICADOR";

            return _dapper.Query<DetallePedidoExpedido>(connection, sql, param: param, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<DetallePedidoDuplicado> FiltroDuplicados(List<DetallePedidoDuplicado> dups, DetallePedidoExpedido detExpedido, bool flujoAuto)
        {
            var result = new List<DetallePedidoDuplicado>();

            result = dups.Where(d => d.Pedido == detExpedido.Pedido && d.Cliente == detExpedido.Cliente && d.Empresa == detExpedido.Empresa && d.Producto == detExpedido.Producto
                     && d.Faixa == detExpedido.Faixa && d.Identificador == detExpedido.Identificador && d.IdEspecificaIdentificador == detExpedido.EspecificaLoteId)
                     .OrderBy(d => d.IdLineaSistemaExterno).ToList();

            if (result.Count == 0 || flujoAuto)
            {
                var auto = dups.Where(d => d.Pedido == detExpedido.Pedido && d.Cliente == detExpedido.Cliente && d.Empresa == detExpedido.Empresa && d.Producto == detExpedido.Producto
                      && d.Faixa == detExpedido.Faixa && d.Identificador == ManejoIdentificadorDb.IdentificadorAuto && d.IdEspecificaIdentificador == detExpedido.EspecificaLoteId)
                     .OrderBy(d => d.IdLineaSistemaExterno).ToList();

                if (flujoAuto)

                    result.AddRange(auto);
                else
                    result = auto;
            }

            return result;
        }

        public virtual List<DetallePedidoDuplicado> FiltroDuplicados(List<DetallePedidoDuplicado> dups, DetallePedido detalle, bool flujoAuto)
        {
            var result = new List<DetallePedidoDuplicado>();

            result = dups.Where(d => d.Pedido == detalle.Id && d.Cliente == detalle.Cliente && d.Empresa == detalle.Empresa && d.Producto == detalle.Producto
                     && d.Faixa == detalle.Faixa && d.Identificador == detalle.Identificador && d.IdEspecificaIdentificador == detalle.EspecificaIdentificadorId)
                     .OrderBy(d => d.IdLineaSistemaExterno).ToList();

            if (result.Count == 0 || flujoAuto)
            {
                var auto = dups.Where(d => d.Pedido == detalle.Id && d.Cliente == detalle.Cliente && d.Empresa == detalle.Empresa && d.Producto == detalle.Producto
                      && d.Faixa == detalle.Faixa && d.Identificador == ManejoIdentificadorDb.IdentificadorAuto && d.IdEspecificaIdentificador == detalle.EspecificaIdentificadorId)
                     .OrderBy(d => d.IdLineaSistemaExterno).ToList();

                if (flujoAuto)
                    result.AddRange(auto);
                else
                    result = auto;
            }

            return result;
        }

        public virtual List<Contenedor> GetContenedoresCamion(int cdcamion, int empresa, DbConnection connection, DbTransaction tran, bool facturacion = false)
        {
            string sql = GetSqlSelectContenedor() +
                @"FROM T_CONTENEDOR co 
                WHERE co.CD_CAMION = :cdcamion";

            if (facturacion)
            {
                sql = GetSqlSelectContenedor() +
                      @"FROM T_CLIENTE_CAMION cc 
                      INNER JOIN T_DET_PICKING dp ON cc.CD_CLIENTE = dp.CD_CLIENTE 
                        AND cc.NU_CARGA = dp.NU_CARGA
                      INNER JOIN T_CONTENEDOR co ON dp.NU_PREPARACION = co.NU_PREPARACION 
                        AND dp.NU_CONTENEDOR = co.NU_CONTENEDOR
                      WHERE cc.CD_CAMION = :cdcamion 
                        AND co.CD_CAMION_FACTURADO = :cdcamion 
                        AND dp.CD_EMPRESA = :empresa
                      GROUP BY
                          co.CD_AGRUPADOR,
                          co.CD_CAMION,
                          co.CD_CAMION_CONGELADO,
                          co.CD_CAMION_FACTURADO,
                          co.CD_CANAL,
                          co.CD_ENDERECO,
                          co.CD_FUNCIONARIO_EXPEDICION,
                          co.CD_PORTA,
                          co.CD_SITUACAO,
                          co.CD_SUB_CLASSE,
                          co.CD_UNIDAD_BULTO,
                          co.DS_CONTENEDOR,
                          co.DT_ADDROW,
                          co.DT_EXPEDIDO,
                          co.DT_PULMON,
                          co.DT_UPDROW,
                          co.FL_HABILITADO,
                          co.FL_SEPARADO_DOS_FASES,
                          co.ID_CONTENEDOR_EMPAQUE,
                          co.ID_PRECINTO_1,
                          co.ID_PRECINTO_2,
                          co.NU_CONTENEDOR,
                          co.NU_PREPARACION,
                          co.NU_TRANSACCION,
                          co.NU_UNIDAD_TRANSPORTE,
                          co.NU_VIAJE,
                          co.PS_REAL,
                          co.QT_BULTO,
                          co.TP_CONTENEDOR,
                          co.TP_CONTROL,
                          co.VL_ALTURA,
                          co.VL_CONTROL,
                          co.VL_CUBAGEM,
                          co.VL_LARGURA,
                          co.VL_PROFUNDIDADE,
                          co.NU_TRANSACCION_DELETE,                          
                          co.ID_EXTERNO_CONTENEDOR,
                          co.CD_BARRAS,
                          co.NU_LPN,
                          co.ID_EXTERNO_TRACKING ";
            }

            return _dapper.Query<Contenedor>(connection, sql, param: new
            {
                cdcamion = cdcamion,
                empresa = empresa
            }, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public static string GetSqlSelectContenedor()
        {
            return @"SELECT 
                          co.NU_CONTENEDOR as Numero,
                          co.NU_PREPARACION as NumeroPreparacion,
                          co.TP_CONTENEDOR as TipoContenedor,
                          co.CD_CAMION as CodigoCamion,
                          co.CD_AGRUPADOR as CodigoAgrupador,
                          co.CD_CAMION_CONGELADO as CodigoCamionCongelado,
                          co.CD_CAMION_FACTURADO as CamionFacturado,
                          co.CD_CANAL as CodigoCanal,
                          co.CD_ENDERECO as Ubicacion,
                          co.CD_FUNCIONARIO_EXPEDICION as CodigoFuncionarioExpedicion,
                          co.CD_PORTA as CodigoPuerta,
                          co.CD_SITUACAO as EstadoId,
                          co.CD_SUB_CLASSE as CodigoSubClase,
                          co.CD_UNIDAD_BULTO as CodigoUnidadBulto,
                          co.DS_CONTENEDOR as DescripcionContenedor,
                          co.DT_ADDROW as FechaAgregado,
                          co.DT_EXPEDIDO as FechaExpedido,
                          co.DT_PULMON as FechaPulmon,
                          co.DT_UPDROW as FechaModificado,
                          co.FL_HABILITADO as Habilitado,
                          co.FL_SEPARADO_DOS_FASES as SegundaFase,
                          co.ID_CONTENEDOR_EMPAQUE as IdContenedorEmpaque,
                          co.ID_PRECINTO_1 as Precinto1,
                          co.ID_PRECINTO_2 as Precinto2,
                          co.NU_TRANSACCION as NumeroTransaccion,
                          co.NU_UNIDAD_TRANSPORTE as NumeroUnidadTransporte,
                          co.NU_VIAJE as NumeroViaje,
                          co.PS_REAL as PesoReal,
                          co.QT_BULTO as CantidadBulto,
                          co.TP_CONTROL as TipoControl,
                          co.VL_ALTURA as Altura,
                          co.VL_CONTROL as ValorControl,
                          co.VL_CUBAGEM as ValorCubagem,
                          co.VL_LARGURA as Largo,
                          co.VL_PROFUNDIDADE as Profundidad,
                          co.NU_TRANSACCION_DELETE as NumeroTransaccionDelete,                          
                          co.ID_EXTERNO_CONTENEDOR as IdExterno,
                          co.CD_BARRAS as CodigoBarras,
                          co.NU_LPN as NroLpn,
                          co.ID_EXTERNO_TRACKING as IdExternoTracking ";
        }

        public virtual List<DetallePreparacion> GetLineasPicking(int contenedor, int preparacion, int empresa, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            NU_PREPARACION as NumeroPreparacion,
                            CD_CLIENTE as Cliente,
                            CD_EMPRESA as Empresa,
                            CD_ENDERECO as Ubicacion,
                            CD_FAIXA as Faixa,
                            CD_FORNECEDOR as Proveedor,
                            CD_FUNCIONARIO as Usuario,
                            CD_FUNC_PICKEO as UsuarioPickeo,
                            CD_PRODUTO as Producto,
                            DT_ADDROW as FechaAlta,
                            DT_FABRICACAO_PICKEO as VencimientoPickeo,
                            DT_PICKEO as FechaPickeo,
                            DT_SEPARACION as FechaSeparacion,
                            DT_UPDROW as FechaModificacion,
                            FL_CANCELADO as CanceladoId,
                            FL_ERROR_CONTROL as ErrorControl,
                            ID_AGRUPACION as EstadoId,
                            ID_AREA_AVERIA as AreaAveria,
                            ID_AVERIA_PICKEO as AveriaPickeo,
                            ID_ESPECIFICA_IDENTIFICADOR as EspecificaLote,
                            ND_ESTADO as Estado,
                            NU_CARGA as Carga,
                            NU_CONTENEDOR as NroContenedor,
                            NU_CONTENEDOR_PICKEO as NumeroContenedorPickeo,
                            NU_CONTENEDOR_SYS as NumeroContenedorSys,
                            NU_IDENTIFICADOR as Lote,
                            NU_PEDIDO as Pedido,
                            NU_SEQ_PREPARACION as NumeroSecuencia,
                            NU_TRANSACCION as Transaccion,
                            QT_CONTROL as CantidadControl,
                            QT_CONTROLADO as CantidadControlada,
                            QT_PICKEO as CantidadPickeo,
                            QT_PREPARADO as CantidadPreparada,
                            QT_PRODUTO as Cantidad,
                            VL_ESTADO_REFERENCIA as ReferenciaEstado,
                            NU_TRANSACCION_DELETE as TransaccionDelete,
                            ID_DET_PICKING_LPN as IdDetallePickingLpn
                        FROM T_DET_PICKING 
                        WHERE NU_PREPARACION = :preparacion 
                            AND NU_CONTENEDOR = :contenedor 
                            AND CD_EMPRESA= :empresa";

            return _dapper.Query<DetallePreparacion>(connection, sql, param: new
            {
                contenedor = contenedor,
                preparacion = preparacion,
                empresa = empresa
            }, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual async Task<InterfazEjecucion> CrearEjecucion(int camion, int cdEmpresa, string grupoConsulta, int intExterna, DbConnection connection, DbTransaction tran, string dsReferencia)
        {
            var ejecucionRepository = new EjecucionRepository(_dapper);

            var interfaz = new InterfazEjecucion
            {
                CdInterfazExterna = intExterna,
                Situacion = SituacionDb.ProcesadoPendiente,
                Comienzo = DateTime.Now,
                FechaSituacion = DateTime.Now,
                ErrorCarga = "N",
                ErrorProcedimiento = "N",
                Referencia = dsReferencia,
                Empresa = cdEmpresa,
                GrupoConsulta = grupoConsulta
            };

            return await ejecucionRepository.AddEjecucion(interfaz, connection, tran);
        }

        public virtual async Task CrearEjecucionData(long nuEjecucion, object camion, int empresa, DbConnection connection, DbTransaction tran)
        {
            var ejecucionRepository = new EjecucionRepository(_dapper);
            var data = JsonConvert.SerializeObject(camion);
            var itfzData = new InterfazData
            {
                Id = nuEjecucion,
                Alta = DateTime.Now,
                Data = Encoding.UTF8.GetBytes(data)
            };
            await ejecucionRepository.AddEjecucionData(itfzData, connection, tran);
        }

        public virtual async Task UpdateCamion(int cdCamion, long nroEjecucion, long nuTransaccion, DbConnection connection, DbTransaction tran, bool facturacion = false)
        {
            var param = new DynamicParameters(new
            {
                cdCamion = cdCamion,
                nroEjecucion = nroEjecucion,
                Updrow = DateTime.Now,
                nuTransaccion = nuTransaccion
            });

            string nroEjecucionField = "NU_INTERFAZ_EJECUCION";
            if (facturacion)
                nroEjecucionField = "NU_INTERFAZ_EJECUCION_FACT";

            string sql = @"UPDATE T_CAMION 
                SET " + nroEjecucionField + @" = :nroEjecucion, 
                    DT_UPDROW = :Updrow, 
                    NU_TRANSACCION = :nuTransaccion
                WHERE CD_CAMION = :cdCamion";

            await _dapper.ExecuteAsync(connection, sql, param, transaction: tran);
        }

        public virtual async Task FinalizarEmpaquetado(int cdCamion, long nuTransaccion, DbConnection connection, DbTransaction tran)
        {
            var param = new DynamicParameters(new
            {
                cdCamion = cdCamion,
                situacion = SituacionDb.CamionCerrado,
                fechaModificacion = DateTime.Now,
                nuTransaccion = nuTransaccion
            });

            var sql = @$"DELETE FROM T_CLIENTE_CAMION WHERE CD_CAMION = :cdCamion ";

            await _dapper.ExecuteAsync(connection, sql, param, transaction: tran);

            sql = @"UPDATE T_CAMION 
                    SET CD_SITUACAO = :situacion, 
                        DT_UPDROW = :fechaModificacion, 
                        NU_TRANSACCION = :nuTransaccion
                    WHERE CD_CAMION = :cdCamion";

            await _dapper.ExecuteAsync(connection, sql, param, transaction: tran);
        }

        public virtual List<Entrega> GetEntregas(int cdcamion, int empresa, DbConnection connection, DbTransaction tran, bool facturacion = false)
        {
            string sql = @"SELECT 
                            CD_AGENCIA as Agencia,
                            CD_BARRAS as CodigoBarras,
                            CD_CAMION as Camion,
                            CD_CAMION_FACTURADO as CamionFacturado,
                            CD_CLIENTE as Cliente,
                            CD_EMPRESA as Empresa,
                            CD_PUNTO_ENTREGA as PuntoEntrega,
                            CD_ROTA as Ruta,
                            CD_TRANSPORTADORA as Transportadora,
                            DS_ANEXO as Anexo,
                            DS_ENTREGA as DescripcionEntrega,
                            DT_ADDROW as FechaAlta,
                            DT_ANULADA as FechaAnulacion,
                            DT_CARGADO as FechaCarga,
                            DT_EXPEDIDO as FechaExpedicion,
                            FL_REENVIO as IdReenvio,
                            NU_CONTENEDOR as Contenedor,
                            NU_ENTREGA as Id,
                            NU_INTERFAZ_EJECUCION as NumeroInterfazEjecucion,
                            NU_INTERFAZ_EJECUCION_ANULA as NumeroInterfazEjecucionAnulacion,
                            NU_ORDEN_ENTREGA as NroOrdenEntrega,
                            NU_PREPARACION as Preparacion,
                            TP_ENTREGA as TipoEntrega,
                            VL_AGRUPACION_ENTREGA as AgrupacionEntrega,
                            VL_SERIALIZADO as Serializado
                        FROM T_ENTREGA 
                        WHERE CD_CAMION = :cdcamion 
                        AND CD_EMPRESA = :empresa";

            return _dapper.Query<Entrega>(connection, sql, param: new
            {
                cdcamion = cdcamion,
                empresa = empresa
            }, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<DetallePedidoDuplicado> GetAllDetallesDuplicadosExp(List<DetallePedidoExpedido> detalles)
        {
            var objs = new List<object>();
            var keys = new List<string>();
            foreach (var det in detalles)
            {
                string key = $"{det.Pedido}.{det.Cliente}.{det.Empresa}.{det.Producto}";
                if (!keys.Contains(key))
                {
                    objs.Add(new
                    {
                        Pedido = det.Pedido,
                        Empresa = det.Empresa,
                        Cliente = det.Cliente,
                        Producto = det.Producto
                    });
                    keys.Add(key);
                }
            }
            return GetDupTemp(objs);
        }

        public virtual List<DetallePedidoDuplicado> GetAllDetallesDuplicadosFact(List<DetallePedido> detalles)
        {
            var objs = new List<object>();
            var keys = new List<string>();
            foreach (var det in detalles)
            {
                string key = $"{det.Id}.{det.Cliente}.{det.Empresa}.{det.Producto}";
                if (!keys.Contains(key))
                {
                    objs.Add(new
                    {
                        Pedido = det.Id,
                        Empresa = det.Empresa,
                        Cliente = det.Cliente,
                        Producto = det.Producto
                    });
                    keys.Add(key);
                }
            }
            return GetDupTemp(objs);
        }

        public virtual List<DetallePedidoDuplicado> GetDupTemp(List<object> objs)
        {
            List<DetallePedidoDuplicado> resultado;
            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_DET_PEDIDO_SAIDA_DUP_TEMP (NU_PEDIDO, CD_EMPRESA, CD_CLIENTE, CD_PRODUTO) VALUES (:Pedido, :Empresa, :Cliente, :Producto)";
                    _dapper.Execute(connection, sql, objs, transaction: tran);

                    sql = @"SELECT  P.NU_PEDIDO AS Pedido,
                            P.CD_CLIENTE AS Cliente,
                            P.CD_EMPRESA AS Empresa,
                            P.CD_PRODUTO AS Producto,
                            P.CD_FAIXA AS Faixa,
                            P.NU_IDENTIFICADOR AS Identificador,
                            P.ID_ESPECIFICA_IDENTIFICADOR AS IdEspecificaIdentificador,
                            P.ID_LINEA_SISTEMA_EXTERNO AS IdLineaSistemaExterno,
                            P.TP_LINEA AS TipoLinea,
                            P.QT_PEDIDO AS CantidadPedida,
                            P.QT_EXPEDIDO AS CantidadExpedida,
                            P.QT_ANULADO AS CantidadAnulada,
                            P.QT_FACTURADO AS CantidadFacturada,
                            P.DT_ADDROW AS FechaAlta,
                            P.DT_UPDROW AS FechaModificacion,
                            P.VL_SERIALIZADO_1 AS DatosSerializados
                        FROM T_DET_PEDIDO_SAIDA_DUP P 
                        INNER JOIN T_DET_PEDIDO_SAIDA_DUP_TEMP T ON P.NU_PEDIDO = T.NU_PEDIDO AND P.CD_CLIENTE = T.CD_CLIENTE 
                        AND P.CD_EMPRESA = T.CD_EMPRESA AND P.CD_PRODUTO = T.CD_PRODUTO";

                    resultado = _dapper.Query<DetallePedidoDuplicado>(connection, sql, transaction: tran).ToList();

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual void UpdateCamionInicioCierre(int cdCamion, short situacion, DateTime fechaModificacion, long transaccion)
        {
            var param = new DynamicParameters(new
            {
                DT_UPDROW = fechaModificacion,
                CD_SITUACAO = situacion,
                NU_TRANSACCION = transaccion,
                CD_CAMION = cdCamion
            });

            string sql = @"UPDATE T_CAMION 
                                        SET DT_UPDROW = :DT_UPDROW,
                                            CD_SITUACAO = :CD_SITUACAO, 
                                            NU_TRANSACCION = :NU_TRANSACCION
                                        WHERE CD_CAMION = :CD_CAMION";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual List<dtoDetalleContenedorPuerta> GetContenedoresCamion(DbConnection connection, DbTransaction tran, int idCamion, string ubicacionPuerta)
        {
            var param = new DynamicParameters(new
            {
                CD_CAMION = idCamion,
                CD_ENDERECO = ubicacionPuerta,

            });
            string sql = @"SELECT 
                                CD_EMPRESA  as Empresa,
                                CD_PRODUTO as Producto,
                                CD_FAIXA as Faixa,
                                NU_IDENTIFICADOR as Lote,
                                CD_CLIENTE as Cliente,
                                NU_PEDIDO as Pedido,
                                ID_ESPECIFICA_IDENTIFICADOR as EspecificaIdentificador,
                                CD_ENDERECO as Ubicacion,
                                CD_CAMION as Camion,
                                QT_PREPARADO as CantidadPreparada
                                FROM V_CANT_CONTE_PUERTA_WEXP 
                                WHERE CD_CAMION = :CD_CAMION 
                                AND CD_ENDERECO = :CD_ENDERECO";

            return _dapper.Query<dtoDetalleContenedorPuerta>(connection, sql, param: param, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual void UpdateStockExpedicionCamion(int idCamion, string ubicacionPuerta, long transaccion)
        {
            var param = new DynamicParameters(new
            {
                CD_CAMION = idCamion,
                CD_ENDERECO = ubicacionPuerta,
                NU_TRANSACCION = transaccion,
                DT_UPDROW = DateTime.Now,
            });

            var alias = "st";
            var from = @"
                T_STOCK st
                INNER JOIN (
                    SELECT 
                        CD_ENDERECO,
                        CD_EMPRESA,
                        CD_PRODUTO,
                        CD_FAIXA,
                        NU_IDENTIFICADOR,
                        SUM(QT_PREPARADO) QT_PREPARADO
                    FROM V_CANT_CONTE_PUERTA_WEXP
                    WHERE CD_CAMION = :CD_CAMION
                        AND CD_ENDERECO = :CD_ENDERECO 
                    GROUP BY CD_ENDERECO, CD_EMPRESA, CD_PRODUTO, CD_FAIXA, NU_IDENTIFICADOR
                ) cc ON 
                cc.CD_ENDERECO = st.CD_ENDERECO AND
                cc.CD_PRODUTO = st.CD_PRODUTO AND
                cc.CD_FAIXA = st.CD_FAIXA AND
                cc.NU_IDENTIFICADOR  = st.NU_IDENTIFICADOR AND
                cc.CD_EMPRESA = st.CD_EMPRESA ";

            var set = @"
                QT_ESTOQUE = QT_ESTOQUE - QT_PREPARADO,
                QT_RESERVA_SAIDA = QT_RESERVA_SAIDA - QT_PREPARADO,
                NU_TRANSACCION = :NU_TRANSACCION,
                DT_UPDROW = :DT_UPDROW";

            var where = "";

            _dapper.ExecuteUpdate(_context.Database.GetDbConnection(), alias, from, set, where, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void RemoveClienteCamion(int cdCamion)
        {
            var param = new DynamicParameters(new
            {
                CD_CAMION = cdCamion
            });

            string sql = @"DELETE T_CLIENTE_CAMION WHERE CD_CAMION = :CD_CAMION";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdateExpedirContenedor(int cdCamion, string ubicacionPuerta, short situacionCondicion, short situacionUpdate, long nuTransaccion, out IEnumerable<long> nuLpns)
        {
            var param = new DynamicParameters(new
            {
                CD_SITUACAO = situacionUpdate,
                DT_EXPEDIDO = DateTime.Now,
                NU_TRANSACCION = nuTransaccion,
                CD_CAMION = cdCamion,
                CD_ENDERECO = ubicacionPuerta,
                CD_SITUACAO_WHERE = situacionCondicion
            });

            string sql = @" SELECT 
                                NU_LPN 
                            FROM T_CONTENEDOR
                            WHERE CD_CAMION = :CD_CAMION
                            AND CD_ENDERECO = :CD_ENDERECO
                            AND CD_SITUACAO = :CD_SITUACAO_WHERE
                            AND NU_LPN IS NOT NULL ";

            nuLpns = _dapper.GetAll<long>(sql, param);

            sql = @"UPDATE T_CONTENEDOR 
                    SET CD_SITUACAO = :CD_SITUACAO ,
                        DT_EXPEDIDO = :DT_EXPEDIDO,
                        NU_TRANSACCION = :NU_TRANSACCION
                    WHERE CD_CAMION = :CD_CAMION
                    AND CD_ENDERECO = :CD_ENDERECO
                    AND CD_SITUACAO = :CD_SITUACAO_WHERE";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdateExpedirUt(int cdCamion, string ubicacionPuerta, short situacionUpdate, long nuTransaccion)
        {

            var param = new DynamicParameters(new
            {
                CD_CAMION = cdCamion,
                CD_ENDERECO = ubicacionPuerta,
                DT_UPDROW = DateTime.Now,
                CD_SITUACAO = situacionUpdate,
            });

            var alias = "ut";
            var from = @"
                T_UNIDAD_TRANSPORTE ut
                INNER JOIN (
                    SELECT 
                       ut.NU_UNIDAD_TRANSPORTE
                    FROM T_CONTENEDOR c
                    INNER JOIN T_UNIDAD_TRANSPORTE ut on ut.NU_UNIDAD_TRANSPORTE = c.NU_UNIDAD_TRANSPORTE
                    WHERE c.CD_CAMION = :CD_CAMION
                        AND c.CD_ENDERECO = :CD_ENDERECO 
                      GROUP BY ut.NU_UNIDAD_TRANSPORTE
                ) cc ON 
                cc.NU_UNIDAD_TRANSPORTE = ut.NU_UNIDAD_TRANSPORTE";
            var set = @"
                DT_UPDROW = :DT_UPDROW,
                CD_SITUACAO = :CD_SITUACAO";
            var where = "";

            _dapper.ExecuteUpdate(_context.Database.GetDbConnection(), alias, from, set, where, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdateEntregaNoCargadas(int cdCamion)
        {
            var param = new DynamicParameters(new
            {
                CD_CAMION = cdCamion,
            });

            string sql = @"UPDATE T_ENTREGA
                                             SET CD_CAMION = NULL,
                                                 DT_EXPEDIDO = NULL,
                                                 DT_CARGADO = NULL
                                             WHERE CD_CAMION = :CD_CAMION
                                               AND DT_CARGADO IS NULL";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdateEntregaExpedir(int cdCamion)
        {
            var param = new DynamicParameters(new
            {
                DT_EXPEDIDO = DateTime.Now,
                CD_CAMION = cdCamion
            });

            string sql = @"UPDATE T_ENTREGA
                                      SET DT_EXPEDIDO = :DT_EXPEDIDO
                                      WHERE CD_CAMION = :CD_CAMION
                                        AND DT_CARGADO IS NOT NULL";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdateCamionCierre(int cdCamion, short situacion, DateTime fechaModificacion, long transaccion)
        {
            var param = new DynamicParameters(new
            {
                CD_SITUACAO = situacion,
                DT_UPDROW = fechaModificacion,
                DT_CIERRE = fechaModificacion,
                NU_INTERFAZ_EJECUCION = -1,
                NU_TRANSACCION = transaccion,
                CD_CAMION = cdCamion
            });

            string sql = @"UPDATE T_CAMION SET
                                                CD_SITUACAO = :CD_SITUACAO,
                                                DT_UPDROW = :DT_UPDROW,
                                                DT_CIERRE = :DT_CIERRE,
                                                NU_INTERFAZ_EJECUCION = :NU_INTERFAZ_EJECUCION,
                                                NU_TRANSACCION = :NU_TRANSACCION
                                                WHERE CD_CAMION = :CD_CAMION";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void AddDetalleExpedicion(int cdCamion)
        {
            var param = new DynamicParameters(new
            {
                DT_EXPEDICION = DateTime.Now,
                CD_CAMION = cdCamion
            });

            string sql = @"
                INSERT INTO T_DET_PEDIDO_EXPEDIDO (
                    CD_CAMION,
                    NU_PEDIDO,
                    CD_CLIENTE,
                    CD_EMPRESA,
                    CD_PRODUTO,
                    CD_FAIXA,
                    NU_IDENTIFICADOR,
                    ID_ESPECIFICA_IDENTIFICADOR,
                    QT_PRODUTO,DT_EXPEDICION)
                SELECT
                    dp.CD_CAMION,
                    dp.NU_PEDIDO,
                    dp.CD_CLIENTE,
                    dp.CD_EMPRESA,
                    dp.CD_PRODUTO,
                    dp.CD_FAIXA,
                    dp.NU_IDENTIFICADOR,
                    dp.ID_ESPECIFICA_IDENTIFICADOR,
                    dp.QT_PREPARADO, 
                    :DT_EXPEDICION
                 FROM  (
                    SELECT 
                        c.CD_CAMION,
                        dp.NU_PEDIDO,
                        dp.CD_CLIENTE,
                        dp.CD_EMPRESA,
                        dp.CD_PRODUTO,
                        dp.CD_FAIXA,
                        dp.NU_IDENTIFICADOR,
                        dp.ID_ESPECIFICA_IDENTIFICADOR,
                        SUM (dp.qt_preparado) QT_PREPARADO 
                    FROM T_CONTENEDOR c
                    INNER JOIN T_DET_PICKING dp ON dp.nu_preparacion = c.nu_preparacion 
                        AND dp.nu_contenedor = c.nu_contenedor
                    WHERE c.CD_CAMION = :CD_CAMION 
                    GROUP BY 
                        c.CD_CAMION, 
                        dp.NU_PEDIDO, 
                        dp.CD_CLIENTE, 
                        dp.CD_EMPRESA, 
                        dp.CD_PRODUTO, 
                        dp.CD_FAIXA, 
                        dp.NU_IDENTIFICADOR, 
                        dp.ID_ESPECIFICA_IDENTIFICADOR 
                ) dp";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual async Task RegularizarDatosSerializados(DbConnection connection, int cdCamion, long nuTransaccion, bool facturacion, DbTransaction tran = null)
        {
            var sqlFromWhere = @"SELECT  dps.NU_PEDIDO,
                                    dps.CD_EMPRESA ,
                                    dps.CD_CLIENTE ,
                                    dps.CD_PRODUTO ,
                                    dps.NU_IDENTIFICADOR ,
                                    dps.CD_FAIXA ,
                                    dps.ID_ESPECIFICA_IDENTIFICADOR  
                                FROM T_DET_PEDIDO_SAIDA dps 
                                INNER JOIN T_DET_PEDIDO_EXPEDIDO pex ON dps.CD_CLIENTE = pex.CD_CLIENTE 
                                    AND dps.NU_PEDIDO = pex.NU_PEDIDO 
                                    AND dps.CD_EMPRESA = pex.CD_EMPRESA
                                    AND dps.CD_PRODUTO = pex.CD_PRODUTO
                                    AND dps.NU_IDENTIFICADOR = pex.NU_IDENTIFICADOR 
                                    AND dps.CD_FAIXA = pex.CD_FAIXA
                                    AND dps.ID_ESPECIFICA_IDENTIFICADOR = pex.ID_ESPECIFICA_IDENTIFICADOR
                                WHERE pex.CD_CAMION = :cdcamion ";

            if (facturacion)
            {
                sqlFromWhere = @" SELECT  dps.NU_PEDIDO,
                                    dps.CD_EMPRESA ,
                                    dps.CD_CLIENTE ,
                                    dps.CD_PRODUTO ,
                                    dps.NU_IDENTIFICADOR ,
                                    dps.CD_FAIXA ,
                                    dps.ID_ESPECIFICA_IDENTIFICADOR  
                                FROM T_CLIENTE_CAMION cc 
                                INNER JOIN T_DET_PICKING dp ON cc.CD_CLIENTE = dp.CD_CLIENTE 
                                    AND cc.NU_CARGA = dp.NU_CARGA
                                INNER JOIN T_CONTENEDOR co ON dp.NU_PREPARACION = co.NU_PREPARACION 
                                    AND dp.NU_CONTENEDOR = co.NU_CONTENEDOR
                                INNER JOIN T_DET_PEDIDO_SAIDA dps ON dp.NU_PEDIDO = dps.NU_PEDIDO 
                                    AND dp.CD_EMPRESA = dps.CD_EMPRESA 
                                    AND dp.CD_CLIENTE = dps.CD_CLIENTE
                                    AND dp.CD_PRODUTO = dps.CD_PRODUTO
                                    AND dp.NU_IDENTIFICADOR = dps.NU_IDENTIFICADOR 
                                    AND dp.CD_FAIXA = dps.CD_FAIXA
                                    AND dp.ID_ESPECIFICA_IDENTIFICADOR = dps.ID_ESPECIFICA_IDENTIFICADOR
                                WHERE cc.CD_CAMION = :cdcamion 
                                    AND co.CD_CAMION_FACTURADO = :cdcamion ";
            }


            var alias = "dp";
            var from = $@"
                T_DET_PEDIDO_SAIDA dp
                INNER JOIN (
                    SELECT 
                        dp.NU_PEDIDO,
                        dp.CD_EMPRESA ,
                        dp.CD_CLIENTE ,
                        dp.CD_PRODUTO ,
                        dp.NU_IDENTIFICADOR ,
                        dp.CD_FAIXA ,
                        dp.ID_ESPECIFICA_IDENTIFICADOR,
                        MIN(dpt.DS_MEMO ) as DS_MEMO_DESTINO,
                        MIN(dpt.VL_SERIALIZADO_1 ) as VL_SERIALIZADO_DESTINO,
                        MIN(dpt.DT_GENERICO_1 ) as DT_GENERICO_DESTINO,
                        MIN(dpt.NU_GENERICO_1 ) as NU_GENERICO_DESTINO,
                        MIN(dpt.VL_GENERICO_1 ) as VL_GENERICO_DESTINO,
                        MIN(dpt.DS_ANEXO1 ) as DS_ANEXO1_DESTINO,
                        MIN(dpt.DS_ANEXO2 ) as DS_ANEXO2_DESTINO,
                        MIN(dpt.DS_ANEXO3 ) as DS_ANEXO3_DESTINO,
                        MIN(dpt.DS_ANEXO4 ) as DS_ANEXO4_DESTINO
                    FROM ({sqlFromWhere}) dp
                    INNER JOIN T_DET_PEDIDO_SAIDA dpt on dpt.NU_PEDIDO = dp.NU_PEDIDO 
                                    AND dpt.CD_EMPRESA = dp.CD_EMPRESA 
                                    AND dpt.CD_CLIENTE = dp.CD_CLIENTE
                                    AND dpt.CD_PRODUTO = dp.CD_PRODUTO
                                    AND dpt.NU_IDENTIFICADOR = :idenficadorAuto
                                    AND dpt.CD_FAIXA = dp.CD_FAIXA
                                    AND dpt.ID_ESPECIFICA_IDENTIFICADOR = 'N'
                   GROUP by  dp.NU_PEDIDO,
                        dp.CD_EMPRESA ,
                        dp.CD_CLIENTE ,
                        dp.CD_PRODUTO ,
                        dp.NU_IDENTIFICADOR ,
                        dp.CD_FAIXA ,
                        dp.ID_ESPECIFICA_IDENTIFICADOR
                ) dpt ON  dpt.NU_PEDIDO = dp.NU_PEDIDO 
                        AND dpt.CD_EMPRESA = dp.CD_EMPRESA 
                        AND dpt.CD_CLIENTE = dp.CD_CLIENTE
                        AND dpt.CD_PRODUTO = dp.CD_PRODUTO
                        AND dpt.NU_IDENTIFICADOR = dp.NU_IDENTIFICADOR 
                        AND dpt.CD_FAIXA = dp.CD_FAIXA
                        AND dpt.ID_ESPECIFICA_IDENTIFICADOR = dp.ID_ESPECIFICA_IDENTIFICADOR";
            var set = @"
                DS_MEMO = DS_MEMO_DESTINO,
                VL_SERIALIZADO_1 = VL_SERIALIZADO_DESTINO,
                DT_GENERICO_1 = DT_GENERICO_DESTINO,
                NU_GENERICO_1 = NU_GENERICO_DESTINO,
                VL_GENERICO_1 = VL_GENERICO_DESTINO,
                DS_ANEXO1 = DS_ANEXO1_DESTINO,
                DS_ANEXO2 = DS_ANEXO2_DESTINO,
                DS_ANEXO3 = DS_ANEXO3_DESTINO,
                DS_ANEXO4 = DS_ANEXO4_DESTINO,
                DT_UPDROW = :fechaModificacion,
                NU_TRANSACCION = :nuTransaccion ";
            var where = "";

            await _dapper.ExecuteUpdateAsync(connection, alias, from, set, where, param: new
            {
                cdcamion = cdCamion,
                nuTransaccion = nuTransaccion,
                fechaModificacion = DateTime.Now,
                idenficadorAuto = ManejoIdentificadorDb.IdentificadorAuto
            }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

        }

        #endregion

        #region  EXP040 ArmadoPorCarga
        public virtual void AsociarCargaCamion(IUnitOfWork _uow, Camion camion, List<CargaAsociarUnidad> cargas)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var cargasTemp = cargas;
            var grupoExpedicionCamion = GetGrupoExpedicion(connection, tran, camion.Id);

            AddPrepaparacionCargaTemp(connection, tran, cargas);

            ValidarGrupoExpedicion(connection, tran, grupoExpedicionCamion);

            _uow.PreparacionRepository.AnyDetalleParaCarga(connection, tran, out long? cargaConProblema);
            if (cargaConProblema != null)
                throw new ValidationFailedException("WEXP_grid1_Error_NoExisteDetallesParaLaCarga", new string[] { cargaConProblema.ToString() });

            cargas = _uow.PreparacionRepository.GetCargasCliente(connection, tran);
            var cargasPendienteAsociar = cargas
                .GroupBy(x => new { x.Cliente, x.Carga, x.Empresa })
                .Select(x => new CargaAsociarUnidad
                {
                    Cliente = x.Key.Cliente,
                    Empresa = x.Key.Empresa,
                    Carga = x.Key.Carga,
                    FechaAlta = DateTime.Now,
                    Camion = camion.Id,
                    TipoModalidadArmado = TipoModalidadArmado.Carga,
                    IdCarga = "S",
                    FlSyncRealizada = "N",
                })
                .ToList();

            List<CargaCamion> cargasAsociadas = AddCargaCamion(connection, tran, cargasPendienteAsociar);

            camion.Cargas.AddRange(cargasAsociadas);

            RemovePrepaparacionCargaTemp(connection, tran, cargasTemp);

        }

        public virtual void DesasociarCargaCamion(IUnitOfWork _uow, Camion camion, List<CargaAsociarUnidad> cargas)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var cargasTemp = cargas;

            AddPrepaparacionCargaTemp(connection, tran, cargas);

            List<CargaCamion> cargasPedidosEliminar = GetCargasCamion(connection, tran, camion.Id);

            RemoveCargasCamion(connection, tran, camion.Id, cargasPedidosEliminar);

            foreach (var carga in cargasPedidosEliminar)
                camion.Cargas.Remove(camion.Cargas.FirstOrDefault(x => x.Carga == carga.Carga && x.Cliente == carga.Cliente));

            RemovePrepaparacionCargaTemp(connection, tran, cargasTemp);

        }

        #region Get

        public virtual void ValidarGrupoExpedicion(DbConnection connection, DbTransaction tran, string grupoExpedicionCamion)
        {
            if (!string.IsNullOrEmpty(grupoExpedicionCamion))
            {
                var parameter = new DynamicParameters(new
                {
                    CD_GRUPO_EXPEDICION = grupoExpedicionCamion
                });

                string sql = @$"SELECT 
                            ct.NU_CARGA 
                        FROM T_CARGA_TEMP  ct
                        INNER JOIN V_EXP010_CARGA_CAMION  ecc ON  ecc.NU_PREPARACION = ct.NU_PREPARACION
                            AND ecc.CD_EMPRESA = ct.CD_EMPRESA
                            AND ecc.CD_CLIENTE = ct.CD_CLIENTE
                            AND ecc.NU_CARGA = ct.NU_CARGA
                        WHERE ecc.CD_GRUPO_EXPEDICION is not null 
                            AND  ecc.CD_GRUPO_EXPEDICION  <> :CD_GRUPO_EXPEDICION";

                var cargaConProblema = _dapper.Query<long?>(connection, sql, param: parameter, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
                if (cargaConProblema != null)
                    throw new ValidationFailedException("WEXP_grid1_Error_CargaConDiferenteGrupoExpedicionCamion", new string[] { Convert.ToString(cargaConProblema) });

            }
        }

        public virtual List<CargaCamion> GetCargasCamion(DbConnection connection, DbTransaction tran, int camion)
        {
            var parameter = new DynamicParameters(new
            {
                CD_CAMION = camion
            });

            string sql = @$"		SELECT 
                            cc.CD_CAMION as Camion,
                            ct.NU_CARGA as Carga,
                            cc.CD_CLIENTE as Cliente,
                            cc.CD_EMPRESA as Empresa,
                            cc.ID_CARGAR as IdCargar,
                            cc.TP_MODALIDAD as TipoModalidad,
                            CASE WHEN cc.FL_SYNC_REALIZADA = 'S' THEN 1 ELSE 0  END as SincronizacionRealizada,
                            cc.DT_UPDROW as FechaModificacion,
                            cc.DT_ADDROW as FechaAlta
                        FROM T_CARGA_TEMP  ct
                        INNER JOIN T_CLIENTE_CAMION  cc ON cc.CD_EMPRESA = ct.CD_EMPRESA
                            AND cc.NU_CARGA = ct.NU_CARGA
                            AND cc.CD_CAMION = :CD_CAMION";

            return _dapper.Query<CargaCamion>(connection, sql, param: parameter, commandType: CommandType.Text, transaction: tran).ToList();

        }

        #endregion

        #region Remove

        public virtual void RemoveCargasCamion(DbConnection connection, DbTransaction tran, int id, List<CargaCamion> cargasPedidosEliminar)
        {
            _dapper.BulkDelete(connection, tran, cargasPedidosEliminar, "T_CLIENTE_CAMION", new Dictionary<string, Func<CargaCamion, object>>
            {
                { "CD_CAMION", x => x.Camion},
                { "CD_CLIENTE", x => x.Cliente},
                { "CD_EMPRESA", x => x.Empresa},
                { "NU_CARGA", x => x.Carga},
            });
        }

        public virtual void RemovePrepaparacionCargaTemp(DbConnection connection, DbTransaction tran, List<CargaAsociarUnidad> pedidosAsociar)
        {
            _dapper.BulkDelete(connection, tran, pedidosAsociar, "T_CARGA_TEMP", new Dictionary<string, Func<CargaAsociarUnidad, object>>
            {
                { "CD_EMPRESA", x => x.Empresa},
                { "CD_CLIENTE", x => x.Cliente},
                { "NU_PREPARACION", x => x.Preparacion},
                { "NU_CARGA", x => x.Carga},
                { "CD_GRUPO_EXPEDICION", x => x.GrupoExpedicion},
            });

        }

        #endregion

        #region Add

        public virtual void AddPrepaparacionCargaTemp(DbConnection connection, DbTransaction tran, List<CargaAsociarUnidad> pedidosAsociar)
        {
            _dapper.BulkInsert(connection, tran, pedidosAsociar, "T_CARGA_TEMP", new Dictionary<string, Func<CargaAsociarUnidad, ColumnInfo>>
            {
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "NU_PREPARACION", x => new ColumnInfo( x.Preparacion)},
                { "NU_CARGA", x => new ColumnInfo(x.Carga)},
                { "CD_GRUPO_EXPEDICION", x => new ColumnInfo(x.GrupoExpedicion)},
            });

        }

        public virtual List<CargaCamion> AddCargaCamion(DbConnection connection, DbTransaction tran, List<CargaAsociarUnidad> cargasPedidos)
        {
            _dapper.BulkInsert(connection, tran, cargasPedidos, "T_CLIENTE_CAMION", new Dictionary<string, Func<CargaAsociarUnidad, ColumnInfo>>
             {
                { "CD_CAMION", x => new ColumnInfo(x.Camion)},
                { "NU_CARGA", x => new ColumnInfo(x.Carga)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "DT_ADDROW", x => new ColumnInfo(x.FechaAlta)},
                { "TP_MODALIDAD", x => new ColumnInfo(x.TipoModalidadArmado)},
                { "ID_CARGAR", x => new ColumnInfo(x.IdCarga)},
                { "FL_SYNC_REALIZADA", x => new ColumnInfo(x.FlSyncRealizada)},
            });

            List<CargaCamion> cargasCamion = new List<CargaCamion>();
            foreach (var cargaCamion in cargasPedidos)
            {
                cargasCamion.Add(new CargaCamion
                {
                    Camion = cargaCamion.Camion,
                    Carga = cargaCamion.Carga,
                    Cliente = cargaCamion.Cliente,
                    Empresa = cargaCamion.Empresa,
                    FechaAlta = DateTime.Now,
                    TipoModalidad = TipoModalidadArmado.Pedido,
                    SincronizacionRealizada = false,
                    SincronizacionRealizadaId = "N",
                    IdCargar = "S"
                });
            }

            return cargasCamion;
        }

        #endregion

        #endregion

        #region  EXP040 ArmadoPorPedido

        public virtual void AsociarPedidoCamion(IUnitOfWork _uow, Camion camion, List<PedidoAsociarUnidad> cargasPedidos)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();
            var cargasPedidosTemp = cargasPedidos;

            AddPedidoTemp(connection, tran, cargasPedidos);

            if (AnyAnulacionPendiente(connection, tran, out string pedido))
                throw new ValidationFailedException("WEXP013_Sec0_Error_PedidoConAnulacionPendiente", new string[] { pedido });

            cargasPedidos = GetPreparacionPedidosCargasNoAsociadas(connection, tran, camion.Id);

            AddPrepaparacionCargaTemp(connection, tran, cargasPedidos);

            var cargasMultiPedidosPedidos = GetCargasMultiplesPedidos(connection, tran);

            var cargasPedidosUpdate = cargasPedidos.Join(cargasMultiPedidosPedidos,
                 cp => cp.Carga,
                 cmpp => cmpp,
                 (cp, cmpp) => cp).ToList();

            var cantidadCargasGenerar = cargasPedidosUpdate.Count();

            if (cantidadCargasGenerar > 0)
            {
                var newCargas = _cargaRepository.GetNewNumeroCargas(cantidadCargasGenerar, connection, tran);

                foreach (var cargaPedidoUpdate in cargasPedidosUpdate)
                {
                    var newCarga = newCargas[0];
                    cargaPedidoUpdate.CargaDestino = newCarga;
                    newCargas.RemoveAt(0);
                }
            }

            UpdatePrepaparacionCargaTemp(connection, tran, cargasPedidosUpdate);

            var detallePedìdoUpdate = cargasPedidosUpdate.Select(x => new PedidoAsociarUnidad
            {
                Preparacion = x.Preparacion,
                Cliente = x.Cliente,
                Pedido = x.Pedido,
                Carga = x.Carga,
                Empresa = x.Empresa,
                CargaDestino = x.CargaDestino,
                Descripcion = $"Generada por el armado de egreso para el Pedido {x.Pedido} - Cliente: {x.Cliente} - Empresa: {x.Empresa}",
                FechaAlta = DateTime.Now,
                Transaccion = _uow.GetTransactionNumber(),
                Ruta = x.Ruta
            }).ToList();

            CopiarCargaUpdatePrepaparacionCarga(connection, tran, detallePedìdoUpdate);

            var cargasPedidoAsociar = cargasPedidos
                .GroupBy(x => new { x.Cliente, x.Carga, x.CargaDestino, x.Empresa })
                .Select(x => new PedidoAsociarUnidad
                {
                    Cliente = x.Key.Cliente,
                    Empresa = x.Key.Empresa,
                    Carga = x.Key.CargaDestino ?? x.Key.Carga,
                    FechaAlta = DateTime.Now,
                    Camion = camion.Id,
                    TipoModalidadArmado = TipoModalidadArmado.Pedido,
                    IdCarga = "S",
                    FlSyncRealizada = "N",
                })
                .ToList();

            List<CargaCamion> cargas = AddCargaCamion(connection, tran, cargasPedidoAsociar);

            camion.Cargas.AddRange(cargas);

            CargaPredefinida(connection, tran, camion);

            RemovePrepaparacionCargaTemp(connection, tran, cargasPedidos);

            RemovePedidoTemp(connection, tran, cargasPedidosTemp);
        }

        public virtual void CargaPredefinida(DbConnection connection, DbTransaction tran, Camion camion)
        {
            List<PedidoAsociarUnidad> cargasPedidos = GetPedidoConPendiente(connection, tran);

            var pedidoGenerarCargaPredefinida = cargasPedidos.Where(x => x.Carga == null).ToList();

            if (pedidoGenerarCargaPredefinida.Count() > 0)
            {
                var newCargas = _cargaRepository.GetNewNumeroCargas(pedidoGenerarCargaPredefinida.Count(), connection, tran);

                foreach (var cargaPedidoUpdate in pedidoGenerarCargaPredefinida)
                {
                    var newCarga = newCargas[0];
                    cargaPedidoUpdate.CargaDestino = newCarga;
                    cargaPedidoUpdate.Descripcion = $"Generada por el armado de egreso para el Pedido {cargaPedidoUpdate.Pedido} - Cliente: {cargaPedidoUpdate.Cliente} - Empresa: {cargaPedidoUpdate.Empresa}";
                    cargaPedidoUpdate.FechaAlta = DateTime.Now;
                    newCargas.RemoveAt(0);
                }
            }

            var cargasNuevas = cargasPedidos.Where(x => x.CargaDestino != null).ToList();

            UpdatePedidoCargaPredefinida(connection, tran, cargasNuevas);

            AddCarga(connection, tran, cargasNuevas);

            var cargasPedidoAsociar = cargasPedidos
                .GroupBy(x => new { x.Cliente, x.Carga, x.CargaDestino, x.Empresa })
                .Select(x => new PedidoAsociarUnidad
                {
                    Cliente = x.Key.Cliente,
                    Empresa = x.Key.Empresa,
                    Carga = x.Key.CargaDestino ?? x.Key.Carga,
                    FechaAlta = DateTime.Now,
                    Camion = camion.Id,
                    TipoModalidadArmado = TipoModalidadArmado.Pedido,
                    IdCarga = "S",
                    FlSyncRealizada = "N",
                })
                .ToList();

            List<CargaCamion> cargas = AddCargaCamion(connection, tran, cargasPedidoAsociar);

            camion.Cargas.AddRange(cargas);
        }

        public virtual void DesasociarPedidosCamion(IUnitOfWork _uow, Camion camion, List<PedidoAsociarUnidad> cargasPedidos)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();
            var cargasPedidosTemp = cargasPedidos;

            AddPedidoTemp(connection, tran, cargasPedidos);

            cargasPedidos = GetCargasPedidoCamion(connection, tran, camion.Id);

            AddPrepaparacionCargaTemp(connection, tran, cargasPedidos);

            var cargasMultiPedidosPedidos = GetCargasMultiplesPedidos(connection, tran);

            var cargasPedidosUpdate = cargasPedidos.Join(cargasMultiPedidosPedidos,
                 cp => cp.Carga,
                 cmpp => cmpp,
                 (cp, cmpp) => cp).ToList();

            var cantidadCargasGenerar = cargasPedidosUpdate.Count();

            if (cantidadCargasGenerar > 0)
            {
                var newCargas = _cargaRepository.GetNewNumeroCargas(cantidadCargasGenerar, connection, tran);

                foreach (var cargaPedidoUpdate in cargasPedidosUpdate)
                {
                    var newCarga = newCargas[0];
                    cargaPedidoUpdate.CargaDestino = newCarga;
                    newCargas.RemoveAt(0);
                }
            }

            UpdatePrepaparacionCargaTemp(connection, tran, cargasPedidosUpdate);

            var detallePedìdoUpdate = cargasPedidosUpdate.Select(x => new PedidoAsociarUnidad
            {
                Preparacion = x.Preparacion,
                Cliente = x.Cliente,
                Pedido = x.Pedido,
                Carga = x.Carga,
                Empresa = x.Empresa,
                CargaDestino = x.CargaDestino,
                Descripcion = $"Desarmar camión pedido. Pedido:  {x.Pedido}",
                FechaAlta = DateTime.Now,
                Ruta = x.Ruta,
                Transaccion = _uow.GetTransactionNumber()
            }).ToList();

            CopiarCargaUpdatePrepaparacionCarga(connection, tran, detallePedìdoUpdate);

            var cargasPedidosEliminar = cargasPedidos.Except(cargasPedidosUpdate).Select(x => new PedidoAsociarUnidad
            {
                Preparacion = x.Preparacion,
                Cliente = x.Cliente,
                Pedido = x.Pedido,
                Carga = x.Carga,
                Empresa = x.Empresa,
                Camion = camion.Id
            }).ToList();

            RemoveCargasCamion(connection, tran, camion.Id, cargasPedidosEliminar);

            var cargasSinDetalles = RemoveCargasSinDetallePreparacionCamion(connection, tran, camion.Id);

            var cargas = camion.Cargas.Join(cargasPedidosEliminar,
                 cp => new { Carga = (long?)cp.Carga, cp.Cliente },
                 cpe => new { cpe.Carga, cpe.Cliente },
                 (cp, cpe) => cp).ToList();

            foreach (var carga in cargas)
                camion.Cargas.Remove(carga);

            cargas = camion.Cargas.Join(cargasSinDetalles,
                cp => new { Carga = (long?)cp.Carga, cp.Cliente },
                cpe => new { cpe.Carga, cpe.Cliente },
                (cp, cpe) => cp).ToList();

            foreach (var carga in cargas)
                camion.Cargas.Remove(carga);

            RemovePrepaparacionCargaTemp(connection, tran, cargasPedidos);

            RemovePedidoTemp(connection, tran, cargasPedidosTemp);
        }

        #region Get

        public virtual List<PedidoAsociarUnidad> GetPedidoConPendiente(DbConnection connection, DbTransaction tran)
        {
            string sql = @$"SELECT 
                            ps.NU_CARGA as Carga,
                            ps.NU_PEDIDO as Pedido,
                            ps.CD_EMPRESA as Empresa,
                            ps.CD_CLIENTE as Cliente,
                            ps.CD_ROTA as Ruta
                        FROM T_PEDIDO_SAIDA_TEMP pst
                        INNER JOIN V_PRE100_PEDIDO_SAIDA ps ON ps.NU_PEDIDO = pst.NU_PEDIDO
                            AND ps.CD_EMPRESA = pst.CD_EMPRESA
                            AND ps.CD_CLIENTE = pst.CD_CLIENTE
                        LEFT JOIN T_CLIENTE_CAMION cc on cc.NU_CARGA = ps.NU_CARGA 
                            AND cc.CD_CLIENTE = ps.CD_CLIENTE
                        WHERE ((ps.NU_CARGA is not null AND cc.NU_CARGA is null) OR ps.NU_CARGA is null)
                            AND (ps.QT_PEDIDO > (ps.QT_LIBERADO + ps.QT_ANULADO))
                        GROUP BY 
                            ps.NU_CARGA,
                            ps.NU_PEDIDO,
                            ps.CD_EMPRESA,
                            ps.CD_CLIENTE,
                            ps.CD_ROTA";

            return _dapper.Query<PedidoAsociarUnidad>(connection, sql, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<long> GetCargasMultiplesPedidos(DbConnection connection, DbTransaction tran)
        {
            string sql = @$"SELECT 
                            dp.NU_CARGA 
                        FROM T_DET_PICKING_TEMP  pst
                        INNER JOIN V_CARGAS_CON_MULTIPLE_PEDIDO dp ON dp.NU_CARGA = pst.NU_CARGA
                        GROUP BY dp.NU_CARGA";

            return _dapper.Query<long>(connection, sql, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<PedidoAsociarUnidad> GetPreparacionPedidosCargasNoAsociadas(DbConnection connection, DbTransaction tran, int cdCamion)
        {
            var parameters = new DynamicParameters(new
            {
                CD_SITUACAO = SituacionDb.ContenedorEnPreparacion,
                CD_CAMION = cdCamion,
                ESTADOS_ANULACION = EstadoDetallePreparacion.GetEstadosAnulacion(),
            });

            var sql = @$"SELECT 
                            dp.NU_PEDIDO as Pedido,
                            dp.CD_CLIENTE as Cliente,
                            dp.CD_EMPRESA as Empresa,
                            dp.NU_PREPARACION as Preparacion,
                            dp.NU_CARGA as Carga,
                            c.CD_ROTA as Ruta
                        FROM T_PEDIDO_SAIDA_TEMP pst
                        INNER JOIN T_DET_PICKING  dp ON dp.NU_PEDIDO = pst.NU_PEDIDO
                            AND dp.CD_EMPRESA = pst.CD_EMPRESA
                            AND dp.CD_CLIENTE = pst.CD_CLIENTE
                        INNER JOIN T_CARGA c ON c.NU_CARGA = dp.NU_CARGA
                        LEFT JOIN T_CLIENTE_CAMION cc on dp.NU_CARGA = cc.NU_CARGA 
                            AND dp.CD_CLIENTE = cc.CD_CLIENTE 
                            AND cc.CD_CAMION = :CD_CAMION
                        LEFT JOIN T_CONTENEDOR co on dp.NU_PREPARACION = co.NU_PREPARACION 
                            AND dp.NU_CONTENEDOR = co.NU_CONTENEDOR 
                            AND co.CD_SITUACAO = :CD_SITUACAO
                        WHERE cc.CD_CAMION is null
                            AND dp.ND_ESTADO NOT IN :ESTADOS_ANULACION 
                            AND (dp.NU_CONTENEDOR is null OR co.NU_CONTENEDOR is not null) 
                        GROUP BY dp.NU_PEDIDO,
                            dp.CD_CLIENTE,
                            dp.CD_EMPRESA,
                            dp.NU_PREPARACION,
                            dp.NU_CARGA,
                            c.CD_ROTA";

            return _dapper.Query<PedidoAsociarUnidad>(connection, sql, param: parameters, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<PedidoAsociarUnidad> GetCargasPedidoCamion(DbConnection connection, DbTransaction tran, int cdCamion)
        {
            var parameters = new DynamicParameters(new
            {
                CD_CAMION = cdCamion
            });
            string sql = @$"
                        SELECT 
                            pst.NU_PEDIDO as Pedido,
                            pst.CD_CLIENTE as Cliente,
                            pst.CD_EMPRESA as Empresa,
                            ccp.NU_CARGA as Carga,
                            c.CD_ROTA as Ruta,
                            ccp.NU_PREPARACION as Preparacion
                        FROM T_PEDIDO_SAIDA_TEMP pst
                        INNER JOIN V_CAMION_CARGA_PEDIDO ccp ON pst.CD_CLIENTE = ccp.CD_CLIENTE 
                            AND pst.CD_EMPRESA = ccp.CD_EMPRESA 
                            AND pst.NU_PEDIDO = ccp.NU_PEDIDO 
                            AND ccp.CD_CAMION = :CD_CAMION
                        INNER JOIN T_CARGA c ON c.NU_CARGA = ccp.NU_CARGA
                        GROUP BY pst.NU_PEDIDO,
                            pst.CD_CLIENTE,
                            pst.CD_EMPRESA,
                            ccp.NU_CARGA,
                            c.CD_ROTA,
                            ccp.NU_PREPARACION
                        UNION 
                        SELECT 
                            pst.NU_PEDIDO as Pedido,
                            pst.CD_CLIENTE as Cliente,
                            pst.CD_EMPRESA as Empresa,
                            cc.NU_CARGA as Carga,
                            c.CD_ROTA as Ruta,
                            null as Preparacion
                        FROM T_PEDIDO_SAIDA_TEMP pst
                        INNER JOIN T_PEDIDO_SAIDA ps ON pst.NU_PEDIDO = ps.NU_PEDIDO
                            AND pst.CD_CLIENTE = ps.CD_CLIENTE 
                            AND pst.CD_EMPRESA = ps.CD_EMPRESA 
                        INNER JOIN T_CARGA c ON c.NU_CARGA = ps.NU_CARGA
                        LEFT JOIN T_CLIENTE_CAMION cc ON cc.CD_CAMION = :CD_CAMION 
                            AND ps.CD_CLIENTE = cc.CD_CLIENTE 
                            AND ps.NU_CARGA = cc.NU_CARGA 
                        WHERE cc.CD_CAMION is not null";

            return _dapper.Query<PedidoAsociarUnidad>(connection, sql, param: parameters, commandType: CommandType.Text, transaction: tran).ToList();
        }

        #endregion

        #region Remove
        public virtual void RemovePedidoTemp(DbConnection connection, DbTransaction tran, List<PedidoAsociarUnidad> pedidosAsociar)
        {
            _dapper.BulkDelete(connection, tran, pedidosAsociar, "T_PEDIDO_SAIDA_TEMP", new Dictionary<string, Func<PedidoAsociarUnidad, object>>
            {
                { "NU_PEDIDO", x => x.Pedido},
                { "CD_CLIENTE", x => x.Cliente},
                { "CD_EMPRESA", x => x.Empresa},
            });
        }

        public virtual void RemovePrepaparacionCargaTemp(DbConnection connection, DbTransaction tran, List<PedidoAsociarUnidad> pedidosAsociar)
        {
            _dapper.BulkDelete(connection, tran, pedidosAsociar, "T_DET_PICKING_TEMP", new Dictionary<string, Func<PedidoAsociarUnidad, object>>
            {
                { "NU_PEDIDO", x => x.Pedido},
                { "CD_CLIENTE", x => x.Cliente},
                { "CD_EMPRESA", x => x.Empresa},
            });
        }

        public virtual void RemoveCargasCamion(DbConnection connection, DbTransaction tran, int id, List<PedidoAsociarUnidad> cargasPedidosEliminar)
        {
            _dapper.BulkDelete(connection, tran, cargasPedidosEliminar, "T_CLIENTE_CAMION", new Dictionary<string, Func<PedidoAsociarUnidad, object>>
            {
                { "CD_CAMION", x => x.Camion},
                { "CD_CLIENTE", x => x.Cliente},
                { "CD_EMPRESA", x => x.Empresa},
                { "NU_CARGA", x => x.Carga},
            });
        }

        public virtual List<PedidoAsociarUnidad> RemoveCargasSinDetallePreparacionCamion(DbConnection connection, DbTransaction tran, int camion)
        {
            var sqlDetallesPicking = $@"
                SELECT
                    dpt.CD_CLIENTE as Cliente,
                    dpt.NU_CARGA as Carga,
                    :Camion as Camion,
                    dpt.CD_EMPRESA as Empresa
                FROM T_DET_PICKING_TEMP dpt
                LEFT JOIN T_DET_PICKING dp ON dpt.NU_PREPARACION = dp.NU_PREPARACION
                    AND dpt.CD_CLIENTE = dp.CD_CLIENTE
                    AND dpt.CD_EMPRESA = dp.CD_EMPRESA 
                    AND dpt.NU_CARGA = dp.NU_CARGA
                WHERE dp.NU_PREPARACION is null
                GROUP BY dpt.CD_CLIENTE,
                         dpt.NU_CARGA,
                         dpt.CD_EMPRESA";

            var cargasSinDetallePreparacion = _dapper.Query<PedidoAsociarUnidad>(connection, sqlDetallesPicking, new { Camion = camion }, transaction: tran).ToList();

            _dapper.BulkDelete(connection, tran, cargasSinDetallePreparacion, "T_CLIENTE_CAMION", new Dictionary<string, Func<PedidoAsociarUnidad, object>>
            {
                { "CD_CAMION", x => x.Camion},
                { "CD_CLIENTE", x => x.Cliente},
                { "CD_EMPRESA", x => x.Empresa},
                { "NU_CARGA", x => x.Carga},
            });

            return cargasSinDetallePreparacion;
        }

        #endregion

        #region Update
        public virtual void UpdatePedidoCargaPredefinida(DbConnection connection, DbTransaction tran, List<PedidoAsociarUnidad> pedidosCargaPredefinida)
        {
            _dapper.BulkUpdate(connection, tran, pedidosCargaPredefinida, "T_PEDIDO_SAIDA", new Dictionary<string, Func<PedidoAsociarUnidad, ColumnInfo>>
            {
                { "NU_CARGA", x => new ColumnInfo(x.CargaDestino, DbType.Int64)}
            }, new Dictionary<string, Func<PedidoAsociarUnidad, ColumnInfo>>
            {
                { "NU_PEDIDO", x => new ColumnInfo(x.Pedido)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
            });
        }

        public virtual void UpdatePrepaparacionCargaTemp(DbConnection connection, DbTransaction tran, List<PedidoAsociarUnidad> pedidosAsociar)
        {
            _dapper.BulkUpdate(connection, tran, pedidosAsociar, "T_DET_PICKING_TEMP", new Dictionary<string, Func<PedidoAsociarUnidad, ColumnInfo>>
            {
                { "NU_CARGA_DEST", x => new ColumnInfo(x.CargaDestino, DbType.Int64)}
            }, new Dictionary<string, Func<PedidoAsociarUnidad, ColumnInfo>>
            {
                { "NU_PEDIDO", x => new ColumnInfo(x.Pedido)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "NU_CARGA", x => new ColumnInfo(x.Carga)},
            });
        }

        #endregion

        #region Add

        public virtual void CopiarCargaUpdatePrepaparacionCarga(DbConnection connection, DbTransaction tran, List<PedidoAsociarUnidad> lineas)
        {
            if (lineas.Count > 0)
            {
                _dapper.BulkInsert(connection, tran, lineas, "T_CARGA", new Dictionary<string, Func<PedidoAsociarUnidad, ColumnInfo>>
                {
                    { "NU_CARGA", x => new ColumnInfo(x.CargaDestino)},
                    { "DS_CARGA", x => new ColumnInfo(x.Descripcion)},
                    { "CD_ROTA", x => new ColumnInfo(x.Ruta)},
                    { "DT_ADDROW", x => new ColumnInfo(x.FechaAlta)},
                });
                var param = new DynamicParameters(new
                {
                    NU_TRANSACCION = lineas.FirstOrDefault()?.Transaccion,
                    DT_UPDROW = DateTime.Now,
                });

                var alias = "dp";
                var from = @"
                            T_DET_PICKING dp
                            INNER JOIN (
                                SELECT
                                    dpt.NU_PREPARACION,
                                    dpt.CD_EMPRESA,
                                    dpt.CD_CLIENTE,
                                    dpt.NU_PEDIDO,
                                    dpt.NU_CARGA as NU_CARGA_TEMP,
                                    MAX(dpt.NU_CARGA_DEST) as NU_CARGA_DEST
                                FROM T_DET_PICKING_TEMP dpt
                                WHERE dpt.NU_CARGA_DEST is not null
                                GROUP BY
                                    dpt.NU_PREPARACION,
                                    dpt.CD_EMPRESA,
                                    dpt.CD_CLIENTE,
                                    dpt.NU_PEDIDO,
                                    dpt.NU_CARGA
                            ) cc ON cc.NU_PREPARACION = dp.NU_PREPARACION 
                                AND cc.CD_EMPRESA  = dp.CD_EMPRESA 
                                AND cc.CD_CLIENTE  = dp.CD_CLIENTE 
                                AND cc.NU_PEDIDO  = dp.NU_PEDIDO 
                                AND cc.NU_CARGA_TEMP  = dp.NU_CARGA";
                var set = @"
                    NU_CARGA = NU_CARGA_DEST,
                    NU_TRANSACCION = :NU_TRANSACCION,
                    DT_UPDROW = :DT_UPDROW";
                var where = "";

                _dapper.ExecuteUpdate(connection, alias, from, set, where, param: param, commandType: CommandType.Text, transaction: tran);
            }
        }

        public virtual void AddCarga(DbConnection connection, DbTransaction tran, List<PedidoAsociarUnidad> lineas)
        {
            _dapper.BulkInsert(connection, tran, lineas, "T_CARGA", new Dictionary<string, Func<PedidoAsociarUnidad, ColumnInfo>>
            {
                { "NU_CARGA", x => new ColumnInfo(x.CargaDestino)},
                { "DS_CARGA", x => new ColumnInfo(x.Descripcion)},
                { "CD_ROTA", x => new ColumnInfo(x.Ruta)},
                { "DT_ADDROW", x => new ColumnInfo(x.FechaAlta)},
            });
        }

        public virtual List<CargaCamion> AddCargaCamion(DbConnection connection, DbTransaction tran, List<PedidoAsociarUnidad> cargasPedidos)
        {
            _dapper.BulkInsert(connection, tran, cargasPedidos, "T_CLIENTE_CAMION", new Dictionary<string, Func<PedidoAsociarUnidad, ColumnInfo>>
             {
                { "CD_CAMION", x => new ColumnInfo(x.Camion)},
                { "NU_CARGA", x => new ColumnInfo(x.Carga)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "DT_ADDROW", x => new ColumnInfo(x.FechaAlta)},
                { "TP_MODALIDAD", x => new ColumnInfo(x.TipoModalidadArmado)},
                { "ID_CARGAR", x => new ColumnInfo(x.IdCarga)},
                { "FL_SYNC_REALIZADA", x => new ColumnInfo(x.FlSyncRealizada)},
            });

            List<CargaCamion> cargasCamion = new List<CargaCamion>();
            foreach (var cargaCamion in cargasPedidos)
            {
                cargasCamion.Add(new CargaCamion
                {
                    Camion = cargaCamion.Camion,
                    Carga = cargaCamion.Carga ?? 0,
                    Cliente = cargaCamion.Cliente,
                    Empresa = cargaCamion.Empresa,
                    FechaAlta = DateTime.Now,
                    TipoModalidad = TipoModalidadArmado.Pedido,
                    SincronizacionRealizada = false,
                    SincronizacionRealizadaId = "N",
                    IdCargar = "S"
                });
            }

            return cargasCamion;
        }

        public virtual void AddPrepaparacionCargaTemp(DbConnection connection, DbTransaction tran, List<PedidoAsociarUnidad> pedidosAsociar)
        {
            _dapper.BulkInsert(connection, tran, pedidosAsociar, "T_DET_PICKING_TEMP", new Dictionary<string, Func<PedidoAsociarUnidad, ColumnInfo>>
            {
                { "NU_PEDIDO", x => new ColumnInfo(x.Pedido)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "NU_PREPARACION", x => new ColumnInfo(x.Preparacion)},
                { "NU_CARGA", x => new ColumnInfo(x.Carga)},
            });

        }

        public virtual void AddPedidoTemp(DbConnection connection, DbTransaction tran, List<PedidoAsociarUnidad> pedidosAsociar)
        {
            _dapper.BulkInsert(connection, tran, pedidosAsociar, "T_PEDIDO_SAIDA_TEMP", new Dictionary<string, Func<PedidoAsociarUnidad, ColumnInfo>>
            {
                { "NU_PEDIDO", x => new ColumnInfo(x.Pedido)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
            });
        }

        #endregion

        #region Any

        public virtual bool AnyAnulacionPendiente(DbConnection connection, DbTransaction tran, out string pedido)
        {
            string sql = @"
                    SELECT 
                        da.NU_PEDIDO as Pedido,
                        da.CD_CLIENTE as Cliente,
                        da.CD_EMPRESA as Empresa
                    FROM T_PEDIDO_SAIDA_TEMP pst 
                    INNER JOIN T_ANULACIONES_PENDIENTES da ON da.NU_PEDIDO = pst.NU_PEDIDO
                        AND da.CD_EMPRESA = pst.CD_EMPRESA
                        AND da.CD_CLIENTE = pst.CD_CLIENTE";

            var pedidoPendienteAnu = _dapper.Query<PedidoAsociarUnidad>(connection, sql, commandType: CommandType.Text, transaction: tran).FirstOrDefault();

            pedido = pedidoPendienteAnu?.Pedido;

            return pedidoPendienteAnu != null;
        }

        #endregion

        #endregion

        #region  EXP040 ArmadoPorContenedor

        public virtual void AsociarContenedorCamion(IUnitOfWork _uow, Camion camion, List<ContenedorAsociarUnidad> contenedores)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var contenedorTemp = contenedores;
            var grupoExpedicionCamion = GetGrupoExpedicion(connection, tran, camion.Id);

            AddPrepaparacionCargaTemp(connection, tran, contenedores);

            ValidarGrupoExpedicionContenedor(connection, tran, grupoExpedicionCamion);

            ValidarPuedeAsignarContenedorACamion(connection, tran);

            ValidarIsFacturadoContenedor(connection, tran, camion.IsFacturado() ? "S" : "N");

            contenedores = GetCargasContenedores(connection, tran, contenedores);

            AddPrepaparacionTemp(connection, tran, contenedores);

            var contenedoresCompartidos = GetCargasConContenedoresCompartidos(connection, tran);

            var cargasContenedoresUpdate = contenedores.Join(contenedoresCompartidos,
                 cp => cp.Carga,
                 cmpp => cmpp,
                 (cp, cmpp) => cp).ToList();

            var cantidadCargasGenerar = cargasContenedoresUpdate.Count();

            if (cantidadCargasGenerar > 0)
            {
                var newCargas = _cargaRepository.GetNewNumeroCargas(cantidadCargasGenerar, connection, tran);

                foreach (var cargaContenedorUpdate in cargasContenedoresUpdate)
                {
                    var newCarga = newCargas[0];
                    cargaContenedorUpdate.CargaDestino = newCarga;
                    newCargas.RemoveAt(0);
                }
            }

            UpdatePrepaparacionCargaTemp(connection, tran, cargasContenedoresUpdate);

            var detalleContenedoresUpdate = cargasContenedoresUpdate.Select(x => new ContenedorAsociarUnidad
            {
                Preparacion = x.Preparacion,
                Cliente = x.Cliente,
                Carga = x.Carga,
                Empresa = x.Empresa,
                CargaDestino = x.CargaDestino,
                Descripcion = $"Contenedor: " + x.Contenedor + ". Generada por Armado de camión.",
                FechaAlta = DateTime.Now,
                Transaccion = _uow.GetTransactionNumber(),
                Ruta = x.Ruta
            }).ToList();

            CopiarCargaUpdatePrepaparacionCarga(connection, tran, detalleContenedoresUpdate);

            var cargasContenedoresAsociar = contenedores
               .GroupBy(x => new { x.Cliente, x.Carga, x.CargaDestino, x.Empresa })
               .Select(x => new ContenedorAsociarUnidad
               {
                   Cliente = x.Key.Cliente,
                   Empresa = x.Key.Empresa,
                   Carga = x.Key.CargaDestino ?? x.Key.Carga,
                   FechaAlta = DateTime.Now,
                   Camion = camion.Id,
                   TipoModalidadArmado = TipoModalidadArmado.Contenedor,
                   IdCarga = "S",
                   FlSyncRealizada = "N",
               })
               .ToList();

            List<CargaCamion> cargas = AddCargaCamion(connection, tran, cargasContenedoresAsociar);

            camion.Cargas.AddRange(cargas);

            RemovePrepaparacionTemp(connection, tran, contenedorTemp);

            RemovePrepaparacionCargaTemp(connection, tran, contenedorTemp);
        }

        public virtual void DesarmarContenedorCamion(IUnitOfWork _uow, Camion camion, List<ContenedorAsociarUnidad> contenedores)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var contenedorTemp = contenedores;

            AddPrepaparacionCargaTemp(connection, tran, contenedores);

            contenedores = GetCargasContenedores(connection, tran, contenedores);

            AddPrepaparacionTemp(connection, tran, contenedores);

            var contenedoresCompartidos = GetCargasConContenedoresCompartidos(connection, tran);

            var cargasContenedoresUpdate = contenedores.Join(contenedoresCompartidos,
                 cp => cp.Carga,
                 cmpp => cmpp,
                 (cp, cmpp) => cp).ToList();

            var cantidadCargasGenerar = cargasContenedoresUpdate.Count();

            if (cantidadCargasGenerar > 0)
            {
                var newCargas = _cargaRepository.GetNewNumeroCargas(cantidadCargasGenerar, connection, tran);

                foreach (var cargaPedidoUpdate in cargasContenedoresUpdate)
                {
                    var newCarga = newCargas[0];
                    cargaPedidoUpdate.CargaDestino = newCarga;
                    newCargas.RemoveAt(0);
                }
            }

            UpdatePrepaparacionCargaTemp(connection, tran, cargasContenedoresUpdate);

            var detallePedìdoUpdate = cargasContenedoresUpdate.Select(x => new ContenedorAsociarUnidad
            {
                Preparacion = x.Preparacion,
                Cliente = x.Cliente,
                Carga = x.Carga,
                Empresa = x.Empresa,
                CargaDestino = x.CargaDestino,
                Descripcion = $"Contenedor: " + x.Contenedor + ". Generada por Armado de camión.",
                FechaAlta = DateTime.Now,
                Transaccion = _uow.GetTransactionNumber(),
                Ruta = x.Ruta
            }).ToList();

            CopiarCargaUpdatePrepaparacionCarga(connection, tran, detallePedìdoUpdate);

            var cargasPedidosEliminar = contenedores.Except(cargasContenedoresUpdate).Select(x => new ContenedorAsociarUnidad
            {
                Preparacion = x.Preparacion,
                Cliente = x.Cliente,
                Carga = x.Carga,
                Empresa = x.Empresa,
                Camion = camion.Id
            }).ToList();

            RemoveCargasCamion(connection, tran, camion.Id, cargasPedidosEliminar);

            var cargasSinDetalles = RemoveCargasContenedorSinDetallePreparacionCamion(connection, tran, camion.Id);

            var cargas = camion.Cargas.Join(cargasPedidosEliminar,
                 cp => new { Carga = cp.Carga, cp.Cliente },
                 cpe => new { cpe.Carga, cpe.Cliente },
                 (cp, cpe) => cp).ToList();

            foreach (var carga in cargas)
                camion.Cargas.Remove(carga);

            cargas = camion.Cargas.Join(cargasSinDetalles,
                cp => new { Carga = (long?)cp.Carga, cp.Cliente },
                cpe => new { cpe.Carga, cpe.Cliente },
                (cp, cpe) => cp).ToList();

            foreach (var carga in cargas)
                camion.Cargas.Remove(carga);

            RemovePrepaparacionTemp(connection, tran, contenedorTemp);

            RemovePrepaparacionCargaTemp(connection, tran, contenedorTemp);
        }

        public virtual void DesasociarContenedorCamion(IUnitOfWork _uow, Camion camion, List<CargaAsociarUnidad> cargas)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var cargasTemp = cargas;

            AddPrepaparacionCargaTemp(connection, tran, cargas);

            List<CargaCamion> cargasPedidosEliminar = GetCargasCamion(connection, tran, camion.Id);

            RemoveCargasCamion(connection, tran, camion.Id, cargasPedidosEliminar);

            foreach (var carga in cargasPedidosEliminar)
                camion.Cargas.Remove(camion.Cargas.FirstOrDefault(x => x.Carga == carga.Carga && x.Cliente == carga.Cliente));

            RemovePrepaparacionCargaTemp(connection, tran, cargasTemp);

        }

        #region Get

        public virtual string GetGrupoExpedicion(DbConnection connection, DbTransaction tran, int cdCamion)
        {
            var sql = @$"SELECT 
                            te.CD_GRUPO_EXPEDICION
                        FROM T_CLIENTE_CAMION cc
                        INNER JOIN T_DET_PICKING dp ON cc.NU_CARGA = dp.NU_CARGA
                        INNER JOIN T_PEDIDO_SAIDA ped ON dp.NU_PEDIDO = ped.NU_PEDIDO AND dp.CD_CLIENTE = dp.CD_CLIENTE AND dp.CD_EMPRESA = dp.CD_EMPRESA
                        INNER JOIN T_TIPO_EXPEDICION te ON ped.TP_EXPEDICION = te.TP_EXPEDICION
                        WHERE cc.CD_CAMION= :cdCamion
                        GROUP BY 
                            te.CD_GRUPO_EXPEDICION";

            return _dapper.Query<string>(connection, sql, param: new { cdCamion }, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual List<ContenedorAsociarUnidad> GetCargasContenedores(DbConnection connection, DbTransaction tran, List<ContenedorAsociarUnidad> contenedores)
        {

            string sql = @$"SELECT 
                            dp.NU_CONTENEDOR  as Contenedor,
                            dp.NU_PREPARACION as Preparacion,
                            dp.CD_EMPRESA as Empresa,
                            dp.CD_CLIENTE as Cliente,
                            dp.NU_CARGA as Carga,
                            c.CD_ROTA as Ruta
                        FROM T_CARGA_TEMP pst
                        INNER JOIN T_DET_PICKING  dp ON dp.NU_PREPARACION = pst.NU_PREPARACION
                            AND dp.NU_CONTENEDOR = pst.NU_CONTENEDOR
                        INNER JOIN T_CARGA c ON c.NU_CARGA = dp.NU_CARGA
                        GROUP BY dp.NU_CONTENEDOR,
                            dp.NU_PREPARACION,
                            dp.CD_EMPRESA,
                            dp.CD_CLIENTE,
                            dp.NU_CARGA,
                            c.CD_ROTA";

            return _dapper.Query<ContenedorAsociarUnidad>(connection, sql, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<long> GetCargasConContenedoresCompartidos(DbConnection connection, DbTransaction tran)
        {

            string sql = @$"SELECT ff.NU_CARGA 
                             FROM (
                                    SELECT dp.NU_CARGA,
                                            count(dp.NU_CARGA) CANTIDAD
                                        FROM 
                                            (SELECT 
                                                dp.NU_CONTENEDOR,
                                                dp.NU_PREPARACION,
                                                dp.NU_CARGA
                                            FROM T_DET_PICKING_TEMP pst
                                            INNER JOIN T_DET_PICKING  dp ON dp.NU_PREPARACION = pst.NU_PREPARACION
                                                AND dp.NU_CARGA = pst.NU_CARGA
                                            GROUP BY dp.NU_CONTENEDOR,
                                                dp.NU_PREPARACION,
                                                dp.NU_CARGA
                                        ) dp 
                                        GROUP BY dp.NU_CARGA 
                                    ) ff
                             WHERE CANTIDAD > 1";

            return _dapper.Query<long>(connection, sql, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual void ValidarIsFacturadoContenedor(DbConnection connection, DbTransaction tran, string camionfacturado)
        {
            var parameter = new DynamicParameters(new
            {
                CAMION_FACTURADO = camionfacturado
            });

            string sql = @$"SELECT 
                            ct.NU_CONTENEDOR,
                            c.TP_CONTENEDOR,
                            c.ID_EXTERNO_CONTENEDOR 
                        FROM T_CARGA_TEMP  ct
                        INNER JOIN T_CONTENEDOR  c ON  c.NU_PREPARACION = ct.NU_PREPARACION
                            AND c.NU_CONTENEDOR = ct.NU_CONTENEDOR
                            AND CASE WHEN c.CD_SITUACAO is null THEN 'N' ELSE 'N'  END <> :CAMION_FACTURADO";

            var contenedorConProblema = _dapper.Query<(int? NuContenedor, string TipoContenedor, string IdExterno)>(connection, sql, param: parameter, commandType: CommandType.Text, transaction: tran).FirstOrDefault();

            if (contenedorConProblema.NuContenedor != null)
                throw new ValidationFailedException("WEXP011_Sec0_Error_Er004_ContenedorSituacionInvalida", new string[] { contenedorConProblema.TipoContenedor, contenedorConProblema.IdExterno });
        }

        public virtual void ValidarPuedeAsignarContenedorACamion(DbConnection connection, DbTransaction tran)
        {
            var parameter = new DynamicParameters(new
            {
                CD_SITUACAO = SituacionDb.ContenedorEnPreparacion
            });

            string sql = @$"SELECT 
                            ct.NU_CONTENEDOR,
                            c.TP_CONTENEDOR,
                            c.ID_EXTERNO_CONTENEDOR
                        FROM T_CARGA_TEMP  ct
                        INNER JOIN T_CONTENEDOR  c ON  c.NU_PREPARACION = ct.NU_PREPARACION
                            AND c.NU_CONTENEDOR = ct.NU_CONTENEDOR
                            AND c.CD_SITUACAO <> :CD_SITUACAO";

            var contenedorConProblema = _dapper.Query<(int? NuContenedor, string TipoContenedor, string IdExterno)>(connection, sql, param: parameter, commandType: CommandType.Text, transaction: tran).FirstOrDefault();

            if (contenedorConProblema.NuContenedor != null)
                throw new ValidationFailedException("WEXP011_Sec0_Error_Er004_ContenedorSituacionInvalida", new string[] { contenedorConProblema.TipoContenedor, contenedorConProblema.IdExterno });
        }

        public virtual void ValidarGrupoExpedicionContenedor(DbConnection connection, DbTransaction tran, string grupoExpedicionCamion)
        {
            if (!string.IsNullOrEmpty(grupoExpedicionCamion))
            {
                var parameter = new DynamicParameters(new
                {
                    CD_GRUPO_EXPEDICION = grupoExpedicionCamion
                });

                string sql = @$"SELECT 
                            ct.NU_CONTENEDOR 
                        FROM T_CARGA_TEMP  ct
                        INNER JOIN V_EXP011_CONTENEDOR_CAMION  ecc ON  ecc.NU_PREPARACION = ct.NU_PREPARACION
                            AND ecc.CD_EMPRESA = ct.CD_EMPRESA
                            AND ecc.CD_CLIENTE = ct.CD_CLIENTE
                            AND ecc.NU_CARGA = ct.NU_CARGA
                            AND ecc.NU_CONTENEDOR = ct.NU_CONTENEDOR
                        WHERE ecc.CD_GRUPO_EXPEDICION is not null 
                            AND  ecc.CD_GRUPO_EXPEDICION  <> :CD_GRUPO_EXPEDICION";

                var cargaConProblema = _dapper.Query<long?>(connection, sql, param: parameter, commandType: CommandType.Text, transaction: tran).FirstOrDefault();

                if (cargaConProblema != null)
                    throw new ValidationFailedException("WEXP_grid1_Error_CargaConDiferenteGrupoExpedicionCamion", new string[] { Convert.ToString(cargaConProblema) });

            }
        }

        #endregion

        #region Remove

        public virtual List<PedidoAsociarUnidad> RemoveCargasContenedorSinDetallePreparacionCamion(DbConnection connection, DbTransaction tran, int camion)
        {
            var sqlDetallesPicking = $@"
                SELECT
                    dpt.CD_CLIENTE as Cliente,
                    dpt.NU_CARGA as Carga,
                    :Camion as Camion,
                    dpt.CD_EMPRESA as Empresa
                FROM T_DET_PICKING_TEMP dpt
                LEFT JOIN T_DET_PICKING dp ON dpt.NU_PREPARACION = dp.NU_PREPARACION
                    AND dpt.CD_CLIENTE = dp.CD_CLIENTE
                    AND dpt.CD_EMPRESA = dp.CD_EMPRESA 
                    AND dpt.NU_CARGA = dp.NU_CARGA
                WHERE dp.NU_PREPARACION is null
                GROUP BY dpt.CD_CLIENTE ,
                         dpt.NU_CARGA,
                         dpt.CD_EMPRESA";

            var cargasSinDetallePreparacion = _dapper.Query<PedidoAsociarUnidad>(connection, sqlDetallesPicking, new { Camion = camion }, transaction: tran).ToList();

            _dapper.BulkDelete(connection, tran, cargasSinDetallePreparacion, "T_CLIENTE_CAMION", new Dictionary<string, Func<PedidoAsociarUnidad, object>>
            {
                { "CD_CAMION", x => x.Camion},
                { "CD_CLIENTE", x => x.Cliente},
                { "CD_EMPRESA", x => x.Empresa},
                { "NU_CARGA", x => x.Carga},
            });

            return cargasSinDetallePreparacion;
        }

        public virtual void RemoveCargasCamion(DbConnection connection, DbTransaction tran, int id, List<ContenedorAsociarUnidad> cargasPedidosEliminar)
        {
            _dapper.BulkDelete(connection, tran, cargasPedidosEliminar, "T_CLIENTE_CAMION", new Dictionary<string, Func<ContenedorAsociarUnidad, object>>
            {
                { "CD_CAMION", x => x.Camion},
                { "CD_CLIENTE", x => x.Cliente},
                { "CD_EMPRESA", x => x.Empresa},
                { "NU_CARGA", x => x.Carga},
            });
        }

        public virtual void RemovePrepaparacionTemp(DbConnection connection, DbTransaction tran, List<ContenedorAsociarUnidad> pedidosAsociar)
        {
            _dapper.BulkDelete(connection, tran, pedidosAsociar, "T_DET_PICKING_TEMP", new Dictionary<string, Func<ContenedorAsociarUnidad, object>>
            {
                { "NU_CONTENEDOR", x => x.Contenedor},
                { "CD_EMPRESA", x => x.Empresa},
                { "CD_CLIENTE", x => x.Cliente},
                { "NU_PREPARACION", x => x.Preparacion},
                { "NU_CARGA", x => x.Carga}
            });

        }

        public virtual void RemovePrepaparacionCargaTemp(DbConnection connection, DbTransaction tran, List<ContenedorAsociarUnidad> pedidosAsociar)
        {
            _dapper.BulkDelete(connection, tran, pedidosAsociar, "T_CARGA_TEMP", new Dictionary<string, Func<ContenedorAsociarUnidad, object>>
            {
                { "NU_CONTENEDOR", x => x.Contenedor},
                { "CD_EMPRESA", x => x.Empresa},
                { "CD_CLIENTE", x => x.Cliente},
                { "NU_PREPARACION", x => x.Preparacion},
                { "NU_CARGA", x => x.Carga},
                { "CD_GRUPO_EXPEDICION", x => x.GrupoExpedicion},
            });

        }

        #endregion

        #region Update

        public virtual void UpdatePrepaparacionCargaTemp(DbConnection connection, DbTransaction tran, List<ContenedorAsociarUnidad> pedidosAsociar)
        {
            _dapper.BulkUpdate(connection, tran, pedidosAsociar, "T_DET_PICKING_TEMP", new Dictionary<string, Func<ContenedorAsociarUnidad, ColumnInfo>>
            {
                { "NU_CARGA_DEST", x => new ColumnInfo(x.CargaDestino, DbType.Int64)}
            }, new Dictionary<string, Func<ContenedorAsociarUnidad, ColumnInfo>>
            {
                { "NU_PREPARACION", x => new ColumnInfo(x.Preparacion)},
                { "NU_CONTENEDOR", x => new ColumnInfo(x.Contenedor)},
                { "NU_CARGA", x => new ColumnInfo(x.Carga)},
            });
        }

        #endregion

        #region Add

        public virtual void AddPrepaparacionCargaTemp(DbConnection connection, DbTransaction tran, List<ContenedorAsociarUnidad> contenedorAsociar)
        {
            _dapper.BulkInsert(connection, tran, contenedorAsociar, "T_CARGA_TEMP", new Dictionary<string, Func<ContenedorAsociarUnidad, ColumnInfo>>
            {
                { "NU_CONTENEDOR", x => new ColumnInfo(x.Contenedor)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "NU_PREPARACION", x => new ColumnInfo(x.Preparacion)},
                { "NU_CARGA", x => new ColumnInfo(x.Carga)},
                { "CD_GRUPO_EXPEDICION", x => new ColumnInfo(x.GrupoExpedicion)},
            });

        }

        public virtual void AddPrepaparacionTemp(DbConnection connection, DbTransaction tran, List<ContenedorAsociarUnidad> contenedorAsociar)
        {
            _dapper.BulkInsert(connection, tran, contenedorAsociar, "T_DET_PICKING_TEMP", new Dictionary<string, Func<ContenedorAsociarUnidad, ColumnInfo>>
            {
                { "NU_CONTENEDOR", x => new ColumnInfo(x.Contenedor)},
                { "CD_ROTA", x => new ColumnInfo(x.Ruta)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "NU_PREPARACION", x => new ColumnInfo(x.Preparacion)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "NU_CARGA", x => new ColumnInfo(x.Carga)},
            });

        }

        public virtual void CopiarCargaUpdatePrepaparacionCarga(DbConnection connection, DbTransaction tran, List<ContenedorAsociarUnidad> lineas)
        {
            if (lineas.Count > 0)
            {
                _dapper.BulkInsert(connection, tran, lineas, "T_CARGA", new Dictionary<string, Func<ContenedorAsociarUnidad, ColumnInfo>>
                {
                    { "NU_CARGA", x => new ColumnInfo(x.CargaDestino)},
                    { "DS_CARGA", x => new ColumnInfo(x.Descripcion)},
                    { "CD_ROTA", x => new ColumnInfo(x.Ruta)},
                    { "DT_ADDROW", x => new ColumnInfo(x.FechaAlta)},
                });
                var param = new DynamicParameters(new
                {
                    NU_TRANSACCION = lineas.FirstOrDefault()?.Transaccion,
                    DT_UPDROW = DateTime.Now,
                });

                var alias = "dp";
                var from = @"
                            T_DET_PICKING dp
                            INNER JOIN (
                                SELECT
                                    dpt.NU_PREPARACION,
                                    dpt.NU_CONTENEDOR,
                                    dpt.CD_CLIENTE,
                                    dpt.NU_CARGA as NU_CARGA_TEMP,
                                    MAX(dpt.NU_CARGA_DEST) as NU_CARGA_DEST
                                FROM T_DET_PICKING_TEMP dpt
                                WHERE dpt.NU_CARGA_DEST is not null
                                GROUP BY
                                    dpt.NU_PREPARACION,
                                    dpt.NU_CONTENEDOR,
                                    dpt.CD_CLIENTE,
                                    dpt.NU_CARGA
                            ) cc ON cc.NU_PREPARACION = dp.NU_PREPARACION 
                                AND cc.NU_CONTENEDOR  = dp.NU_CONTENEDOR 
                                AND cc.CD_CLIENTE  = dp.CD_CLIENTE 
                                AND cc.NU_CARGA_TEMP  = dp.NU_CARGA";
                var set = @"
                    NU_CARGA = NU_CARGA_DEST,
                    NU_TRANSACCION = :NU_TRANSACCION,
                    DT_UPDROW = :DT_UPDROW";
                var where = "";

                _dapper.ExecuteUpdate(connection, alias, from, set, where, param: param, commandType: CommandType.Text, transaction: tran);
            }
        }

        public virtual List<CargaCamion> AddCargaCamion(DbConnection connection, DbTransaction tran, List<ContenedorAsociarUnidad> cargasContenedor)
        {
            _dapper.BulkInsert(connection, tran, cargasContenedor, "T_CLIENTE_CAMION", new Dictionary<string, Func<ContenedorAsociarUnidad, ColumnInfo>>
             {
                { "CD_CAMION", x => new ColumnInfo(x.Camion)},
                { "NU_CARGA", x => new ColumnInfo(x.Carga)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "DT_ADDROW", x => new ColumnInfo(x.FechaAlta)},
                { "TP_MODALIDAD", x => new ColumnInfo(x.TipoModalidadArmado)},
                { "ID_CARGAR", x => new ColumnInfo(x.IdCarga)},
                { "FL_SYNC_REALIZADA", x => new ColumnInfo(x.FlSyncRealizada)},
            });

            List<CargaCamion> cargasCamion = new List<CargaCamion>();
            foreach (var cargaCamion in cargasContenedor)
            {
                cargasCamion.Add(new CargaCamion
                {
                    Camion = cargaCamion.Camion,
                    Carga = cargaCamion.Carga,
                    Cliente = cargaCamion.Cliente,
                    Empresa = cargaCamion.Empresa,
                    FechaAlta = DateTime.Now,
                    TipoModalidad = TipoModalidadArmado.Pedido,
                    SincronizacionRealizada = false,
                    SincronizacionRealizadaId = "N",
                    IdCargar = "S"
                });
            }

            return cargasCamion;
        }

        #endregion

        #endregion

        public virtual bool RequiereFacturacion(int cdCamion)
        {
            string sql = @$"SELECT 
                            FL_REQUIERE_FACTURACION
                        FROM V_CAMION_REQUIERE_FACTURACION
                        WHERE CD_CAMION = :cdCamion";

            return (_dapper.Query<string>(_context.Database.GetDbConnection(), sql, param: new { cdCamion = cdCamion }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction())
                .FirstOrDefault() ?? "N") == "S";

        }

        #endregion
    }
}
