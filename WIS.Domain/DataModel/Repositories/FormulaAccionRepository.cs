using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Produccion;
using WIS.Domain.Extensions;
using WIS.Domain.Produccion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
	public class FormulaAccionRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly FormulaAccionMapper _mapper;
        protected readonly IDapper _dapper;

        public FormulaAccionRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new FormulaAccionMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyFormulaAccion(short id)
        {
            return this._context.T_PRDC_ACCION_INSTANCIA
                .Any(d => d.CD_ACCION_INSTANCIA == id);
        }

        #endregion

        #region Get
        
        public virtual FormulaAccion GetFormulaAccion(int id)
        {
            T_PRDC_ACCION_INSTANCIA entity = this._context.T_PRDC_ACCION_INSTANCIA
                .AsNoTracking()
                .Where(d => d.CD_ACCION_INSTANCIA == id)
                .FirstOrDefault();

            return this._mapper.MapEntityToObject(entity);
        }
        
        public virtual List<FormulaAccion> GetAccionByDescriptionPartial(string value)
        {
            var acciones = new List<FormulaAccion>();

            List<T_PRDC_ACCION_INSTANCIA> entityList = this._context.T_PRDC_ACCION_INSTANCIA
                .AsNoTracking()
                .Where(d => d.DS_ACCION_INSTANCIA.ToLower().Contains(value.ToLower()))
                .ToList();

            foreach (var entity in entityList)
            {
                acciones.Add(this._mapper.MapEntityToObject(entity));
            }

            return acciones;
        }
        
        public virtual int GetNumeroInstancia()
        {
            return this._context.GetNextSequenceValueInt(_dapper, "S_ACCION_INSTANCIA");
        }

        #endregion

        #region Add

        public virtual void AddFormulaAccion(FormulaAccion instancia)
        {
            T_PRDC_ACCION_INSTANCIA entity = this._mapper.MapObjetcToEntity(instancia);

            this._context.T_PRDC_ACCION_INSTANCIA.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateFormulaAccion(FormulaAccion instancia)
        {
            var entity = this._mapper.MapObjetcToEntity(instancia);
            var attachedEntity = _context.T_PRDC_ACCION_INSTANCIA.Local
                .FirstOrDefault(x => x.CD_ACCION_INSTANCIA == entity.CD_ACCION_INSTANCIA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_PRDC_ACCION_INSTANCIA.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        #endregion

        #region Dapper

        #endregion
    }
}
