using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class PaisSubdivisionRepository
    {
        protected readonly WISDB _context;
        protected readonly IDapper _dapper;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly PaisSubdivisionMapper _mapper;

        public PaisSubdivisionRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new PaisSubdivisionMapper(new PaisMapper());
            this._dapper = dapper;
        }
         
        public virtual PaisSubdivision GetPaisSubdivision(string idPaisSubdivision)
        {
            T_PAIS_SUBDIVISION subdivision = this._context.T_PAIS_SUBDIVISION.Include("T_PAIS")
                .FirstOrDefault(d => d.CD_SUBDIVISION == idPaisSubdivision);

            return this._mapper.MapToObject(subdivision);
        }

        public virtual List<PaisSubdivision> GetByNombreOrIdPartial(string value)
        {
            return this._context.T_PAIS_SUBDIVISION.AsNoTracking()
                .Where(d => d.NM_SUBDIVISION.ToLower().Contains(value.ToLower()) || d.CD_SUBDIVISION == value)
                .ToList().Select(d => this._mapper.MapToObject(d)).ToList();
        }
        public virtual List<PaisSubdivision> GetByNombreOrIdPartial(string value, string codigoPais)
        {
            return this._context.T_PAIS_SUBDIVISION.AsNoTracking()
                .Where(d => d.CD_PAIS == codigoPais)
                .Where(d => d.NM_SUBDIVISION.ToLower().Contains(value.ToLower()) || d.CD_SUBDIVISION == value)
                .ToList().Select(d => this._mapper.MapToObject(d)).ToList();
        }
        public virtual bool AnyPaisSubdivision(string value)
        {
            return this._context.T_PAIS_SUBDIVISION.Any(d => d.CD_SUBDIVISION == value);
        }
        public virtual bool AnyPaisSubdivision(string value, string codigoPais)
        {
            return this._context.T_PAIS_SUBDIVISION.Any(d => d.CD_SUBDIVISION == value && d.CD_PAIS == codigoPais);
        }

        #region Dapper

        public virtual Dictionary<string, string> GetSubdivisionesPaises(IEnumerable<PaisSubdivision> subdivisiones)
        {
            Dictionary<string, string> resultado = new Dictionary<string, string>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PAIS_SUBDIVISION_TEMP (CD_SUBDIVISION) VALUES (:Id)";
                    _dapper.Execute(connection, sql, subdivisiones, transaction: tran);

                    sql = @"SELECT S.CD_SUBDIVISION AS Id
                        , S.CD_PAIS AS IdPais
                        FROM T_PAIS_SUBDIVISION S INNER JOIN T_PAIS_SUBDIVISION_TEMP T ON S.CD_SUBDIVISION = T.CD_SUBDIVISION";

                    foreach (var s in _dapper.Query<PaisSubdivision>(connection, sql, transaction: tran))
                    {
                        resultado[s.Id] = s.IdPais;
                    }

                    tran.Rollback();
                }
            }

            return resultado;
        }

        #endregion
    }
}
