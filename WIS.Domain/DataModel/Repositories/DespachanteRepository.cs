using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class DespachanteRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly DespachanteMapper _mapper;

        public DespachanteRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new DespachanteMapper();
        }

        #region Any

        public virtual bool AnyDespachante(int cdDespachante)
        {
            return this._context.T_DESPACHANTE
                .AsNoTracking()
                .Any(d => d.CD_DESPACHANTE == cdDespachante);
        }

        #endregion

        #region Get

        public virtual Despachante GetDespachante(short cdDespachante)
        {
            var result = this._context.T_DESPACHANTE
                .AsNoTracking()
                .Where(d => d.CD_DESPACHANTE == cdDespachante)
                .FirstOrDefault();

            return this._mapper.MapToObject(result);
        }

        public virtual List<Despachante> GetDespachanteByNombrePartial(string nombre)
        {
            return this._context.T_DESPACHANTE
                .AsNoTracking()
                .Where(d => ((d.CD_DESPACHANTE.ToString().Contains(nombre.ToLower()))
                            || d.NM_DESPACHANTE.ToLower().Contains(nombre.ToLower())))
                .Select(d => this._mapper.MapToObject(d))
                .ToList();
        }

        public virtual string GetNombreDespachante(int cdDespachante)
        {
            return this._context.T_DESPACHANTE
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_DESPACHANTE == cdDespachante)?.NM_DESPACHANTE;
        }

        #endregion

        #region Add

        #endregion

        #region Update

        #endregion

        #region Remove

        #endregion

        #region Dapper

        #endregion
    }
}
