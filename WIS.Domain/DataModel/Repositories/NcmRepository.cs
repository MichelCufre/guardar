using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class NcmRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly NCMMaper _mapper;

        public NcmRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new NCMMaper();
        }

        #region Any
        
        public virtual bool ExisteNCM(string id)
        {
            return this._context.T_NAM.Any(cb => cb.CD_NAM == id);
        }
        
        #endregion

        #region Get

        public virtual List<CodigoNomenclaturaComunMercosur> GetByNombreOrCodePartial(string value)
        {
            return this._context.T_NAM.Where(x => x.CD_NAM.ToLower().Contains(value.ToLower()) ||
            x.DS_NAM.ToLower().Contains(value.ToLower())).ToList().Select(x => this._mapper.MapToObject(x)).ToList();
        }

        public virtual CodigoNomenclaturaComunMercosur GetNCM(string id)
        {
            return this._mapper.MapToObject(this._context.T_NAM.FirstOrDefault(x => x.CD_NAM == id));
        }

        public virtual List<CodigoNomenclaturaComunMercosur> GetNCMs()
        {
            var entities = this._context.T_NAM.AsNoTracking()
               .ToList();

            var ncms = new List<CodigoNomenclaturaComunMercosur>();

            foreach (var entity in entities)
            {
                ncms.Add(this._mapper.MapToObject(entity));
            }

            return ncms;
        }

        #endregion

        #region Add

        public virtual void AddNCM(CodigoNomenclaturaComunMercosur ncm)
        {
            T_NAM entity = this._mapper.MapToEntity(ncm);
            this._context.T_NAM.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateNCM(CodigoNomenclaturaComunMercosur ncm)
        {
            T_NAM entity = this._mapper.MapToEntity(ncm);
            T_NAM attachedEntity = _context.T_NAM.Local
                .FirstOrDefault(w => w.CD_NAM == entity.CD_NAM);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_NAM.Attach(entity);
                _context.Entry<T_NAM>(entity).State = EntityState.Modified;
            }
        }
        
        #endregion

        #region Remove
        
        public virtual void DeleteNCM(string id)
        {
            T_NAM entity = this._context.T_NAM.FirstOrDefault(x => x.CD_NAM == id);
            T_NAM attachedEntity = _context.T_NAM.Local
                .FirstOrDefault(w => w.CD_NAM == entity.CD_NAM);

            if (attachedEntity != null)
            {
                _context.T_NAM.Remove(attachedEntity);
            }
            else
            {
                _context.T_NAM.Attach(entity);
                _context.T_NAM.Remove(entity);
            }
        }
        
        #endregion
    }
}
