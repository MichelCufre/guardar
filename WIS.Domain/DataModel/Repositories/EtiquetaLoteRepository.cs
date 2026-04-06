using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.General.API;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Interfaces;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;
using WIS.Persistence.General;

namespace WIS.Domain.DataModel.Repositories
{
    public class EtiquetaLoteRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly EtiquetaLoteMapper _mapper;
        protected readonly IDapper _dapper;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly ParametroRepository _parametroRepository;

        public EtiquetaLoteRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new EtiquetaLoteMapper();
            this._dapper = dapper;
            this._parametroRepository = new ParametroRepository(_context, cdAplicacion, userId, dapper);
        }

        #region Any

        public virtual bool AnyDetalleEtiquetaConStock(int numero)
        {
            return _context.T_DET_ETIQUETA_LOTE
               .Any(x => x.NU_ETIQUETA_LOTE == numero && x.QT_PRODUTO > 0);
        }

        public virtual bool AnyDetalleEtiquetaConStockRecibido(int nuAgenda)
        {
            return _context.T_DET_ETIQUETA_LOTE
                .Include("T_ETIQUETA_LOTE")
                .Any(x => x.T_ETIQUETA_LOTE.NU_AGENDA == nuAgenda && x.QT_PRODUTO_RECIBIDO > 0);
        }

        #endregion

        #region Get

        public virtual LogEtiqueta GetFirstLogEtiqueta(int nuAgenda, string tpMovimiento)
        {
            return this._mapper.MapToObject(this._context.T_LOG_ETIQUETA
                .AsNoTracking()
                .Where(l => l.NU_AGENDA == nuAgenda
                    && l.TP_MOVIMIENTO == tpMovimiento)
                .OrderBy(x => x.NU_LOG_ETIQUETA)
                .FirstOrDefault());
        }

        public virtual EtiquetaLote GetEtiquetaLote(int etiquetaLote)
        {
            return this._mapper.MapToObject(this._context.T_ETIQUETA_LOTE
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_ETIQUETA_LOTE == etiquetaLote));
        }

        public virtual EtiquetaLote GetEtiquetaLoteActiva(string tpEtiqueta, string nuExterno, bool filtroPorExternoEtiqueta = true)
        {
            if (filtroPorExternoEtiqueta)
            {
                return this._context.T_ETIQUETA_LOTE
                   .Join(this._context.T_ETIQUETAS_EN_USO
                           .Where(x => x.TP_ETIQUETA == tpEtiqueta && x.NU_EXTERNO_ETIQUETA == nuExterno),
                       e => new { e.NU_EXTERNO_ETIQUETA, e.TP_ETIQUETA, e.NU_ETIQUETA_LOTE },
                       eeu => new { eeu.NU_EXTERNO_ETIQUETA, eeu.TP_ETIQUETA, eeu.NU_ETIQUETA_LOTE },
                       (e, eeu) => e
                   )
                   .AsNoTracking()
                   .Select(e => this._mapper.MapToObject(e))
                   .FirstOrDefault();

            }
            else
            {
                return this._context.T_ETIQUETA_LOTE
                .Join(this._context.T_ETIQUETAS_EN_USO,
                    e => new { e.NU_EXTERNO_ETIQUETA, e.TP_ETIQUETA, e.NU_ETIQUETA_LOTE },
                    eeu => new { eeu.NU_EXTERNO_ETIQUETA, eeu.TP_ETIQUETA, eeu.NU_ETIQUETA_LOTE },
                    (e, eeu) => e
                ).Where(x => x.TP_ETIQUETA == tpEtiqueta && x.CD_BARRAS == nuExterno)
                .AsNoTracking()
                .Select(e => this._mapper.MapToObject(e))
                .FirstOrDefault();
            }

        }

        public virtual EtiquetaLote GetEtiquetaLoteEmpresaAsociadas(string tpEtiqueta, string nuExterno)
        {
            return this._context.T_ETIQUETA_LOTE.
                Join(this._context.T_AGENDA,
                    e => e.NU_AGENDA,
                    ef => ef.NU_AGENDA,
                    (e, ef) => e)
                 .Join(this._context.T_EMPRESA_FUNCIONARIO.Where(ef => ef.USERID == this._userId),
                    e => e.T_AGENDA.CD_EMPRESA,
                    ef => ef.CD_EMPRESA,
                    (e, ef) => e)
                .Join(this._context.T_ETIQUETAS_EN_USO
                        .Where(x => x.TP_ETIQUETA == tpEtiqueta && x.NU_EXTERNO_ETIQUETA == nuExterno),
                    e => new { e.NU_EXTERNO_ETIQUETA, e.TP_ETIQUETA, e.NU_ETIQUETA_LOTE },
                    eeu => new { eeu.NU_EXTERNO_ETIQUETA, eeu.TP_ETIQUETA, eeu.NU_ETIQUETA_LOTE },
                    (e, eeu) => e
                )
                .AsNoTracking()
                .Select(e => this._mapper.MapToObject(e))
                .FirstOrDefault();
        }

        public virtual bool AnyEtiquetaLote(string tpEtiqueta, string nuExterno)
        {
            return this._context.T_ETIQUETA_LOTE
                .Join(this._context.T_ETIQUETAS_EN_USO
                        .Where(x => x.TP_ETIQUETA == tpEtiqueta && x.NU_EXTERNO_ETIQUETA == nuExterno),
                    e => new { e.NU_EXTERNO_ETIQUETA, e.TP_ETIQUETA },
                    eeu => new { eeu.NU_EXTERNO_ETIQUETA, eeu.TP_ETIQUETA },
                    (e, eeu) => e
                )
                .AsNoTracking()
                .Any();
        }

        public virtual List<EtiquetaLoteDetalle> GetDetallesDeEtiquetaLoteAgenda(int idAgenda)
        {
            List<EtiquetaLoteDetalle> etiquetas = new List<EtiquetaLoteDetalle>();

            List<T_DET_ETIQUETA_LOTE> entities = this._context.T_DET_ETIQUETA_LOTE
               .Include("T_ETIQUETA_LOTE")
               .AsNoTracking()
               .Where(del => del.T_ETIQUETA_LOTE.NU_AGENDA == idAgenda)
               .ToList();

            foreach (var entity in entities)
            {
                etiquetas.Add(this._mapper.MapDetalleConCabezalToObject(entity));
            }

            return etiquetas;
        }

        public virtual List<EtiquetaEnUso> GetEtiquetaLoteEnUso(IEnumerable<Contenedor> Contenedores)
        {

            IEnumerable<EtiquetaEnUso> resultado = new List<EtiquetaEnUso>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_ETIQUETAS_EN_USO_TEMP (NU_EXTERNO_ETIQUETA,TP_ETIQUETA) VALUES (:Numero,:TipoContenedor)";
                    _dapper.Execute(connection, sql, Contenedores, transaction: tran);

                    sql = @"SELECT 
                        P.NU_EXTERNO_ETIQUETA AS  NumeroExterno,
                        P.TP_ETIQUETA AS TipoEtiqueta,
                        P.NU_ETIQUETA_LOTE AS Numero
                        FROM T_ETIQUETAS_EN_USO P 
                        INNER JOIN T_ETIQUETAS_EN_USO_TEMP T ON P.NU_EXTERNO_ETIQUETA = T.NU_EXTERNO_ETIQUETA AND P.TP_ETIQUETA = T.TP_ETIQUETA
                        ";

                    resultado = _dapper.Query<EtiquetaEnUso>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado.ToList();
        }

        public virtual EtiquetaLoteDetalle GetEtiquetaLoteDetalle(int numero, string producto, int empresa, decimal faixa, string identificador)
        {
            return _mapper.MapDetalleConCabezalToObject(this._context.T_DET_ETIQUETA_LOTE
               .Where(d => d.NU_ETIQUETA_LOTE == numero
                   && d.CD_PRODUTO == producto
                   && d.CD_EMPRESA == empresa
                   && d.CD_FAIXA == faixa
                   && d.NU_IDENTIFICADOR == identificador)
               .FirstOrDefault());
        }

        public virtual List<EtiquetaLoteDetalle> GetDetalles(int nuEtiquetaLote)
        {
            return this._context.T_DET_ETIQUETA_LOTE
               .AsNoTracking()
               .Where(del => del.NU_ETIQUETA_LOTE == nuEtiquetaLote)
               .Select(del => this._mapper.MapDetalleConCabezalToObject(del))
               .ToList();
        }

        public virtual List<EtiquetaLoteDetalle> GetDetalles(int nuEtiquetaLote, int empresa, string producto, string lote = null)
        {
            return this._context.T_DET_ETIQUETA_LOTE
               .AsNoTracking()
               .Where(del => del.NU_ETIQUETA_LOTE == nuEtiquetaLote
                    && del.CD_EMPRESA == empresa
                    && del.CD_PRODUTO == producto
                    && (lote == null || del.NU_IDENTIFICADOR == lote))
               .Select(del => this._mapper.MapDetalleConCabezalToObject(del))
               .ToList();
        }

        public virtual EtiquetaLoteDetalle GetEtiquetaLoteDetalle(string producto, decimal faixa, int empresa, string identificador, int numeroEtiquetaLote)
        {
            T_DET_ETIQUETA_LOTE entity = this._context.T_DET_ETIQUETA_LOTE
                .AsNoTracking()
                .FirstOrDefault(d => d.NU_ETIQUETA_LOTE == numeroEtiquetaLote
                    && d.CD_PRODUTO == producto
                    && d.CD_FAIXA == faixa
                    && d.CD_EMPRESA == empresa
                    && d.NU_IDENTIFICADOR == identificador);

            return this._mapper.MapDetalleConCabezalToObject(entity);
        }

        public virtual int GetProximoNumeroEtiquetaEntrada()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_ETIQUETA_ENTRADA);
        }

        public virtual IEnumerable<EtiquetaLote> GetEtiquetasCriterios(IEnumerable<CriterioControlCalidadAPI> criteriosEtiqueta)
        {
            IEnumerable<EtiquetaLote> resultado;

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    _dapper.BulkInsert(connection, tran, criteriosEtiqueta, "T_STOCK_PREDIO_TEMP", new Dictionary<string, Func<CriterioControlCalidadAPI, ColumnInfo>>
                    {
                        { "NU_PREDIO", x => new ColumnInfo(x.Predio)},
                        { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                        { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                        { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Lote)},
                        { "CD_FAIXA", x => new ColumnInfo(x.Faixa)}
                    });

                    string sql = @"
                        SELECT DISTINCT
                             ETL.CD_BARRAS AS CodigoBarras,
                             ETL.CD_CLIENTE AS Cliente,
                             ETL.CD_ENDERECO AS IdUbicacion,
                             ETL.CD_ENDERECO_MOVTO_PARCIAL AS UbicacionMovimiento,
                             ETL.CD_ENDERECO_SUGERIDO AS IdUbicacionSugerida,
                             ETL.CD_FUNC_ALMACENAMIENTO AS FuncionarioAlmacenamiento,
                             ETL.CD_FUNC_RECEPCION AS FuncionarioRecepcion,
                             ETL.CD_GRUPO AS CodigoGrupo,
                             ETL.CD_PALLET AS CodigoPallet,
                             ETL.CD_SITUACAO AS Estado,
                             ETL.CD_SITUACAO_PALLET AS EstadoPallet,
                             ETL.DT_ALMACENAMIENTO AS FechaAlmacenamiento,
                             ETL.DT_RECEPCION AS FechaRecepcion,
                             ETL.DT_UPDROW AS FechaModificacion,
                             ETL.NU_AGENDA AS NumeroAgenda,
                             ETL.NU_ETIQUETA_LOTE AS Numero,
                             ETL.NU_EXTERNO_ETIQUETA AS NumeroExterno,
                             ETL.NU_LPN AS NroLpn,
                             ETL.NU_TRANSACCION AS NumeroTransaccion,
                             ETL.NU_TRANSACCION_DELETE AS NumeroTransaccionDelete,
                             ETL.NU_UNIDAD_TRANSPORTE AS CodigoUnidadTransporte,
                             ETL.TP_ETIQUETA AS TipoEtiqueta,
                             EE.NU_PREDIO AS Predio
                        FROM
                            T_ETIQUETA_LOTE ETL
                            INNER JOIN T_DET_ETIQUETA_LOTE ETD ON
                                ETL.NU_ETIQUETA_LOTE = ETD.NU_ETIQUETA_LOTE
                            INNER JOIN T_ENDERECO_ESTOQUE EE ON
                                ETL.CD_ENDERECO = EE.CD_ENDERECO                    
                            LEFT JOIN T_LPN LPN ON
                                ETL.NU_LPN = lpn.NU_LPN
                            INNER JOIN T_STOCK_PREDIO_TEMP TEMP ON
                                TEMP.NU_PREDIO = EE.NU_PREDIO AND 
                                TEMP.CD_PRODUTO = ETD.CD_PRODUTO AND
                                TEMP.CD_EMPRESA = ETD.CD_EMPRESA AND
                                TEMP.CD_FAIXA = ETD.CD_FAIXA AND
                                TEMP.NU_IDENTIFICADOR = ETD.NU_IDENTIFICADOR
                        WHERE
                            ETL.CD_SITUACAO = 23 AND
                            ETD.QT_PRODUTO > 0
                    ";

                    resultado = _dapper.Query<EtiquetaLote>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        #endregion

        #region Add

        public virtual int AddLogEtiqueta(LogEtiqueta etiquetaLog)
        {
            T_LOG_ETIQUETA entity = this._mapper.MapToEntity(etiquetaLog);

            entity.NU_LOG_ETIQUETA = this._context.GetNextSequenceValueInt(_dapper, "S_LOG_ETIQUETA");

            this._context.T_LOG_ETIQUETA.Add(entity);

            return entity.NU_LOG_ETIQUETA;
        }

        public virtual void AddDetalleEtiqueta(EtiquetaLoteDetalle detEtiqueta)
        {
            T_DET_ETIQUETA_LOTE entity = this._mapper.MapToEntity(detEtiqueta);

            this._context.T_DET_ETIQUETA_LOTE.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateEtiquetaLote(EtiquetaLote etiqueta)
        {
            etiqueta.FechaModificacion = DateTime.Now;

            var entity = this._mapper.MapToEntity(etiqueta);
            var attachedEntity = _context.T_ETIQUETA_LOTE.Local
               .FirstOrDefault(x => x.NU_ETIQUETA_LOTE == entity.NU_ETIQUETA_LOTE);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_ETIQUETA_LOTE.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateEtiquetaLoteDetalle(EtiquetaLoteDetalle etiqueta)
        {
            var entity = this._mapper.MapToEntity(etiqueta);
            var attachedEntity = _context.T_DET_ETIQUETA_LOTE.Local
                .FirstOrDefault(x => x.NU_ETIQUETA_LOTE == entity.NU_ETIQUETA_LOTE &&
                    x.CD_PRODUTO == entity.CD_PRODUTO &&
                    x.NU_IDENTIFICADOR == entity.NU_IDENTIFICADOR &&
                    x.CD_FAIXA == entity.CD_FAIXA &&
                    x.CD_EMPRESA == entity.CD_EMPRESA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_DET_ETIQUETA_LOTE.Attach(entity);
                _context.Entry<T_DET_ETIQUETA_LOTE>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateEtiquetaLote(EntityChanges<EtiquetaLote> records)
        {
            foreach (var updatedRecord in records.UpdatedRecords)
            {
                this.UpdateEtiquetaLote(updatedRecord);
            }
        }

        #endregion

        #region Remove

        #endregion

        #region Dapper

        public virtual List<EtiquetaLoteDetalle> GetEtiquetaLoteDetalle(List<EtiquetaEnUso> etiquetasEnUso)
        {
            IEnumerable<EtiquetaLoteDetalle> resultado = new List<EtiquetaLoteDetalle>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_ETIQUETA_LOTE_TEMP (NU_ETIQUETA_LOTE) VALUES (:Numero)";
                    _dapper.Execute(connection, sql, etiquetasEnUso, transaction: tran);

                    sql = @"SELECT 
                        P.NU_ETIQUETA_LOTE AS IdEtiquetaLote,
                        P.CD_PRODUTO AS CodigoProducto,
                        P.CD_FAIXA AS Faixa,
                        P.CD_EMPRESA AS IdEmpresa,
                        P.NU_IDENTIFICADOR AS Identificador,
                        P.QT_PRODUTO_RECIBIDO AS CantidadRecibida,
                        P.QT_PRODUTO AS Cantidad,
                        P.QT_AJUSTE_RECIBIDO AS CantidadAjusteRecibido,
                        P.QT_ETIQUETA_GENERADA AS CantidadEtiquetaGenerada,
                        P.QT_ALMACENADO AS CantidadAlmacenada,
                        P.DT_FABRICACAO AS Vencimiento,
                        P.DT_ADDROW AS FechaRegistro,
                        P.DT_UPDROW AS FechaModificacion,
                        P.QT_RASTREO_PALLET AS CantidadRastreoPallet,
                        P.QT_MOVILIZADO AS CantidadMovilizado,
                        P.DT_ENTRADA AS FechaEntrada,
                        P.PS_PRODUTO_RECIBIDO AS PesoProductoRecibido,
                        P.PS_PRODUTO AS PesoProducto,
                        P.DS_MOTIVO AS Motivo,
                        P.NU_TRANSACCION AS Transaccion
                        FROM T_DET_ETIQUETA_LOTE P 
                        INNER JOIN T_ETIQUETA_LOTE_TEMP T ON P.NU_ETIQUETA_LOTE = T.NU_ETIQUETA_LOTE
                        ";

                    resultado = _dapper.Query<EtiquetaLoteDetalle>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado.ToList();
        }

        public virtual async Task<List<APITask>> GetAlmacenamientosPendientesDeConfirmacion(CancellationToken cancelToken = default)
        {
            var sql = @"SELECT 
                            ID_OPERACION AS Id,
                            DT_OPERACION AS Fecha
                        FROM V_CONFIRMACIONES_PENDIENTES
                        WHERE CD_INTERFAZ_EXTERNA = :cdInterfazExterna 
                            AND FL_HABILITADA = 'S'
                        ORDER BY 
                            DT_OPERACION ASC, 
                            ID_OPERACION ASC";

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                return _dapper.Query<APITask>(connection, sql, param: new { cdInterfazExterna = CInterfazExterna.Almacenamiento }, commandType: CommandType.Text).ToList();
            }
        }

        public virtual async Task<List<long>> GenerarInterfaces(string key, Func<int?, string> GetGrupoConsulta, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var datos = new LogEtiqueta()
                {
                    Agenda = int.Parse(key.Split('#')[0]),
                    NumeroEtiqueta = int.Parse(key.Split('#')[1])
                };

                logger.Debug($"Almacenamiento . Agenda {datos.Agenda} Nro. Etiqueta Lote: {datos.NumeroEtiqueta}");

                long nuEjecucion = -2;
                try
                {
                    using (var tran = connection.BeginTransaction())
                    {
                        var context = new BulkAnulacionContext();
                        var data = Map(datos, connection, tran, context);
                        var empresa = data.Detalles.FirstOrDefault().Empresa;

                        var interfazHabilitada = _parametroRepository.GetParamInterfazHabilitada(CInterfazExterna.Almacenamiento, empresa ?? 0, connection, tran);

                        if (!interfazHabilitada)
                        {
                            logger.Debug($"La interfaz {CInterfazExterna.Almacenamiento} no esta habilitada para la empresa {empresa}.");
                            return new List<long>();
                        }

                        var grupoConsulta = GetGrupoConsulta(empresa);
                        nuEjecucion = await CrearEjecucion(data, grupoConsulta, connection, tran);
                        context.UpdateLogEtiqueta = GetLogEtiquetaObject(context.NuLogsEtiqueta, nuEjecucion);

                        await UpdateLogEtiqueta(context.UpdateLogEtiqueta, connection, tran);
                        tran.Commit();
                    }
                }
                catch (Exception ex)
                {
                    nuEjecucion = -2;
                    logger.Error($"Almacenamiento . Agenda {datos.Agenda} Nro. Etiqueta Lote: {datos.NumeroEtiqueta} - Error: {ex}");

                    var almacenamientos = GetLogsEtiqueta(datos, connection, null);
                    if (almacenamientos != null && almacenamientos.Count() > 0)
                    {
                        var logsId = almacenamientos.Select(a => a.Id).ToList();
                        var logsEtiquetaLote = GetLogEtiquetaObject(logsId, nuEjecucion);

                        await UpdateLogEtiqueta(logsEtiquetaLote, connection, null);
                    }
                }

                return new List<long>() { nuEjecucion };
            }
        }

        public virtual AlmacenamientoResponse Map(LogEtiqueta datos, DbConnection connection, DbTransaction tran, BulkAnulacionContext context)
        {
            var etiquetaLote = GetEtiquetaLote((int)datos.NumeroEtiqueta, connection, tran);
            var agente = GetClienteAgenda((int)datos.Agenda, connection, tran);

            var almacenamientos = GetLogsEtiqueta(datos, connection, tran);

            var model = new AlmacenamientoResponse()
            {
                Agenda = etiquetaLote.NumeroAgenda,
                TipoAgente = agente.Tipo,
                CodigoAgente = agente.Codigo,
                Etiqueta = etiquetaLote.NumeroExterno,
                TipoEtiqueta = etiquetaLote.TipoEtiqueta
            };

            if (almacenamientos != null && almacenamientos.Count() > 0)
            {
                context.NuLogsEtiqueta = almacenamientos.Select(a => a.Id).ToList();

                var detalles = almacenamientos.GroupBy(a => new { a.CodigoProducto, a.Faixa, a.Identificador, a.Empresa, a.Funcionario, a.Ubicacion })
                    .Select(a => new DetalleAlmacenamientoResponse()
                    {
                        Empresa = a.Key.Empresa,
                        Producto = a.Key.CodigoProducto,
                        Faixa = a.Key.Faixa,
                        Identificador = a.Key.Identificador,
                        Vencimiento = a.Min(e => e.Vencimiento)?.ToString(CDateFormats.DATE_ONLY),
                        CantidadAlmacenada = a.Sum(e => (e.Cantidad ?? 0)),
                        CantidadDisponible = 0,//Se carga más adelante
                        Ubicacion = a.Key.Ubicacion,
                        FechaOperacion = a.Min(e => e.FechaOperacion)?.ToString(CDateFormats.DATE_ONLY),
                        Funcionario = a.Key.Funcionario,
                    });

                var keysDisponibilidad = almacenamientos.GroupBy(a => new { a.NumeroEtiqueta, a.Empresa, a.CodigoProducto, a.Faixa, a.Identificador })
                   .Select(a => new LogEtiqueta()
                   {
                       NumeroEtiqueta = a.Key.NumeroEtiqueta,
                       Empresa = a.Key.Empresa,
                       CodigoProducto = a.Key.CodigoProducto,
                       Faixa = a.Key.Faixa,
                       Identificador = a.Key.Identificador,
                   });

                var cantidadesDisponibles = GetCantidadesDisponibles(keysDisponibilidad);


                foreach (var det in detalles)
                {
                    var keyDetalleEtiqueta = $"{etiquetaLote.Numero}.{det.Empresa}.{det.Producto}.{det.Faixa?.ToString("#.###")}.{det.Identificador}";
                    det.CantidadDisponible = cantidadesDisponibles.GetValueOrDefault(keyDetalleEtiqueta, 0);
                    det.CantidadAlmacenada = -1 * det.CantidadAlmacenada;
                    model.Detalles.Add(det);
                }
            }

            return model;
        }

        public virtual async Task<long> CrearEjecucion(AlmacenamientoResponse almacenamiento, string grupoConsulta, DbConnection connection, DbTransaction tran)
        {
            var ejecucionRepository = new EjecucionRepository(_dapper);

            var empresa = almacenamiento.Detalles.FirstOrDefault().Empresa;
            var interfaz = new InterfazEjecucion
            {
                CdInterfazExterna = CInterfazExterna.Almacenamiento,
                Situacion = SituacionDb.ProcesadoPendiente,
                Comienzo = DateTime.Now,
                FechaSituacion = DateTime.Now,
                ErrorCarga = "N",
                ErrorProcedimiento = "N",
                Referencia = $"Interfaz de Almacenamiento. Agenda: {almacenamiento.Agenda} - Etiqueta {almacenamiento.Etiqueta}",
                Empresa = empresa,
                GrupoConsulta = grupoConsulta
            };

            var data = JsonConvert.SerializeObject(almacenamiento);
            var itfzData = new InterfazData
            {
                Alta = DateTime.Now,
                Data = Encoding.UTF8.GetBytes(data)
            };

            interfaz = await ejecucionRepository.AddEjecucion(interfaz, itfzData, connection, tran);

            return interfaz.Id;
        }

        public virtual EtiquetaLote GetEtiquetaLote(int nuEtiqueta, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            NU_ETIQUETA_LOTE as Numero,
                            NU_AGENDA as NumeroAgenda,
                            CD_ENDERECO as IdUbicacion,
                            CD_ENDERECO_SUGERIDO as IdUbicacionSugerida,
                            CD_SITUACAO as Estado,
                            CD_FUNC_RECEPCION as FuncionarioRecepcion,
                            DT_RECEPCION as FechaRecepcion,
                            CD_FUNC_ALMACENAMIENTO as FuncionarioAlmacenamiento,
                            DT_ALMACENAMIENTO as FechaAlmacenamiento,
                            CD_CLIENTE as Cliente,
                            CD_GRUPO as CodigoGrupo,
                            CD_PALLET as CodigoPallet,
                            CD_BARRAS as CodigoBarras,
                            CD_ENDERECO_MOVTO_PARCIAL as UbicacionMovimiento,
                            CD_SITUACAO_PALLET as EstadoPallet,
                            NU_EXTERNO_ETIQUETA as NumeroExterno,
                            TP_ETIQUETA as TipoEtiqueta,
                            DT_UPDROW as FechaModificacion,
                            NU_UNIDAD_TRANSPORTE as CodigoUnidadTransporte,
                            NU_TRANSACCION as NumeroTransaccion,
                            NU_TRANSACCION_DELETE  as NumeroTransaccionDelete,
                            NU_LPN as NroLpn
                            FROM T_ETIQUETA_LOTE
                        WHERE NU_ETIQUETA_LOTE = :nuEtiqueta";

            return _dapper.Query<EtiquetaLote>(connection, sql, param: new
            {
                nuEtiqueta = nuEtiqueta
            }, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual Agente GetClienteAgenda(int nuAgenda, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            c.CD_EMPRESA as Empresa,
                            c.CD_CLIENTE as CodigoInterno,
                            c.CD_ROTA as RutaId,
                            c.DS_CLIENTE as Descripcion,
                            c.DS_ENDERECO as Direccion,
                            c.DS_BAIRRO as Barrio,
                            c.CD_CEP as CodigoPostal,
                            c.NU_TELEFONE as TelefonoPrincipal,
                            c.NU_DDD as NuDDD,
                            c.NU_FAX as TelefonoSecundario,
                            c.NU_INSCRICAO as OtroDatoFiscal,
                            c.CD_CGC_CLIENTE as NumeroFiscal,
                            c.CD_SITUACAO as EstadoId,
                            c.DT_SITUACAO as FechaSituacion,
                            c.DT_CADASTRAMENTO as FechaAlta,
                            c.DT_ALTERACAO as FechaModificacion,
                            c.TP_ATIVIDADE as TipoActividad,
                            c.NU_PRIOR_CARGA as OrdenDeCarga,
                            c.NU_DV_CLIENTE as NuDvCliente,
                            c.DS_ANEXO1 as Anexo1,
                            c.DS_ANEXO2 as Anexo2,
                            c.DS_ANEXO3 as Anexo3,
                            c.DS_ANEXO4 as Anexo4,
                            c.ID_CLIENTE_FILIAL as IdClienteFilial,
                            c.CD_FORNECEDOR as Fornecedor,
                            c.CD_CLIENTE_EN_CONSOLIDADO as ClienteConsolidado,
                            c.CD_EMPRESA_CONSOLIDADA as EmpresaConsolidada,
                            c.CD_AGENTE as Codigo,
                            c.TP_AGENTE as Tipo,
                            c.FL_ACEPTA_DEVOLUCION as AceptaDevolucionId,
                            c.CD_GLN as NumeroLocalizacionGlobal,
                            c.CD_PUNTO_ENTREGA as PuntoDeEntrega,
                            c.CD_CATEGORIA as Categoria,
                            c.ND_TIPO_FISCAL as TipoFiscalId,
                            c.ID_LOCALIDAD as IdLocalidad,
                            c.VL_PORCENTAJE_VIDA_UTIL as ValorManejoVidaUtil,
                            c.CD_GRUPO_CONSULTA as GrupoConsulta,
                            c.FL_SYNC_REALIZADA as SincronizacionRealizadaId
                           FROM T_AGENDA a
                           INNER JOIN T_CLIENTE c ON a.CD_EMPRESA = c.CD_EMPRESA AND a.CD_CLIENTE = c.CD_CLIENTE
                           WHERE a.NU_AGENDA = :nuAgenda";

            return _dapper.Query<Agente>(connection, sql, param: new
            {
                nuAgenda = nuAgenda
            }, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual IEnumerable<LogEtiqueta> GetLogsEtiqueta(LogEtiqueta datos, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            NU_LOG_ETIQUETA as Id,
                            NU_AGENDA as Agenda,
                            NU_ETIQUETA as NumeroEtiqueta,
                            CD_PRODUTO as CodigoProducto,
                            CD_FAIXA as Faixa,
                            CD_EMPRESA as Empresa,
                            NU_IDENTIFICADOR as Identificador,
                            QT_MOVIMIENTO as Cantidad,
                            CD_ENDERECO as Ubicacion,
                            DT_OPERACION as FechaOperacion,
                            NU_TRANSACCION as NroTransaccion,
                            NU_INTERFAZ_EJECUCION as NroInterfazEjecucion,
                            DT_FABRICACAO as Vencimiento,
                            TP_MOVIMIENTO as TipoMovimiento,
                            CD_FUNCIONARIO as Funcionario,
                            CD_APLICACAO as Aplicacion
                FROM T_LOG_ETIQUETA 
                WHERE NU_AGENDA = :nuAgenda AND NU_ETIQUETA = :nuEtiqueta AND NU_INTERFAZ_EJECUCION = -1";

            return _dapper.Query<LogEtiqueta>(connection, sql, param: new
            {
                nuAgenda = datos.Agenda,
                nuEtiqueta = datos.NumeroEtiqueta
            }, commandType: CommandType.Text, transaction: tran);
        }

        public virtual Dictionary<string, decimal> GetCantidadesDisponibles(IEnumerable<LogEtiqueta> almacenamientos)
        {
            var resultado = new Dictionary<string, decimal>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_DET_ETIQUETA_LOTE_TEMP (NU_ETIQUETA_LOTE, CD_EMPRESA, CD_PRODUTO, NU_IDENTIFICADOR, CD_FAIXA) 
                                   VALUES (:NumeroEtiqueta, :Empresa, :CodigoProducto, :Identificador, :Faixa)";
                    _dapper.Execute(connection, sql, almacenamientos, transaction: tran);

                    sql = @"SELECT
                                DEL.NU_ETIQUETA_LOTE as IdEtiquetaLote,
                                DEL.CD_PRODUTO as CodigoProducto,
                                DEL.NU_IDENTIFICADOR as Identificador,
                                DEL.CD_FAIXA as Faixa,
                                DEL.CD_EMPRESA as IdEmpresa,
                                COALESCE(DEL.QT_PRODUTO, 0) as Cantidad
                            FROM T_DET_ETIQUETA_LOTE DEL
                            INNER JOIN T_DET_ETIQUETA_LOTE_TEMP T ON 
                            DEL.NU_ETIQUETA_LOTE = T.NU_ETIQUETA_LOTE AND
                            DEL.CD_EMPRESA = T.CD_EMPRESA AND
                            DEL.CD_PRODUTO = T.CD_PRODUTO AND
                            DEL.NU_IDENTIFICADOR = T.NU_IDENTIFICADOR AND
                            DEL.CD_FAIXA = T.CD_FAIXA";

                    resultado = _dapper.Query<EtiquetaLoteDetalle>(connection, sql, null, commandType: CommandType.Text, transaction: tran)
                    .ToDictionary(x => $"{x.IdEtiquetaLote}.{x.IdEmpresa}.{x.CodigoProducto}.{x.Faixa.ToString("#.###")}.{x.Identificador}", x => (x.Cantidad ?? 0));

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual List<object> GetLogEtiquetaObject(List<int> nuLogsEtiqueta, long nuEjecucion)
        {
            var result = new List<object>();

            foreach (var id in nuLogsEtiqueta)
            {
                result.Add(new
                {
                    Id = id,
                    InterfazEjecucion = nuEjecucion
                });
            }

            return result;
        }

        public virtual async Task UpdateLogEtiqueta(List<object> almacenamientos, DbConnection connection, DbTransaction tran)
        {
            string sql = @"UPDATE T_LOG_ETIQUETA 
                SET NU_INTERFAZ_EJECUCION = :InterfazEjecucion
                WHERE NU_LOG_ETIQUETA = :Id";

            await _dapper.ExecuteAsync(connection, sql, almacenamientos, transaction: tran);
        }

        public virtual void MovilizarProductoLote(EtiquetaLoteDetalle etiqueta, decimal cantidad)
        {
            var sql = @"
                UPDATE T_DET_ETIQUETA_LOTE
                SET QT_PRODUTO = COALESCE(QT_PRODUTO, 0) - :QT_MOVILIZADO,
                    QT_MOVILIZADO = COALESCE(QT_MOVILIZADO, 0) + :QT_MOVILIZADO,
                    DT_UPDROW = :DT_UPDROW,
                    NU_TRANSACCION = :NU_TRANSACCION
                WHERE NU_ETIQUETA_LOTE = :NU_ETIQUETA_LOTE
                    AND CD_PRODUTO = :CD_PRODUTO
                    AND NU_IDENTIFICADOR = :NU_IDENTIFICADOR
                    AND CD_FAIXA = :CD_FAIXA
                    AND CD_EMPRESA = :CD_EMPRESA
            ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, new
            {
                QT_MOVILIZADO = cantidad,
                DT_UPDROW = DateTime.Now,
                NU_TRANSACCION = etiqueta.NumeroTransaccion,
                NU_ETIQUETA_LOTE = etiqueta.IdEtiquetaLote,
                CD_PRODUTO = etiqueta.CodigoProducto,
                NU_IDENTIFICADOR = etiqueta.Identificador,
                CD_FAIXA = etiqueta.Faixa,
                CD_EMPRESA = etiqueta.IdEmpresa,
            }, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public class BulkAnulacionContext
        {
            public List<int> NuLogsEtiqueta = new List<int>();
            public List<object> UpdateLogEtiqueta = new List<object>();
        }

        #endregion
    }
}
