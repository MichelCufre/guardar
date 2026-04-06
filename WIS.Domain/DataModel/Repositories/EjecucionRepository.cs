using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Interfaces;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.DataModel.Repositories
{
    public class EjecucionRepository
    {
        protected readonly IDapper _dapper;     

        public EjecucionRepository(IDapper dapper)
        {            
            _dapper = dapper;         
        }

        public virtual async Task<bool> ExisteEjecucion(long nroEjecucion)
        {
            var sql = @"SELECT * FROM T_INTERFAZ_EJECUCION WHERE NU_INTERFAZ_EJECUCION = :nroEjecucion";
            var param = new DynamicParameters(new { nroEjecucion = nroEjecucion });

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync();

                var query = _dapper.Query<string>(connection, sql, param: param, commandType: CommandType.Text);

                return query.FirstOrDefault() != null;
            }
        }

        public virtual async Task<bool> ExisteIdRequest(string idRequest, int empresa)
        {
            var sql = @"SELECT ID_REQUEST FROM T_INTERFAZ_EJECUCION WHERE ID_REQUEST = :idRequest AND CD_EMPRESA = :empresa";
            var param = new DynamicParameters(new { idRequest = idRequest, empresa = empresa });

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync();

                var query = _dapper.Query<string>(connection, sql, param: param, commandType: CommandType.Text);

                return query.FirstOrDefault() != null;
            }
        }

        public virtual async Task<InterfazEjecucion> GetEjecucion(long nroEjecucion, CancellationToken cancelToken = default)
        {
            string sql = GetSqlSelectInterfazEjecucion() +
                @"WHERE ie.NU_INTERFAZ_EJECUCION = :nroEjecucion";

            var param = new DynamicParameters(new { nroEjecucion = nroEjecucion });

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var interfazEjecucion = _dapper.Query<InterfazEjecucion>(connection, sql, param: param, commandType: CommandType.Text)?.FirstOrDefault();

                sql = GetSqlSelectInterfazExterna() +
                    @" WHERE iex.CD_INTERFAZ_EXTERNA = :CdInterfazExterna";

                interfazEjecucion.InterfazExterna = _dapper.Query<InterfazExterna>(connection, sql, param: new { CdInterfazExterna = interfazEjecucion.CdInterfazExterna }, commandType: CommandType.Text)?.FirstOrDefault();

                return interfazEjecucion;
            }
        }

        public virtual async Task<List<InterfazEjecucion>> GetNotificacionesPendientes(CancellationToken cancelToken = default)
        {
            string sql = GetSqlSelectInterfazEjecucion() +
                @"JOIN T_INTERFAZ inte ON ie.CD_INTERFAZ_EXTERNA = inte.CD_INTERFAZ
                INNER JOIN T_EMPRESA e ON ie.CD_EMPRESA = e.CD_EMPRESA
                WHERE e.TP_NOTIFICACION = :tipoNotificacion 
                    AND inte.ID_ENTRADA_SALIDA = 'S' 
                    AND ie.CD_SITUACAO = :situacion 
                ORDER BY ie.NU_INTERFAZ_EJECUCION";

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var interfacezExternas = _dapper.Query<InterfazExterna>(connection, GetSqlSelectInterfazExterna(), commandType: CommandType.Text);

                var query = _dapper.Query<InterfazEjecucion>(connection, sql, param: new
                {
                    tipoNotificacion = CodigoDominioDb.TipoNotificacionWebhook,
                    situacion = SituacionDb.ProcesadoPendiente
                }, commandType: CommandType.Text);

                foreach (var i in query)
                {
                    i.InterfazExterna = interfacezExternas.FirstOrDefault(x => x.CodigoInterfazExterna == i.CdInterfazExterna);
                }

                return query.ToList();
            }
        }

        public virtual async Task<List<InterfazEjecucion>> GetSalidasPendientes(int empresa, List<string> gruposConsulta, bool includeHooks, CancellationToken cancelToken = default)
        {
            string sql = GetSqlSelectInterfazEjecucion() +
                @"
                  INNER JOIN V_INTERFACES_SALIDA_HABILITADAS ih ON ih.CD_EMPRESA = ie.CD_EMPRESA AND ih.CD_INTERFAZ_EXTERNA = ie.CD_INTERFAZ_EXTERNA
                  LEFT JOIN V_INT050_EMPRESAS_BLOQUEADAS empBL ON ie.CD_EMPRESA = empBL.CD_EMPRESA ";

            if (!includeHooks)
            {
                sql += @"JOIN T_EMPRESA e ON e.CD_EMPRESA = ie.CD_EMPRESA ";
            }

            sql += @"WHERE ie.CD_EMPRESA = :empresa 
                        AND empBL.CD_EMPRESA  is null
                        AND ie.CD_SITUACAO = :situacion ";

            if (!includeHooks)
            {
                sql += @"AND (e.TP_NOTIFICACION IS NULL OR 
                            e.TP_NOTIFICACION <> :tipoNotificacion) ";
            }

            if (gruposConsulta != null && gruposConsulta.Count > 0)
                sql += @"AND ie.CD_GRUPO_CONSULTA IN :gruposConsulta ";

            sql += "ORDER BY ie.NU_INTERFAZ_EJECUCION";

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var interfacezExternas = _dapper.Query<InterfazExterna>(connection, GetSqlSelectInterfazExterna(), commandType: CommandType.Text);

                var query = _dapper.Query<InterfazEjecucion>(connection, sql, param: new
                {
                    empresa = empresa,
                    situacion = SituacionDb.ProcesadoPendiente,
                    tipoNotificacion = CodigoDominioDb.TipoNotificacionWebhook,
                    gruposConsulta = gruposConsulta
                }, commandType: CommandType.Text);


                foreach (var i in query)
                {
                    i.InterfazExterna = interfacezExternas.FirstOrDefault(x => x.CodigoInterfazExterna == i.CdInterfazExterna);
                }

                return query.ToList();
            }
        }

        public static string GetSqlSelectInterfazEjecucion()
        {
            return @"SELECT 
                    ie.NU_INTERFAZ_EJECUCION as Id, 
                    ie.CD_INTERFAZ_EXTERNA as CdInterfazExterna, 
                    ie.NM_ARCHIVO as Archivo, 
                    ie.CD_SITUACAO as Situacion, 
                    ie.DT_COMIENZO as Comienzo, 
                    ie.DT_SITUACAO as FechaSituacion, 
                    ie.FL_ERROR_CARGA as ErrorCarga, 
                    ie.FL_ERROR_PROCEDIMIENTO as ErrorProcedimiento, 
                    ie.CD_FUNCIONARIO_ACEPTACION as FuncionarioAceptacion, 
                    ie.DS_REFERENCIA as Referencia, 
                    ie.ND_SITUACION as NdSituacion, 
                    ie.CD_EMPRESA as Empresa, 
                    ie.CD_GRUPO_CONSULTA as GrupoConsulta, 
                    ie.USERID as UserId, 
                    ie.ID_PROCESADO as Procesado
                FROM T_INTERFAZ_EJECUCION ie ";
        }

        public virtual async Task<long?> GetPrimeraSalida(int empresa, short situacion, List<int> interfacesOmisibles, CancellationToken cancelToken = default)
        {
            string sql = @"SELECT MIN(NU_INTERFAZ_EJECUCION)
                FROM T_INTERFAZ_EJECUCION ie INNER JOIN T_INTERFAZ i on ie.CD_INTERFAZ_EXTERNA = i.CD_INTERFAZ
                WHERE CD_EMPRESA = :empresa AND CD_SITUACAO = :situacion AND ID_ENTRADA_SALIDA = 'S' ";

            if (interfacesOmisibles.Count > 0)
            {
                sql += @"AND ie.CD_INTERFAZ_EXTERNA NOT IN :interfacesOmisibles";
            }

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var query = _dapper.Query<long>(connection, sql, param: new
                {
                    empresa = empresa,
                    situacion = situacion,
                    interfacesOmisibles = interfacesOmisibles
                }, commandType: CommandType.Text);

                return query.FirstOrDefault();
            }
        }

        public virtual async Task<List<InterfazError>> GetErrores(long nroEjecucion, CancellationToken cancelToken = default)
        {
            string sql = @"SELECT 
                    NU_INTERFAZ_EJECUCION as Id, 
                    NU_REGISTRO as Registro, 
                    NU_ERROR as NroError, 
                    DS_REFERENCIA as Referencia, 
                    CD_PARAMETRO as Parametro, 
                    CD_ERROR as CodigoError, 
                    DS_ERROR as Descripcion                             
                FROM T_INTERFAZ_EJECUCION_ERROR WHERE NU_INTERFAZ_EJECUCION = :nroEjecucion ORDER BY NU_REGISTRO, NU_ERROR";

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var query = _dapper.Query<InterfazError>(connection, sql, param: new { nroEjecucion = nroEjecucion }, commandType: CommandType.Text);

                return query.ToList();
            }
        }

        public virtual async Task<InterfazData> GetEjecucionData(long nroEjecucion, CancellationToken cancelToken = default)
        {
            string sql = @"SELECT 
                    NU_INTERFAZ_EJECUCION as Id, 
                    DT_ADDROW as Alta, 
                    DATA as Data 
                FROM T_INTERFAZ_EJECUCION_DATA 
                WHERE NU_INTERFAZ_EJECUCION =:nroEjecucion";

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var query = _dapper.Query<InterfazData>(connection, sql, param: new { nroEjecucion = nroEjecucion }, commandType: CommandType.Text);

                return query.FirstOrDefault();
            }
        }

        public virtual async Task<int?> GetUltimoError(long nroEjecucion, CancellationToken cancelToken = default)
        {
            string sql = @"SELECT MAX(NU_ERROR) as NroError 
                FROM T_INTERFAZ_EJECUCION_ERROR 
                WHERE NU_INTERFAZ_EJECUCION = :nroEjecucion";

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var query = _dapper.Query<int?>(connection, sql, param: new { nroEjecucion = nroEjecucion }, commandType: CommandType.Text);

                return query.FirstOrDefault();
            }
        }

        public virtual async Task<InterfazEjecucion> AddEjecucion(InterfazEjecucion ejecucion, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    await AddEjecucion(ejecucion, connection, tran);
                    tran.Commit();
                }
            }

            return ejecucion;
        }

        public virtual async Task<InterfazEjecucion> AddEjecucion(InterfazEjecucion ejecucion, DbConnection connection, DbTransaction tran)
        {
            string sql = @"INSERT INTO T_INTERFAZ_EJECUCION 
                    (NU_INTERFAZ_EJECUCION, CD_INTERFAZ_EXTERNA, NM_ARCHIVO, CD_SITUACAO, DT_COMIENZO, DT_SITUACAO, 
                    FL_ERROR_CARGA, FL_ERROR_PROCEDIMIENTO, DS_REFERENCIA, CD_EMPRESA, CD_GRUPO_CONSULTA, USERID, ID_REQUEST) 
                    VALUES 
                    (:Id, :CdInterfazExterna, :Archivo, :Situacion, :Comienzo, :FechaSituacion, 
                     :ErrorCarga, :ErrorProcedimiento, :Referencia, :Empresa, :GrupoConsulta, :UserId, :IdRequest)";

            if (ejecucion.Id < 1)
                ejecucion.Id = await GetNextIdEjecucion(connection, tran);

            var param = new DynamicParameters(new
            {
                Id = ejecucion.Id,
                CdInterfazExterna = ejecucion.CdInterfazExterna,
                Archivo = ejecucion.Archivo,
                Situacion = ejecucion.Situacion,
                Comienzo = ejecucion.Comienzo,
                FechaSituacion = ejecucion.FechaSituacion,
                ErrorCarga = ejecucion.ErrorCarga,
                ErrorProcedimiento = ejecucion.ErrorProcedimiento,
                Referencia = ejecucion.Referencia,
                Empresa = ejecucion.Empresa,
                GrupoConsulta = ejecucion.GrupoConsulta,
                UserId = ejecucion.UserId,
                IdRequest = ejecucion.IdRequest,
            });

            await _dapper.ExecuteAsync(connection, sql, param, transaction: tran);

            return ejecucion;
        }

        public virtual async Task<InterfazData> AddEjecucionData(InterfazData ejecucionData, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    ejecucionData = await AddEjecucionData(ejecucionData, connection, tran);
                    tran.Commit();
                }
            }

            return ejecucionData;
        }

        public virtual async Task<InterfazData> AddEjecucionData(InterfazData ejecucionData, DbConnection connection, DbTransaction tran)
        {
            string sql = @"INSERT INTO T_INTERFAZ_EJECUCION_DATA (NU_INTERFAZ_EJECUCION, DT_ADDROW, DATA) 
                        VALUES (:Id, :Alta, :Data)";

            var param = new DynamicParameters(new
            {
                Id = ejecucionData.Id,
                Alta = ejecucionData.Alta,
                Data = ejecucionData.Data
            });

            await _dapper.ExecuteAsync(connection, sql, param, transaction: tran);

            return ejecucionData;
        }

        public virtual async Task<InterfazEjecucion> AddEjecucion(InterfazEjecucion ejecucion, InterfazData ejecucionData, DbConnection connection, DbTransaction tran, CancellationToken cancelToken = default)
        {            
            await AddEjecucion(ejecucion, connection, tran);

            ejecucionData.Id = ejecucion.Id;

            await AddEjecucionData(ejecucionData, connection, tran);

            return ejecucion;
        }

        public virtual async Task<InterfazError> AddEjecucionError(InterfazError ejecucionError, CancellationToken cancelToken = default)
        {
            await AddEjecucionErrores(new List<InterfazError>() { ejecucionError }, cancelToken);
            return ejecucionError;
        }

        public virtual async Task AddEjecucionErrores(List<InterfazError> errores, CancellationToken cancelToken = default)
        {
            string sql = @"INSERT INTO T_INTERFAZ_EJECUCION_ERROR (NU_INTERFAZ_EJECUCION, NU_ERROR, NU_REGISTRO, DS_REFERENCIA, CD_PARAMETRO, CD_ERROR, DS_ERROR) 
                        VALUES (:Id, :NroError, :Registro, :Referencia, :Parametro, :CodigoError, :Descripcion)";

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using var tran = connection.BeginTransaction();
                {
                    await _dapper.ExecuteAsync(connection, sql, errores, transaction: tran);
                    tran.Commit();
                }
            }
        }

        public virtual async Task<InterfazEjecucion> Update(InterfazEjecucion interfaz, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    await Update(interfaz, connection, tran);
                    tran.Commit();
                }

                return interfaz;
            }
        }

        public virtual async Task<InterfazEjecucion> Update(InterfazEjecucion interfaz, DbConnection connection, DbTransaction tran, bool actualizaComienzo = false)
        {
            string sql = @"
                    UPDATE T_INTERFAZ_EJECUCION 
                    SET CD_SITUACAO = :Situacion,
                        DT_SITUACAO = :FechaSituacion,
                        FL_ERROR_CARGA = :ErrorCarga,
                        FL_ERROR_PROCEDIMIENTO = :ErrorProcedimiento";

            if (actualizaComienzo)
                sql += @", DT_COMIENZO = :Comienzo";

            sql += @" WHERE NU_INTERFAZ_EJECUCION = :Id";

            var param = new DynamicParameters(new
            {
                Id = interfaz.Id,
                Situacion = interfaz.Situacion,
                Comienzo = interfaz.Comienzo,
                FechaSituacion = DateTime.Now,
                ErrorCarga = interfaz.ErrorCarga,
                ErrorProcedimiento = interfaz.ErrorProcedimiento
            });

            await _dapper.ExecuteAsync(connection, sql, param, transaction: tran);

            return interfaz;
        }

        public virtual async Task<long> GetNextIdEjecucion(IDbConnection connection, IDbTransaction transaction = null)
        {
            return await _dapper.GetNextSequenceValueAsync<long>(connection, "S_INTERFAZ_EJECUCION", transaction);
        }

        public virtual async Task<InterfazEjecucion> AddErrores(InterfazEjecucion ejecucion, List<ValidationsError> errores)
        {
            int nroError = await GetUltimoError(ejecucion.Id) ?? 0;
            var iErrores = new List<InterfazError>();

            foreach (var error in errores)
            {
                foreach (var messagge in error.Messages)
                {
                    nroError++;
                    iErrores.Add(new InterfazError
                    {
                        Id = ejecucion.Id,
                        NroError = nroError,
                        Registro = error.ItemId,
                        Referencia = ejecucion.Referencia,
                        Descripcion = messagge
                    });
                }
            }
            await AddEjecucionErrores(iErrores);
            return await Update(ejecucion);
        }

        public virtual async Task<InterfazEjecucion> UpdateAndDeleteErrores(InterfazEjecucion interfaz, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using var tran = connection.BeginTransaction();
                {
                    interfaz = await Update(interfaz, connection, tran, true);
                    await DeleteErrores(interfaz, connection, tran);
                    tran.Commit();
                }
            }

            return interfaz;
        }

        public virtual async Task DeleteErrores(InterfazEjecucion interfaz, DbConnection connection, DbTransaction tran)
        {
            string sql = @"DELETE T_INTERFAZ_EJECUCION_ERROR WHERE NU_INTERFAZ_EJECUCION = :Id";
            await _dapper.ExecuteAsync(connection, sql, param: new { Id = interfaz.Id }, transaction: tran);
        }

        public virtual async Task AddErrores(InterfazEjecucion ejecucion, int nroRegistro, List<string> errores)
        {
            var result = new ValidationsResult();
            result.Errors.Add(new ValidationsError(nroRegistro, true, errores));
            await AddErrores(ejecucion, result.Errors);
        }

        public virtual async Task<bool> ExisteIntefazExterna(int cdInterfazExterna, CancellationToken cancelToken = default)
        {
            string sql = @"SELECT CD_INTERFAZ_EXTERNA FROM T_INTERFAZ_EXTERNA WHERE CD_INTERFAZ_EXTERNA = :cdInterfazExterna";

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var query = _dapper.Query<int?>(connection, sql, param: new
                {
                    cdInterfazExterna = cdInterfazExterna
                }, commandType: CommandType.Text).FirstOrDefault();

                return query != null ? true : false;
            }
        }

        public static string GetSqlSelectInterfazExterna()
        {
            return @"SELECT 
                        iex.CD_INTERFAZ as CodigoInterfaz,
                        iex.CD_INTERFAZ_EXTERNA as CodigoInterfazExterna,
                        iex.DS_INTERFAZ_EXTERNA as Descripcion,
                        iex.DT_ADDROW as FechaALta,
                        iex.FL_RE_PROCESABLE as ReProcesable,
                        iex.ID_SECUENCIA as IdSecuencia,
                        iex.LN_COMIENZO_PROCESO as ComienzoProceso,
                        iex.NM_PROCEDIMIENTO as NombreProcedimiento,
                        iex.NU_RECONO_ORDEN as NuReconoOrden,
                        iex.TP_ARCHIVO as TipoArchivo,
                        iex.VL_DELIMITADOR as Delimitador,
                        iex.VL_DELIMITADOR_SEGMENTO as DelimitadorSegmento,
                        iex.VL_ENDPOINT as Endpoint,
                        iex.VL_ENDPOINT_REPROCESS as EndpointReprocess,
                        iex.VL_PARAMETRO_HABILITACION as ParametroDeHabilitacion,
                        iex.VL_PROC_EXTRAE_SECUENCIA as ProcExtraeSecuencia,
                        iex.VL_RECONO_CONTENIDO as ReconoContenido,
                        iex.VL_RECONO_EXTENSION as ReconoExtension,
                        iex.VL_RECONO_POSTFIJO as ReconoPostfijo,
                        iex.VL_RECONO_PREFIJO as ReconoPrefijo
                    FROM T_INTERFAZ_EXTERNA iex ";
        }
    }
}
