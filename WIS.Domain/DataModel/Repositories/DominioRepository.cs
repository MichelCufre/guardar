using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class DominioRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly DominioMapper _mapper;
        protected readonly IDapper _dapper;

        public DominioRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._userId = userId;
            this._dapper = dapper;
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._mapper = new DominioMapper();
        }

        #region Any

        public virtual bool DominioInterno(string cdDominimio)
        {
            return (_context.T_DET_DOMINIO
                .Include("T_DOMINIO")
                .AsNoTracking()
                .FirstOrDefault(w => w.CD_DOMINIO == cdDominimio)?.T_DOMINIO?.FL_INTERNO_WIS ?? "N") == "S";
        }

        public virtual bool ExisteDominio(string codigoDominio)
        {
            return _context.T_DOMINIO.Any(w => w.CD_DOMINIO == codigoDominio);
        }

        public virtual bool ExisteDetalleDominio(string nuDominio)
        {
            return _context.T_DET_DOMINIO.Any(w => w.NU_DOMINIO == nuDominio);
        }

        public virtual bool ExisteDetalleDominio(string cdDominio, string nuDominio)
        {
            return _context.T_DET_DOMINIO.Any(w => w.CD_DOMINIO == cdDominio && w.NU_DOMINIO == nuDominio);
        }

        public virtual bool ExisteDetalleDominioValor(string cdDominio, string cdDominioValor)
        {
            return _context.T_DET_DOMINIO.Any(w => w.CD_DOMINIO == cdDominio && w.CD_DOMINIO_VALOR == cdDominioValor);
        }

        #endregion

        #region Get
        public virtual DominioDetalle GetDominio(string codigo, string valor)
        {
            var dominio = this._context.T_DET_DOMINIO.AsNoTracking().FirstOrDefault(d => d.CD_DOMINIO == codigo && d.CD_DOMINIO_VALOR == valor);

            return this._mapper.MapToObject(dominio);
        }

        public virtual DominioDetalle GetDominio(string id)
        {
            var dominio = this._context.T_DET_DOMINIO.AsNoTracking().FirstOrDefault(d => d.NU_DOMINIO == id);

            return this._mapper.MapToObject(dominio);
        }

        public virtual List<Dominio> GetAllDominios()
        {
            var dominios = new List<Dominio>();

            List<T_DOMINIO> entries = this._context.T_DOMINIO.Include("T_DET_DOMINIO").AsNoTracking().OrderBy(d => d.CD_DOMINIO).ToList();

            foreach (var entry in entries)
            {
                dominios.Add(this._mapper.MapToObject(entry));
            }

            return dominios;
        }

        public virtual List<DominioDetalle> GetDominios(string cdDominio)
        {
            return this._context.T_DET_DOMINIO
                 .AsNoTracking()
                 .Where(d => d.CD_DOMINIO == cdDominio)
                 .Select(d => _mapper.MapToObject(d))
                 .ToList();
        }

        #endregion

        #region Add

        public virtual void AddDetalleDominioEmpresa(int idEmpresa)
        {
            this._context.T_DET_DOMINIO.Add(new T_DET_DOMINIO()
            {
                CD_DOMINIO = ParamManager.PARAM_EMPR,
                NU_DOMINIO = $"{ParamManager.PARAM_EMPR}_{idEmpresa}",
                CD_DOMINIO_VALOR = idEmpresa.ToString(),
                DS_DOMINIO_VALOR = $"Empresa {idEmpresa}"
            });
        }

        public virtual void AddDetalleDominioUsuario(int userId)
        {
            this._context.T_DET_DOMINIO.Add(new T_DET_DOMINIO
            {
                CD_DOMINIO = ParamManager.PARAM_USER,
                NU_DOMINIO = $"{ParamManager.PARAM_USER}_{userId}",
                CD_DOMINIO_VALOR = userId.ToString(),
                DS_DOMINIO_VALOR = "Usuario: " + userId.ToString()
            });
        }

        public virtual void AddDetalleDominio(DominioDetalle nuevoDetalle)
        {
            T_DET_DOMINIO entity = this._mapper.MapToEntity(nuevoDetalle);
            this._context.T_DET_DOMINIO.Add(entity);
        }

        public virtual void AddDetalleDominioPredio(string cdDominioValor, string dsDominioValor)
        {
            this._context.T_DET_DOMINIO.Add(new T_DET_DOMINIO
            {
                NU_DOMINIO = $"{ParamManager.PARAM_PRED}_{cdDominioValor}",
                CD_DOMINIO = ParamManager.PARAM_PRED,
                CD_DOMINIO_VALOR = cdDominioValor,
                DS_DOMINIO_VALOR = dsDominioValor
            });
        }

        public virtual void AddDetalleDominioZona(int idZona, string cdZona)
        {
            this._context.T_DET_DOMINIO.Add(new T_DET_DOMINIO()
            {
                CD_DOMINIO = ParamManager.PARAM_ZONA,
                NU_DOMINIO = $"{ParamManager.PARAM_ZONA}_{idZona}",
                CD_DOMINIO_VALOR = idZona.ToString(),
                DS_DOMINIO_VALOR = cdZona
            });
        }

        #endregion

        #region Update

        public virtual void UpdateDetalleDominio(DominioDetalle detalleDominio)
        {
            T_DET_DOMINIO entity = this._mapper.MapToEntity(detalleDominio);
            T_DET_DOMINIO attachedEntity = _context.T_DET_DOMINIO.Local
                .FirstOrDefault(c => c.NU_DOMINIO.ToUpper().Trim() == detalleDominio.Codigo.ToUpper().Trim());

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_DET_DOMINIO.Attach(entity);
                _context.Entry<T_DET_DOMINIO>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void RemoveDominioPredio(string cdDominioValor)
        {
            T_DET_DOMINIO entity = this._context.T_DET_DOMINIO
                .FirstOrDefault(x => x.CD_DOMINIO_VALOR == cdDominioValor
                    && x.CD_DOMINIO == ParamManager.PARAM_PRED);

            T_DET_DOMINIO attachedEntity = _context.T_DET_DOMINIO.Local
                .FirstOrDefault(w => w.CD_DOMINIO_VALOR == entity.CD_DOMINIO_VALOR
                    && w.CD_DOMINIO == entity.CD_DOMINIO);

            if (attachedEntity != null)
            {
                _context.T_DET_DOMINIO.Remove(attachedEntity);
            }
            else
            {
                _context.T_DET_DOMINIO.Remove(entity);
            }
        }

        public virtual void DeleteDetalleDominio(string numeroDetalle)
        {
            T_DET_DOMINIO entity = this._context.T_DET_DOMINIO.FirstOrDefault(x => x.NU_DOMINIO == numeroDetalle);
            T_DET_DOMINIO attachedEntity = _context.T_DET_DOMINIO.Local.FirstOrDefault(w => w.NU_DOMINIO == entity.NU_DOMINIO);

            if (attachedEntity != null)
            {
                _context.T_DET_DOMINIO.Remove(attachedEntity);
            }
            else
            {
                _context.T_DET_DOMINIO.Attach(entity);
                _context.T_DET_DOMINIO.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        public virtual IEnumerable<DominioDetalle> GetDetallesDominio(IEnumerable<DominioDetalle> dominios)
        {
            IEnumerable<DominioDetalle> resultado = new List<DominioDetalle>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_DET_DOMINIO_TEMP (CD_DOMINIO) VALUES (:Codigo)";
                    _dapper.Execute(connection, sql, dominios, transaction: tran);

                    sql = GetSqlSelectDominioDetalle() +
                        @" INNER JOIN T_DET_DOMINIO_TEMP T ON dd.CD_DOMINIO = T.CD_DOMINIO ";

                    resultado = _dapper.Query<DominioDetalle>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public static string GetSqlSelectDominioDetalle()
        {
            return @"SELECT 
	                    dd.NU_DOMINIO as Id,
	                    dd.CD_DOMINIO as Codigo,
	                    dd.CD_DOMINIO_VALOR as Descripcion,
	                    dd.DS_DOMINIO_VALOR as Valor
                    FROM T_DET_DOMINIO dd ";
        }

        #endregion
    }
}
