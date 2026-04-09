using Custom.Persistence.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using WIS.Domain.Services.Interfaces;

namespace Custom.Domain.DataModel.Repositories
{
    public class MiddlewareColaRepository
    {
        private readonly IDapper _dapper;

        public MiddlewareColaRepository(IDapper dapper)
        {
            _dapper = dapper;
        }

        public void Encolar(string tipo, string payload)
        {
            var sql = @"INSERT INTO T_MIDDLEWARE_COLA 
                            (TP_COLA, DS_PAYLOAD, CD_ESTADO, DT_ADDROW, NU_INTENTOS)
                        VALUES 
                            (:tipo, :payload, 'PENDIENTE', :fecha, 0)";

            var param = new DynamicParameters(new
            {
                tipo    = tipo,
                payload = payload,
                fecha   = DateTime.Now
            });

            using (var conn = _dapper.GetDbConnection())
            {
                conn.Open();
                _dapper.Execute(conn, sql, param: param, commandType: CommandType.Text);
            }
        }

        public IEnumerable<MiddlewareColaItem> GetPendientes(int maxItems = 50)
        {
            var sql = @"SELECT NU_COLA, TP_COLA, DS_PAYLOAD, CD_ESTADO, NU_INTENTOS
                        FROM T_MIDDLEWARE_COLA
                        WHERE CD_ESTADO = 'PENDIENTE'
                        ORDER BY DT_ADDROW
                        FETCH FIRST :maxItems ROWS ONLY";

            var param = new DynamicParameters(new { maxItems });

            using (var conn = _dapper.GetDbConnection())
            {
                conn.Open();
                return _dapper.Query<MiddlewareColaItem>(conn, sql, param: param, commandType: CommandType.Text);
            }
        }

        public void MarcarProcesado(long id)
        {
            var sql = @"UPDATE T_MIDDLEWARE_COLA 
                        SET CD_ESTADO     = 'PROCESADO',
                            DT_PROCESADO = :fecha
                        WHERE NU_COLA = :id";

            var param = new DynamicParameters(new { id, fecha = DateTime.Now });

            using (var conn = _dapper.GetDbConnection())
            {
                conn.Open();
                _dapper.Execute(conn, sql, param: param, commandType: CommandType.Text);
            }
        }

        public void MarcarError(long id, string error)
        {
            var sql = @"UPDATE T_MIDDLEWARE_COLA
                        SET NU_INTENTOS = NU_INTENTOS + 1,
                            DS_ERROR    = :error,
                            CD_ESTADO   = CASE WHEN NU_INTENTOS + 1 >= 3 THEN 'ERROR' ELSE 'PENDIENTE' END
                        WHERE NU_COLA = :id";

            var err   = error?.Length > 2000 ? error.Substring(0, 2000) : error;
            var param = new DynamicParameters(new { id, error = err });

            using (var conn = _dapper.GetDbConnection())
            {
                conn.Open();
                _dapper.Execute(conn, sql, param: param, commandType: CommandType.Text);
            }
        }
    }

    // DTO liviano para leer filas de la cola
    public class MiddlewareColaItem
    {
        public long   NU_COLA { get; set; }
        public string TP_COLA { get; set; }
        public string DS_PAYLOAD { get; set; }
        public string CD_ESTADO { get; set; }
        public int NU_INTENTOS { get; set; }
    }

    public static class MiddlewareColaEstado
    {
        public const string Pendiente = "PENDIENTE";
        public const string Procesado = "PROCESADO";
        public const string Error = "ERROR";
    }

    public static class MiddlewareColaTipo
    {
        public const string Producto     = "PRODUCTO";
        public const string Agente       = "AGENTE";
        public const string CodigoBarras = "CODIGOBARRAS";
    }
}
