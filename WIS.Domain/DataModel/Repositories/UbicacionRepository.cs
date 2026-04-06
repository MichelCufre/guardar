using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using WIS.Domain.Automatismo.Constants;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.General.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;
using WIS.Persistence.General;

namespace WIS.Domain.DataModel.Repositories
{
    public class UbicacionRepository
    {
        protected WISDB _context;
        protected string application;
        protected int userId;
        protected readonly UbicacionMapper _mapper;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly IDapper _dapper;
        protected readonly ParametroRepository _paramRepository;

        public UbicacionRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this.application = application;
            this.userId = userId;
            this._mapper = new UbicacionMapper();
            this._dapper = dapper;
            this._paramRepository = new ParametroRepository(_context, application, userId, dapper);
        }

        #region Any

        public virtual bool AnyUbicacionProductoFamilia(int? codigoFamilia)
        {
            return _context.T_ENDERECO_ESTOQUE.Any(x => x.CD_FAMILIA_PRINCIPAL == codigoFamilia);
        }

        public virtual bool AnyUbicacion(string idUbicacion)
        {
            return _context.T_ENDERECO_ESTOQUE.Any(x => x.CD_ENDERECO == idUbicacion);
        }

        public virtual bool AnyUbicacionBloque(string codigoBloque)
        {
            return _context.T_ENDERECO_ESTOQUE.Any(x => x.ID_BLOQUE == codigoBloque);
        }

        public virtual bool AnyUbicacionConPredio(string nuPredio)
        {
            return _context.T_ENDERECO_ESTOQUE.Any(x => x.NU_PREDIO == nuPredio);
        }

        public virtual bool AnyUbicacionValidaPicking(string ubicacion)
        {
            return this._context.T_ENDERECO_ESTOQUE
            .Include("T_TIPO_AREA")
            .AsNoTracking()
            .Any(d =>
                d.CD_ENDERECO == ubicacion
                && d.T_TIPO_AREA.ID_AREA_PICKING == "S"
                && d.T_TIPO_AREA.ID_DISP_ESTOQUE == "S"
            );
        }

        #endregion

        #region Get

        public virtual UbicacionConfiguracion GetUbicacionConfiguracion()
        {
            UbicacionConfiguracion configuracion = new UbicacionConfiguracion();

            var parameters = _paramRepository.GetParameters(new List<string>
            {
                ParamManager.WREG040_VL_LONG_PREDIO,
                ParamManager.WREG040_VL_TIPO_PREDIO,
                ParamManager.WREG040_VL_LONG_BLOQUE,
                ParamManager.WREG040_VL_TIPO_BLOQUE,
                ParamManager.WREG040_VL_LONG_CALLE,
                ParamManager.WREG040_VL_TIPO_CALLE,
                ParamManager.WREG040_VL_LONG_FRENTE,
                ParamManager.WREG040_VL_LONG_ALTURA,
                ParamManager.WREG040_CD_SITUACAO,
                ParamManager.WREG040_CD_ZONA_UBICACION,
            });

            // Carga de parametros de base
            var largoPredio = string.IsNullOrEmpty(parameters[ParamManager.WREG040_VL_LONG_PREDIO]) ? (short)10 : short.Parse(parameters[ParamManager.WREG040_VL_LONG_PREDIO]);

            configuracion.PredioLargo = largoPredio > 10 ? (short)10 : largoPredio;
            configuracion.PredioNumerico = (parameters[ParamManager.WREG040_VL_TIPO_PREDIO] == "0") ? true : false;

            var largoBloque = string.IsNullOrEmpty(parameters[ParamManager.WREG040_VL_LONG_BLOQUE]) ? (short)10 : short.Parse(parameters[ParamManager.WREG040_VL_LONG_BLOQUE]);

            configuracion.BloqueLargo = largoBloque > 10 ? (short)10 : largoBloque;
            configuracion.BloqueNumerico = (parameters[ParamManager.WREG040_VL_TIPO_BLOQUE] == "0") ? true : false;

            var largoCalle = string.IsNullOrEmpty(parameters[ParamManager.WREG040_VL_LONG_CALLE]) ? (short)10 : short.Parse(parameters[ParamManager.WREG040_VL_LONG_CALLE]);

            configuracion.CalleLargo = largoCalle > 10 ? (short)10 : largoCalle;
            configuracion.CalleNumerico = (parameters[ParamManager.WREG040_VL_TIPO_CALLE] == "0") ? true : false;

            var largoColumna = string.IsNullOrEmpty(parameters[ParamManager.WREG040_VL_LONG_FRENTE]) ? (short)10 : short.Parse(parameters[ParamManager.WREG040_VL_LONG_FRENTE]);
            configuracion.ColumnaLargo = largoColumna > 10 ? (short)10 : largoColumna;

            var largoAltura = string.IsNullOrEmpty(parameters[ParamManager.WREG040_VL_LONG_ALTURA]) ? (short)10 : short.Parse(parameters[ParamManager.WREG040_VL_LONG_ALTURA]);
            configuracion.AlturaLargo = largoAltura > 10 ? (short)10 : largoAltura;

            var situacion = string.IsNullOrEmpty(parameters[ParamManager.WREG040_CD_SITUACAO]) ? SituacionDb.Activo : short.Parse(parameters[ParamManager.WREG040_CD_SITUACAO]);
            configuracion.EstadoCreacion = (situacion != SituacionDb.Activo && situacion != SituacionDb.Inactivo) ? SituacionDb.Activo : situacion;

            configuracion.UbicacionZonaPorDefecto = parameters[ParamManager.WREG040_CD_ZONA_UBICACION];

            // Areas no mantenibles
            configuracion.AreasMantenibles = this._context.T_TIPO_AREA.Where(s => s.ID_PERMITE_MANTENIMIENTO == "S").Select(s => s.CD_AREA_ARMAZ).ToList();


            return configuracion;
        }

