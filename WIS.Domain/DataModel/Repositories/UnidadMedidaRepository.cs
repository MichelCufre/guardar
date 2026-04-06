using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class UnidadMedidaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly UnidadMedidaMapper _mapper;

        public UnidadMedidaRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new UnidadMedidaMapper();
        }

        #region Add
        public virtual void AddUnidadMedida(UnidadMedida unidadMedida)
        {
            T_UNIDADE_MEDIDA entity = this._mapper.MapToEntity(unidadMedida);
            this._context.T_UNIDADE_MEDIDA.Add(entity);
        }
        #endregion

        #region Remove
        public virtual void DeleteUnidadMedida(string cdUnidadMedida)
        {
            T_UNIDADE_MEDIDA entity = this._context.T_UNIDADE_MEDIDA
                .FirstOrDefault(x => x.CD_UNIDADE_MEDIDA == cdUnidadMedida);
            T_UNIDADE_MEDIDA attachedEntity = _context.T_UNIDADE_MEDIDA.Local
                .FirstOrDefault(w => w.CD_UNIDADE_MEDIDA == entity.CD_UNIDADE_MEDIDA);

            if (attachedEntity != null)
            {
                _context.T_UNIDADE_MEDIDA.Remove(attachedEntity);
            }
            else
            {
                _context.T_UNIDADE_MEDIDA.Remove(entity);
            }
        }
        #endregion

        #region Update
        public virtual void UpdateUnidadMedida(UnidadMedida unidadMedida)
        {
            T_UNIDADE_MEDIDA entity = this._mapper.MapToEntity(unidadMedida);
            T_UNIDADE_MEDIDA attachedEntity = _context.T_UNIDADE_MEDIDA.Local
                .FirstOrDefault(w => w.CD_UNIDADE_MEDIDA == entity.CD_UNIDADE_MEDIDA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_UNIDADE_MEDIDA.Attach(entity);
                _context.Entry<T_UNIDADE_MEDIDA>(entity).State = EntityState.Modified;
            }
        }
        #endregion

        #region Any
        public virtual bool ExisteUnidadMedida(string cdUnidadMedida)
        {
            return this._context.T_UNIDADE_MEDIDA
                .AsNoTracking()
                .Any(cb => cb.CD_UNIDADE_MEDIDA == cdUnidadMedida);
        }
        #endregion

        #region Get

        public virtual UnidadMedida GetUnidadMedida(string cdUnidadMedida)
        {
            var result = this._context.T_UNIDADE_MEDIDA
                .AsNoTracking()
                .Where(d => d.CD_UNIDADE_MEDIDA == cdUnidadMedida)
                .FirstOrDefault();

            return this._mapper.MapToObject(result);
        }

        public virtual string GetDescripcion(string cdUnidadMedida)
        {
            return this._context.T_UNIDADE_MEDIDA
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_UNIDADE_MEDIDA == cdUnidadMedida)?.DS_UNIDADE_MEDIDA;
        }

        public virtual UnidadMedida GetById(string cdUnidadMedida)
        {
            return this._mapper.MapToObject(this._context.T_UNIDADE_MEDIDA
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_UNIDADE_MEDIDA == cdUnidadMedida));
        }

        public virtual List<UnidadMedida> GetByNombreOrCodePartial(string value)
        {
            return this._context.T_UNIDADE_MEDIDA
                .AsNoTracking()
                .Where(x => x.CD_UNIDADE_MEDIDA.ToLower().Contains(value.ToLower()) ||
                    x.DS_UNIDADE_MEDIDA.ToLower().Contains(value.ToLower()))
                .Select(x => this._mapper.MapToObject(x))
                .ToList();
        }

        public virtual List<UnidadMedida> GetUnidadesMedida()
        {
            var entities = this._context.T_UNIDADE_MEDIDA
                .AsNoTracking()
               .ToList();

            var unidades = new List<UnidadMedida>();

            foreach (var entity in entities)
            {
                unidades.Add(this._mapper.MapToObject(entity));
            }

            return unidades;
        }

        public virtual void RemoveUt(int? nuUt)
        {
            var ut = _context.T_UNIDAD_TRANSPORTE.FirstOrDefault(x => x.NU_UNIDAD_TRANSPORTE == nuUt);
            if (ut != null)
                _context.T_UNIDAD_TRANSPORTE.Remove(ut);
        }
        #endregion
    }
}
