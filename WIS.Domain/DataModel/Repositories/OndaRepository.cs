using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Liberacion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class OndaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly OndaMapper _mapper;

        public OndaRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new OndaMapper();
        }

        #region Any

        public virtual bool AnyOnda(short id)
        {
            return this._context.T_ONDA.Any(d => d.CD_ONDA == id);
        }

        public virtual bool AnyOnda(string descripcion)
        {
            return this._context.T_ONDA.Any(d => d.DS_ONDA == descripcion);
        }

        #endregion

        #region Get

        public virtual Onda GetOnda(short idOnda)
        {
            T_ONDA entity = this._context.T_ONDA.AsNoTracking().FirstOrDefault(d => d.CD_ONDA == idOnda);

            return this._mapper.MapToObject(entity);
        }

        public virtual List<Onda> GetOndasActivas(string predio = null)
        {
            var entities = this._context.T_ONDA.AsNoTracking().Where(d => d.CD_SITUACAO == SituacionDb.Activo);

            if (!string.IsNullOrEmpty(predio) && predio != GeneralDb.PredioSinDefinir)
                entities = entities.Where(d => (d.NU_PREDIO == predio || d.NU_PREDIO == null));

            return entities.Select(o => _mapper.MapToObject(o)).ToList();
        }

        public virtual List<Onda> GetByDescripcionOrCodePartial(string value, string predio)
        {
            if (short.TryParse(value, out short cdOnda))
            {
                return this._context.T_ONDA.AsNoTracking()
                    .AsNoTracking()
                    .Where(d => (d.NU_PREDIO == predio || d.NU_PREDIO == null || d.NU_PREDIO == GeneralDb.PredioSinDefinir) && (d.CD_ONDA == cdOnda || d.DS_ONDA.ToLower().Contains(value.ToLower())))
                    .Select(d => this._mapper.MapToObject(d))
                    .ToList();
            }
            else
            {
                return this._context.T_ONDA.AsNoTracking()
                    .AsNoTracking()
                    .Where(d => (d.NU_PREDIO == predio || d.NU_PREDIO == null || d.NU_PREDIO == GeneralDb.PredioSinDefinir) && d.DS_ONDA.ToLower().Contains(value.ToLower()))
                    .Select(d => this._mapper.MapToObject(d))
                    .ToList();
            }
        }

        #endregion

        #region Add

        public virtual void AddOnda(Onda onda)
        {
            T_ONDA entity = this._mapper.MapToEntity(onda);

            entity.DT_CADASTRAMENTO = DateTime.Now;
            entity.DT_ALTERACAO = DateTime.Now;
            entity.DT_SITUACAO = DateTime.Now;

            this._context.T_ONDA.Add(entity);

        }

        #endregion

        #region Update

        public virtual void UpdateOnda(Onda onda)
        {
            T_ONDA entity = this._mapper.MapToEntity(onda);
            T_ONDA attachedEntity = _context.T_ONDA.Local
                .FirstOrDefault(x => x.CD_ONDA == entity.CD_ONDA);

            entity.DT_ALTERACAO = DateTime.Now;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_ONDA.Attach(entity);
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
