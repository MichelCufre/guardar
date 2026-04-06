using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Produccion;
using WIS.Domain.Produccion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class AccionRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly AccionMapper _mapper;

        public AccionRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new AccionMapper();
        }

        #region Any

        #endregion

        #region Get

        public virtual Accion GetAccion(string codigo)
        {
            T_PRDC_ACCION entity = this._context.T_PRDC_ACCION
                .FirstOrDefault(c => c.CD_ACCION == codigo);

            return this._mapper.MapEntityToObject(entity);
        }

        public virtual List<Accion> GetAccionesPorCodigo(List<string> ids)
        {
            List<Accion> acciones = new List<Accion>();

            List<T_PRDC_ACCION> entities = this._context.T_PRDC_ACCION
                .Where(d => ids.Contains(d.CD_ACCION))
                .ToList();

            foreach (var entity in entities)
            {
                acciones.Add(this._mapper.MapEntityToObject(entity));
            }

            return acciones;
        }

        public virtual List<Accion> GetAccionesPorCodigoDescripcion(string searchValue)
        {
            List<Accion> acciones = new List<Accion>();
            List<T_PRDC_ACCION> entities = this._context.T_PRDC_ACCION
                .Where(d => (d.CD_ACCION.ToLower().Contains(searchValue.ToLower()) || d.DS_ACCION.ToLower().Contains(searchValue.ToLower())))
                .ToList();

            if (entities == null || entities.Count() == 0)
                return null;

            foreach (var entity in entities)
            {
                acciones.Add(this._mapper.MapEntityToObject(entity));
            }

            return acciones;
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
