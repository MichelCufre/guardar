using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class PaisSubdivisionLocalidadRepository
    {
        protected readonly int _userId;
        protected readonly WISDB _context;
        protected readonly IDapper _dapper;
        protected readonly string _cdAplicacion;
        protected readonly PaisSubdivisionLocalidadMapper _mapper;

        public PaisSubdivisionLocalidadRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new PaisSubdivisionLocalidadMapper(new PaisSubdivisionMapper(new PaisMapper()));
            _dapper = dapper;
        }
        
        /// <summary>
        /// Incluye Subdivisión y País
        /// </summary>
        /// <param name="idLocalidad"> Id de localidad ej: 10</param>
        /// <returns></returns>
        public virtual PaisSubdivisionLocalidad GetLocalidad(long idLocalidad)
        {
            T_PAIS_SUBDIVISION_LOCALIDAD localidad = this._context.T_PAIS_SUBDIVISION_LOCALIDAD
                .Include("T_PAIS_SUBDIVISION")
                .Include("T_PAIS_SUBDIVISION.T_PAIS")
                .FirstOrDefault(d => d.ID_LOCALIDAD == idLocalidad);

            return this._mapper.MapToObject(localidad);
        }

        /// <summary>
        /// Retorna la localidad sin País o Subdivisión
        /// </summary>
        /// <param name="codigoSubdivision">Código de subdivisión ej: UY-MA</param>
        /// <param name="codigoLocalidad">Código de localidad ej: PDS, S/E</param>
        /// <returns>PaisSubdivisionLocalidad</returns>
        public virtual PaisSubdivisionLocalidad GetLocalidad(string codigoSubdivision, string codigoLocalidad)
        {
            T_PAIS_SUBDIVISION_LOCALIDAD localidad = this._context.T_PAIS_SUBDIVISION_LOCALIDAD.AsNoTracking()
                .FirstOrDefault(d => d.CD_SUBDIVISION == codigoSubdivision && d.CD_LOCALIDAD == codigoLocalidad);

            return this._mapper.MapToObject(localidad);
        }

        /// <summary>
        /// Retorna la localidad sin País o Subdivisión
        /// </summary>
        /// <param name="codigoPais">Código de país ej: UY</param>
        /// <param name="codigoSubdivision">Código de subdivisión ej: UY-MA</param>
        /// <param name="codigoLocalidad">Código de localidad ej: PDS, S/E</param>
        /// <returns></returns>
        public virtual PaisSubdivisionLocalidad GetLocalidad(string codigoPais, string codigoSubdivision, string codigoLocalidad)
        {
            T_PAIS_SUBDIVISION_LOCALIDAD localidad = this._context.T_PAIS_SUBDIVISION_LOCALIDAD.AsNoTracking()
                .FirstOrDefault(d => d.T_PAIS_SUBDIVISION.CD_PAIS == codigoPais
                                  && d.CD_SUBDIVISION == codigoSubdivision
                                  && d.CD_LOCALIDAD == codigoLocalidad);

            return this._mapper.MapToObject(localidad);
        }

        public virtual List<PaisSubdivisionLocalidad> GetByNombreOrIdPartial(string value)
        {
            if (long.TryParse(value, out long idLocalidad))
            {
                return this._context.T_PAIS_SUBDIVISION_LOCALIDAD.AsNoTracking()
                .Where(d => d.NM_LOCALIDAD.ToLower().Contains(value.ToLower()) || d.ID_LOCALIDAD == idLocalidad)
                .ToList().Select(d => this._mapper.MapToObject(d)).ToList();
            }
            else
            {
                return this._context.T_PAIS_SUBDIVISION_LOCALIDAD.AsNoTracking()
                .Where(d => d.NM_LOCALIDAD.ToLower().Contains(value.ToLower()))
                .ToList().Select(d => this._mapper.MapToObject(d)).ToList();
            }

        }
        public virtual List<PaisSubdivisionLocalidad> GetByNombreOrIdPartial(string value, string codigoPais, string codigoSubdivision)
        {
            var query = this._context.T_PAIS_SUBDIVISION_LOCALIDAD.AsNoTracking().Select(s => s);

            if (long.TryParse(value, out long idLocalidad))
            {
                query = query.Where(d => d.NM_LOCALIDAD.ToLower().Contains(value.ToLower()) || d.ID_LOCALIDAD == idLocalidad);
            }
            else
            {
                query = query.Where(d => d.NM_LOCALIDAD.ToLower().Contains(value.ToLower()));
            }

            if (!string.IsNullOrEmpty(codigoPais))
                query = query.Where(d => d.T_PAIS_SUBDIVISION.T_PAIS.CD_PAIS == codigoPais);

            if (!string.IsNullOrEmpty(codigoSubdivision))
                query = query.Where(d => d.CD_SUBDIVISION == codigoSubdivision);

            return query.ToList().Select(d => this._mapper.MapToObject(d)).ToList();
        }

        public virtual bool AnyLocalidad(long value)
        {
            return this._context.T_PAIS_SUBDIVISION_LOCALIDAD.Any(d => d.ID_LOCALIDAD == value);
        }
        public virtual bool AnyLocalidad(long value, string codigoSubdivision)
        {
            return this._context.T_PAIS_SUBDIVISION_LOCALIDAD.Any(d => d.ID_LOCALIDAD == value && d.T_PAIS_SUBDIVISION.CD_SUBDIVISION == codigoSubdivision);
        }
        public virtual bool AnyLocalidadEnPais(long value, string codigoPais)
        {
            return this._context.T_PAIS_SUBDIVISION_LOCALIDAD.Any(d => d.ID_LOCALIDAD == value && d.T_PAIS_SUBDIVISION.T_PAIS.CD_PAIS == codigoPais);
        }


        #region Dapper

        public virtual Dictionary<string, PaisSubdivisionLocalidad> GetLocalidadesSubdivisiones(IEnumerable<PaisSubdivisionLocalidad> localidades)
        {
            Dictionary<string, PaisSubdivisionLocalidad> resultado = new Dictionary<string, PaisSubdivisionLocalidad>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PAIS_SUBDIVISION_LOCALIDAD_TEMP (CD_LOCALIDAD, CD_SUBDIVISION) VALUES (:Codigo, :CodigoSubDivicion)";
                    _dapper.Execute(connection, sql, localidades, transaction: tran);

                    sql = @"SELECT L.CD_LOCALIDAD AS Codigo
                        , L.CD_SUBDIVISION AS CodigoSubDivicion
                        , L.ID_LOCALIDAD AS Id
                        , L.NM_LOCALIDAD AS Nombre
                        , L.CD_IATA AS CodigoIATA
                        , L.CD_POSTAL AS CodigoPostal
                        FROM T_PAIS_SUBDIVISION_LOCALIDAD L INNER JOIN T_PAIS_SUBDIVISION_LOCALIDAD_TEMP T ON L.CD_LOCALIDAD = T.CD_LOCALIDAD 
                            AND L.CD_SUBDIVISION = T.CD_SUBDIVISION";

                    foreach (var l in _dapper.Query<PaisSubdivisionLocalidad>(connection, sql, transaction: tran))
                    {
                        var keyLocalidad = $"{l.Codigo}.{l.CodigoSubDivicion}";
                        resultado[keyLocalidad] = l;
                    }

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual async Task<PaisSubdivisionLocalidad> GetLocalidadId(string cdLocalidad, string cdSubdivision, DbConnection connect = null, CancellationToken cancelToken = default)
        {
            using (var connection = connect != null ? connect : this._dapper.GetDbConnection())
            {
                var values = new { CodigoLocalidad = cdLocalidad, cdSubdivision = cdSubdivision };
                var parameters = new DynamicParameters(values);
                await connection.OpenAsync(cancelToken);

                string sql =
                @"SELECT ID_LOCALIDAD as Id,
                    CD_LOCALIDAD as Codigo,
                    CD_SUBDIVISION as CodigoSubDivicion,
                    NM_LOCALIDAD as Nombre,
                    CD_IATA as CodigoIATA,
                    CD_POSTAL as CodigoPostal
                FROM T_PAIS_SUBDIVISION_LOCALIDAD
                WHERE CD_LOCALIDAD = :CodigoLocalidad AND CD_SUBDIVISION=:cdSubdivision";

                var query = _dapper.Query<PaisSubdivisionLocalidad>(connection, sql, param: parameters, commandType: CommandType.Text);

                return query.FirstOrDefault();
            }
        }
        #endregion
    }
}
