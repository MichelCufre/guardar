using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ClaseRepository
    {
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
          
        protected readonly WISDB _context;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly string _cdAplicacion;
        protected readonly ClaseMapper _claseMapper;
        protected readonly SuperClaseMapper _superClaseMapper;

        public ClaseRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            _context = context;
            _userId = userId;
            _dapper = dapper;
            _cdAplicacion = cdAplicacion;
            _claseMapper = new ClaseMapper();
            _superClaseMapper = new SuperClaseMapper(_claseMapper);
        }

        #region Any

        public virtual bool AnySuperClase(string id)
        {
            return this._context.T_SUB_CLASSE.Any(d => d.CD_SUB_CLASSE == id);
        }

        public virtual bool AnyClase(string id)
        {
            return this._context.T_CLASSE.Any(d => d.CD_CLASSE == id);
        }

        #endregion

        #region Get

        public virtual List<SuperClase> GetSuperClases()
        {
            var entities = this._context.T_SUB_CLASSE.AsNoTracking().ToList();

            var superClases = new List<SuperClase>();

            foreach (var entity in entities)
            {
                entity.T_CLASSE = _context.T_CLASSE.AsNoTracking().Where(s => s.CD_SUB_CLASSE == entity.CD_SUB_CLASSE).ToList();
                superClases.Add(this._superClaseMapper.MapToObject(entity));
            }

            return superClases;
        }
        
        public virtual SuperClase GetSuperClaseById(string id)
        {
            var entity = this._context.T_SUB_CLASSE.AsNoTracking()
                .FirstOrDefault(d => d.CD_SUB_CLASSE == id);

            return this._superClaseMapper.MapToObject(entity);
        }
        
        public virtual string GetDescripcionSuperClase(string id)
        {
            if (string.IsNullOrEmpty(id))
                return string.Empty;
            else
                return this._context.T_SUB_CLASSE.AsNoTracking().FirstOrDefault(d => d.CD_SUB_CLASSE == id)?.DS_SUB_CLASSE;

        }

        public virtual List<Clase> GetClases()
        {
            return this._context.T_CLASSE.AsNoTracking().Select(c => _claseMapper.MapToObject(c)).ToList();
        }
        
        public virtual Clase GetClaseById(string codigo)
        {
            var entity = this._context.T_CLASSE.AsNoTracking()
                .FirstOrDefault(d => d.CD_CLASSE == codigo);

            return this._claseMapper.MapToObject(entity);
        }

        #endregion

        #region Add

        public virtual void AddSuperClase(SuperClase superClase)
        {
            var entity = this._superClaseMapper.MapToEntity(superClase);

            entity.DT_ADDROW = DateTime.Now;
            entity.DT_UPDROW = DateTime.Now;

            this._context.T_SUB_CLASSE.Add(entity);
        }

        public virtual void AddClase(Clase clase)
        {
            var entity = this._claseMapper.MapToEntity(clase);

            entity.DT_ADDROW = DateTime.Now;
            entity.DT_UPDROW = DateTime.Now;

            this._context.T_CLASSE.Add(entity);
        }

        #endregion

        #region Update
       
        public virtual void UpdateSuperClase(SuperClase superClase)
        {
            var entity = this._superClaseMapper.MapToEntity(superClase);
            var attachedEntity = _context.T_SUB_CLASSE.Local.FirstOrDefault(w => w.CD_SUB_CLASSE == entity.CD_SUB_CLASSE);

            entity.DT_UPDROW = DateTime.Now;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_SUB_CLASSE.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }
        
        public virtual void UpdateClase(Clase clase)
        {
            var entity = this._claseMapper.MapToEntity(clase);
            var attachedEntity = _context.T_CLASSE.Local.FirstOrDefault(x => x.CD_CLASSE == entity.CD_CLASSE);

            entity.DT_UPDROW = DateTime.Now;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                this._context.T_CLASSE.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void RemoveSuperClase(SuperClase superClase)
        {
            var entity = this._superClaseMapper.MapToEntity(superClase);
            var attachedEntity = _context.T_SUB_CLASSE.Local.FirstOrDefault(w => w.CD_SUB_CLASSE == entity.CD_SUB_CLASSE);

            if (attachedEntity != null)
            {
                this._context.T_SUB_CLASSE.Remove(attachedEntity);
            }
            else
            {
                this._context.T_SUB_CLASSE.Attach(entity);
                this._context.T_SUB_CLASSE.Remove(entity);
            }
        }

        public virtual void RemoveClase(Clase clase)
        {
            var entity = this._claseMapper.MapToEntity(clase);
            var attachedEntity = _context.T_CLASSE.Local.FirstOrDefault(x => x.CD_CLASSE == entity.CD_CLASSE);

            if (attachedEntity != null)
            {
                this._context.T_CLASSE.Remove(attachedEntity);
            }
            else
            {
                this._context.T_CLASSE.Attach(entity);
                this._context.T_CLASSE.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        #endregion
    }
}
