using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel.Mappers.Automatismo;
using WIS.Domain.Extensions;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class AutomatismoPuestoRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly AutomatismoPuestoMapper _mapper;

        public AutomatismoPuestoRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._dapper = dapper;
            this._mapper = new AutomatismoPuestoMapper();
        }

        #region Any
        public virtual bool AnyIdPuesto(string idPuesto)
        {
            return _context.T_AUTOMATISMO_PUESTO.Any(i => i.ID_PUESTO == idPuesto);
        }
        public virtual bool AnyIdPuesto(string idPuesto, int automatismoPuesto)
        {
            return _context.T_AUTOMATISMO_PUESTO.Any(i => i.ID_PUESTO == idPuesto && i.NU_AUTOMATISMO_PUESTO != automatismoPuesto);
        }
        #endregion

        #region Get
        public virtual AutomatismoPuesto GetAutomatismoPuestoById(int id)
        {
            return this._mapper.Map(this._context.T_AUTOMATISMO_PUESTO.FirstOrDefault(f => f.NU_AUTOMATISMO_PUESTO == id));
        }

        public virtual List<AutomatismoPuesto> GetAutomatismosPuesto()
        {
            return this._context.T_AUTOMATISMO_PUESTO.Include("T_AUTOMATISMO").Select(s => this._mapper.Map(s)).ToList();
        }

        public virtual int GetNextNumeroAutomatismoPuesto()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_NU_AUTOMATISMO_PUESTO);
        }

        #endregion

        #region Add
        public virtual void Add(AutomatismoPuesto puesto)
        {
            if (puesto.Id == 0)
                puesto.Id = this.GetNextNumeroAutomatismoPuesto();

            puesto.FechaRegistro = DateTime.Now;

            _context.T_AUTOMATISMO_PUESTO.Add(_mapper.MapToEntity(puesto));
        }
        #endregion

        #region Update
        public virtual void Update(AutomatismoPuesto puesto)
        {
            puesto.FechaModificacion = DateTime.Now;

            var entity = _mapper.MapToEntity(puesto);
            var attachedEntity = _context.T_AUTOMATISMO_PUESTO.Local.FirstOrDefault(x => x.NU_AUTOMATISMO_PUESTO == entity.NU_AUTOMATISMO_PUESTO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_AUTOMATISMO_PUESTO.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }
        #endregion

        #region Remove
        public virtual void Remove(AutomatismoPuesto puesto)
        {
            var entity = this._mapper.MapToEntity(puesto);
            var attachedEntity = _context.T_AUTOMATISMO_PUESTO.Local.FirstOrDefault(w => w.NU_AUTOMATISMO_PUESTO == entity.NU_AUTOMATISMO_PUESTO);

            if (attachedEntity != null)
            {
                this._context.T_AUTOMATISMO_PUESTO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_AUTOMATISMO_PUESTO.Attach(entity);
                this._context.T_AUTOMATISMO_PUESTO.Remove(entity);
            }
        }
        #endregion

        #region Dapper

        #endregion
    }
}
