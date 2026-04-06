using Microsoft.EntityFrameworkCore;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Impresiones;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class LenguajeImpresionRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly LenguajeImpresionMapper _mapper;

        public LenguajeImpresionRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new LenguajeImpresionMapper();
        }

        #region Any

        public virtual bool ExisteLenguajeImpresion(string idLenguajeImpresion)
        {
            return this._context.T_LENGUAJE_IMPRESION.Any(cb => cb.CD_LENGUAJE_IMPRESION == idLenguajeImpresion);
        }

        #endregion

        #region Get

        public virtual LenguajeImpresion GetLenguajeImpresion(string idLenguajeImpresion)
        {
            return this._mapper.MapToObject(this._context.T_LENGUAJE_IMPRESION.FirstOrDefault(x => x.CD_LENGUAJE_IMPRESION == idLenguajeImpresion));
        }

        #endregion

        #region Add

        public virtual void AddLenguajeImpresion(LenguajeImpresion lenguajeImpresion)
        {
            T_LENGUAJE_IMPRESION entity = this._mapper.MapToEntity(lenguajeImpresion);
            this._context.T_LENGUAJE_IMPRESION.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateLenguajeImpresion(LenguajeImpresion lenguajeImpresion)
        {
            T_LENGUAJE_IMPRESION entity = this._mapper.MapToEntity(lenguajeImpresion);
            T_LENGUAJE_IMPRESION attachedEntity = _context.T_LENGUAJE_IMPRESION.Local
                .FirstOrDefault(w => w.CD_LENGUAJE_IMPRESION == entity.CD_LENGUAJE_IMPRESION);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_LENGUAJE_IMPRESION.Attach(entity);
                _context.Entry<T_LENGUAJE_IMPRESION>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void DeleteLenguajeImpresion(string idLenguajeImpresion)
        {
            T_LENGUAJE_IMPRESION entity = this._context.T_LENGUAJE_IMPRESION
                .FirstOrDefault(x => x.CD_LENGUAJE_IMPRESION == idLenguajeImpresion);
            T_LENGUAJE_IMPRESION attachedEntity = _context.T_LENGUAJE_IMPRESION.Local
                .FirstOrDefault(w => w.CD_LENGUAJE_IMPRESION == entity.CD_LENGUAJE_IMPRESION);

            if (attachedEntity != null)
            {
                _context.T_LENGUAJE_IMPRESION.Remove(attachedEntity);
            }
            else
            {
                _context.T_LENGUAJE_IMPRESION.Remove(entity);
            }
        }

        #endregion

    }
}