        public virtual Ubicacion GetUbicacion(string id)
        {
            var entity = this._context.T_ENDERECO_ESTOQUE
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_ENDERECO == id);

            return this._mapper.MapToObject(entity);
        }

        public virtual List<Ubicacion> GetUbiPickingByCodigoPartial(string nombre)
        {
            var ubicaciones = new List<Ubicacion>();

            var entries = this._context.T_ENDERECO_ESTOQUE
                            .Include("T_TIPO_AREA")
                            .AsNoTracking()
                            .Where(d => d.CD_ENDERECO.Contains(nombre.ToUpper())
                                && d.T_TIPO_AREA.ID_AREA_PICKING == "S"
                                && d.T_TIPO_AREA.ID_DISP_ESTOQUE == "S").ToList();
            foreach (var entry in entries)
            {
                ubicaciones.Add(this._mapper.MapToObject(entry));
            }

            return ubicaciones;
        }

        public virtual List<Ubicacion> GetUbicacionesDispoCodigoPartial(string nombre)
        {
            var ubicaciones = new List<Ubicacion>();

            var entries = this._context.T_ENDERECO_ESTOQUE
                            .Include("T_TIPO_AREA")
                            .AsNoTracking()
                            .Where(d => d.CD_ENDERECO.Contains(nombre.ToUpper())
                                && ((d.T_TIPO_AREA.ID_AREA_PICKING == "S"
                                    && d.T_TIPO_AREA.ID_DISP_ESTOQUE == "S")
                                || (d.T_TIPO_AREA.ID_ESTOQUE_GERAL == "S"
                                    && d.T_TIPO_AREA.ID_DISP_ESTOQUE == "S"))).ToList();
            foreach (var entry in entries)
            {
                ubicaciones.Add(this._mapper.MapToObject(entry));
            }

            return ubicaciones;
        }

        public virtual List<Ubicacion> GetUbiPickingAutomamismoByCodigoPartial(string searchValue, int automatismo)
        {
            return this._context.T_ENDERECO_ESTOQUE
            .Join(
                this._context.T_AUTOMATISMO_POSICION,
                ee => new { ee.CD_ENDERECO },
                ap => new { ap.CD_ENDERECO },
                (ee, ap) => new { Ubicacion = ee, Posicion = ap })
            .Where(u => u.Posicion.NU_AUTOMATISMO == automatismo
                    && u.Posicion.ND_TIPO_ENDERECO == AutomatismoPosicionesTipoDb.POS_PICKING
                    && u.Ubicacion.CD_ENDERECO.Contains(searchValue.ToUpper()))
            .Select(u => _mapper.MapToObject(u.Ubicacion))
            .ToList();
        }

        public virtual List<Ubicacion> GetUbicacionColumna(int numeroColumna)
        {
            var entities = this._context.T_ENDERECO_ESTOQUE.AsNoTracking()
                .Where(d => d.NU_COLUMNA == numeroColumna)
               .ToList();

            List<Ubicacion> ubicaciones = new List<Ubicacion>();

            foreach (var entity in entities)
            {
                ubicaciones.Add(this._mapper.MapToObject(entity));
            }

            return ubicaciones;
        }

        public virtual List<Ubicacion> GetUbicaciones()
        {
            var entities = this._context.T_ENDERECO_ESTOQUE.AsNoTracking().ToList();

            List<Ubicacion> ubicaciones = new List<Ubicacion>();

            foreach (var entity in entities)
            {
                ubicaciones.Add(this._mapper.MapToObject(entity));
            }

            return ubicaciones;
        }

        public virtual List<Ubicacion> GetUbicaciones(List<string> ids)
        {
            var entity = this._context.T_ENDERECO_ESTOQUE.AsNoTracking()
                .Where(d => ids.Contains(d.CD_ENDERECO)).ToList();

            if (entity == null || entity.Count == 0)
                return new List<Ubicacion>();

            List<Ubicacion> result = new List<Ubicacion>();
            entity.ForEach(x =>
            {

                result.Add(this._mapper.MapToObject(x));
            });
            return result;
        }

        public virtual List<Ubicacion> GetUbicaciones(List<string> ids, string predio)
        {
            return _context.T_ENDERECO_ESTOQUE
                .AsNoTracking()
                .Where(d => ids.Contains(d.CD_ENDERECO) && d.NU_PREDIO == predio)
                .Select(d => _mapper.MapToObject(d)).ToList();
        }

        public virtual List<long?> GetOrdenesPorDefecto(string nuPredio)
        {
            return _context.T_ENDERECO_ESTOQUE
                .AsNoTracking()
                .Where(x => x.NU_PREDIO == nuPredio && x.NU_ORDEN_DEFAULT != null)
                .Select(x => x.NU_ORDEN_DEFAULT)
                .ToList();
        }

        public virtual string GetPredio(string idUbicacion)
        {
            var endereco = this._context.T_ENDERECO_ESTOQUE.AsNoTracking().FirstOrDefault(ee => ee.CD_ENDERECO == idUbicacion);

            if (endereco != null)
                return endereco.NU_PREDIO;
            else
                return null;
        }

        #endregion

        #region Add

        public virtual void AddUbicacion(Ubicacion ubicacion)
        {
            var entity = this._mapper.MapToEntity(ubicacion);

            entity.DT_ADDROW = DateTime.Now;
            entity.DT_UPDROW = DateTime.Now;

            this._context.T_ENDERECO_ESTOQUE.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateUbicacion(Ubicacion ubicacion)
        {
            var entity = this._mapper.MapToEntity(ubicacion);
            var attachedEntity = _context.T_ENDERECO_ESTOQUE.Local
                .FirstOrDefault(w => w.CD_ENDERECO == entity.CD_ENDERECO);

            entity.DT_UPDROW = DateTime.Now;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_ENDERECO_ESTOQUE.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void RemoveUbicacion(Ubicacion ubicacion)
        {
            var entity = this._mapper.MapToEntity(ubicacion);
            var attachedEntity = _context.T_ENDERECO_ESTOQUE.Local
               .FirstOrDefault(w => w.CD_ENDERECO == entity.CD_ENDERECO);

            if (attachedEntity != null)
            {
                this._context.T_ENDERECO_ESTOQUE.Remove(attachedEntity);
            }
            else
            {
                this._context.T_ENDERECO_ESTOQUE.Attach(entity);
                this._context.T_ENDERECO_ESTOQUE.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        public virtual IEnumerable<Ubicacion> GetUbicaciones(IEnumerable<Ubicacion> ubicaciones)
        {
            IEnumerable<Ubicacion> resultado = new List<Ubicacion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    _dapper.BulkInsert(connection, tran, ubicaciones, "T_ENDERECO_ESTOQUE_TEMP", new Dictionary<string, Func<Ubicacion, ColumnInfo>>
                    {
                        { "CD_ENDERECO", x => new ColumnInfo(x.Id)}
                    });

                    var sql = @"SELECT 
                            P.CD_AREA_ARMAZ as IdUbicacionArea,
                            P.CD_BARRAS as CodigoBarras,
                            P.CD_CLASSE as CodigoClase,
                            P.CD_CONTROL as CodigoControl,
                            P.CD_CONTROL_ACCESO as IdControlAcceso,
                            P.CD_EMPRESA as IdEmpresa,
                            P.CD_ENDERECO as Id,
                            P.CD_FAMILIA_PRINCIPAL as IdProductoFamilia,
                            P.CD_ROTATIVIDADE as IdProductoRotatividad,
                            P.CD_SITUACAO as CodigoSituacion,
                            P.CD_TIPO_ENDERECO as IdUbicacionTipo,
                            P.CD_ZONA_UBICACION as IdUbicacionZona,
                            P.DT_ADDROW as FechaInsercion,
                            P.DT_UPDROW as FechaModificacion,
                            P.ID_BLOQUE as Bloque,
                            P.ID_CALLE as Calle,
                            P.ID_ENDERECO_BAIXO as IdUbicacionBaja,
                            P.ID_ENDERECO_SEP as IdUbicacionSeparacion,
                            P.ID_NECESSIDADE_RESUPRIR as IdNecesitaReabastecer,
                            P.ND_SECTOR as DominioSector,
                            P.NU_ALTURA as Altura,
                            P.NU_COLUMNA as Columna,
                            P.NU_COMPONENTE as FacturacionComponente,
                            P.NU_ORDEN_DEFAULT as OrdenDefecto,
                            P.NU_PREDIO as NumeroPredio,
                            P.NU_PROFUNDIDAD as Profundidad
                        FROM T_ENDERECO_ESTOQUE P 
                            INNER JOIN T_ENDERECO_ESTOQUE_TEMP T ON P.CD_ENDERECO = T.CD_ENDERECO";

                    resultado = _dapper.Query<Ubicacion>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IEnumerable<string> GetCodigosDeBarras(IEnumerable<Ubicacion> ubicaciones)
        {
            IEnumerable<string> resultado = new List<string>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    _dapper.BulkInsert(connection, tran, ubicaciones, "T_ENDERECO_ESTOQUE_TEMP", new Dictionary<string, Func<Ubicacion, ColumnInfo>>
                    {
                        { "CD_BARRAS", x => new ColumnInfo(x.CodigoBarras)},
                        { "NU_PREDIO", x => new ColumnInfo(x.NumeroPredio)}
                    });

                    var sql = @"SELECT 
                                P.CD_BARRAS
                        FROM T_ENDERECO_ESTOQUE P 
                            INNER JOIN T_ENDERECO_ESTOQUE_TEMP T ON P.CD_BARRAS = T.CD_BARRAS AND P.NU_PREDIO = T.NU_PREDIO ";

                    resultado = _dapper.Query<string>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual string GetUbicacionEquipo(int userId, string predio)
        {
            var ubicacion = string.Empty;

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                string sql = @"SELECT eq.CD_ENDERECO FROM T_EQUIPO eq INNER JOIN T_ENDERECO_ESTOQUE ee ON eq.CD_ENDERECO = ee.CD_ENDERECO 
                                   WHERE eq.CD_FUNCIONARIO = :userId AND ee.NU_PREDIO = :predio";

                ubicacion = _dapper.Query<string>(connection, sql, param: new { userId = userId, predio = predio }).FirstOrDefault();
            }

            return ubicacion;
        }

        public virtual IEnumerable<UbicacionEquipo> GetUbicacionesEquipo(int userId)
        {
            IEnumerable<UbicacionEquipo> ubicaciones = new List<UbicacionEquipo>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                string sql = @"SELECT 
                                USERID as Usuario,
                                CD_ENDERECO as Ubicacion,
                                NU_PREDIO as Predio,
                                CD_EQUIPO as CodigoEquipo,
                                ID_AUTOASIGNADO as AutoAsignado
                               FROM V_USUARIO_PREDIO_EQUIPO_PICKING
                               WHERE USERID = :userId ";

                ubicaciones = _dapper.Query<UbicacionEquipo>(connection, sql, param: new { userId = userId, });
            }

            return ubicaciones;
        }

        public virtual void AgregarUbicacionesExternas(List<UbicacionExterna> ubicaciones, IUbicacionServiceContext serviceContext, CancellationToken cancelToken = default)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            _dapper.BulkInsert(connection, tran, ubicaciones, "T_ENDERECO_ESTOQUE", new Dictionary<string, Func<UbicacionExterna, ColumnInfo>>
            {
                { "CD_ENDERECO", x => new ColumnInfo(x.Id)},
                { "CD_EMPRESA", x => new ColumnInfo(x.IdEmpresa)},
                { "CD_TIPO_ENDERECO", x => new ColumnInfo(x.IdUbicacionTipo)},
                { "CD_ROTATIVIDADE", x => new ColumnInfo(x.IdProductoRotatividad)},
                { "CD_FAMILIA_PRINCIPAL", x => new ColumnInfo(x.IdProductoFamilia)},
                { "CD_CLASSE", x => new ColumnInfo(x.CodigoClase)},
                { "ID_ENDERECO_BAIXO", x => new ColumnInfo(x.IdUbicacionBaja)},
                { "ID_ENDERECO_SEP", x => new ColumnInfo(x.IdEsUbicacionSeparacion, DbType.String)},
                { "ID_NECESSIDADE_RESUPRIR", x => new ColumnInfo(x.IdNecesitaReabastecer)},
                { "DT_ADDROW", x => new ColumnInfo(x.FechaInsercion)},
                { "CD_CONTROL", x => new ColumnInfo(x.CodigoControl, DbType.String)},
                { "CD_AREA_ARMAZ", x => new ColumnInfo(x.IdUbicacionArea)},
                { "NU_COMPONENTE", x => new ColumnInfo(x.FacturacionComponente, DbType.String)},
                { "CD_ZONA_UBICACION", x => new ColumnInfo(x.IdUbicacionZona, DbType.String)},
                { "NU_PREDIO", x => new ColumnInfo(x.NumeroPredio )},
                { "ID_BLOQUE", x => new ColumnInfo(x.Bloque, DbType.String)},
                { "ID_CALLE", x => new ColumnInfo(x.Calle, DbType.String)},
                { "NU_COLUMNA", x => new ColumnInfo(x.Columna, DbType.Int32)},
                { "NU_ALTURA", x => new ColumnInfo(x.Altura, DbType.Int32)},
                { "ND_SECTOR", x => new ColumnInfo(x.DominioSector, DbType.String)},
                { "CD_BARRAS", x => new ColumnInfo(x.CodigoBarras, DbType.String)},
                { "NU_PROFUNDIDAD", x => new ColumnInfo(x.Profundidad)},
                { "CD_SITUACAO", x => new ColumnInfo(x.CodigoSituacion)},
                { "CD_CONTROL_ACCESO", x => new ColumnInfo(x.IdControlAcceso, DbType.String)},
                { "NU_ORDEN_DEFAULT", x => new ColumnInfo(x.OrdenDefecto, DbType.Int64)},
            });

            var ubicacionesRecorribles = ubicaciones.Where(u => u.IsRecorrible).ToList();

            _dapper.BulkInsert(connection, tran, ubicacionesRecorribles, "T_RECORRIDO_DET", new Dictionary<string, Func<UbicacionExterna, ColumnInfo>>
            {
                { "NU_RECORRIDO_DET", x => new ColumnInfo(_context.GetNextSequenceValueLong(_dapper, Secuencias.S_RECORRIDO_DET))},
                { "NU_RECORRIDO", x => new ColumnInfo(serviceContext.RecorridoPorDefecto.Id)},
                { "CD_ENDERECO", x => new ColumnInfo(x.Id)},
                { "NU_ORDEN", x => new ColumnInfo(x.OrdenDefecto)},
                { "VL_ORDEN", x => new ColumnInfo(x.ValorDefecto)},
                { "DT_ADDROW", x => new ColumnInfo(DateTime.Now)},
                { "DT_UPDROW", x => new ColumnInfo(DateTime.Now)},
                { "NU_TRANSACCION", x => new ColumnInfo(_context.GetTransactionNumber())}
            });
        }

        #endregion
    }
}
