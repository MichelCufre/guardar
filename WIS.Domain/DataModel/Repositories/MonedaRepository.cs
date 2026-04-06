using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class MonedaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly MonedaMapper _mapper;

        public MonedaRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new MonedaMapper();
        }

        #region Any

        public virtual bool ExisteMoneda(string moneda)
        {
            return this._context.T_MONEDA
                .AsNoTracking()
                .Any(w => w.CD_MONEDA == moneda);
        }

        #endregion

        #region Get

        public virtual string GetDescripcion(string cdMoneda)
        {
            return this._context.T_MONEDA
                .AsNoTracking()
                .FirstOrDefault(m => m.CD_MONEDA == cdMoneda)?.DS_MONEDA;
        }

        public virtual Moneda GetMoneda(string cdMoneda)
        {
            return this._mapper.MapToObject(this._context.T_MONEDA
                .AsNoTracking()
                .FirstOrDefault(f => f.CD_MONEDA == cdMoneda));
        }

        public virtual List<Moneda> GetMonedas()
        {
            var entities = this._context.T_MONEDA.AsNoTracking().ToList();

            var tipos = new List<Moneda>();

            foreach (var entity in entities)
            {
                tipos.Add(this._mapper.MapToObject(entity));
            }

            return tipos;
        }

        public virtual List<Moneda> GetMonedaByNombre(string nombre)
        {
            return this._context.T_MONEDA
                .AsNoTracking()
                .Where(d => d.DS_MONEDA.ToLower().Contains(nombre.ToLower()))
                .Select(d => this._mapper.MapToObject(d))
                .ToList();
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
