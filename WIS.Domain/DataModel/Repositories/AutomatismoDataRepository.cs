using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel.Mappers.Automatismo;
using WIS.Domain.Extensions;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class AutomatismoDataRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly AutomatismoDataMapper _mapper;

        public AutomatismoDataRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._dapper = dapper;
            _mapper = new AutomatismoDataMapper();
        }

        #region Any

        #endregion

        #region Get
        public virtual AutomatismoData GetAutomatismoDataById(int id)
        {
            return this._mapper.Map(this._context.T_AUTOMATISMO_DATA.FirstOrDefault(f => f.NU_AUTOMATISMO_DATA == id));
        }

        public virtual int GetNextNuAutomatismoData()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_NU_AUTOMATISMO_DATA);
        }
        #endregion

        #region Add
        public virtual void Add(AutomatismoData obj)
        {
            obj.FechaRegistro = DateTime.Now;
            obj.Id = this.GetNextNuAutomatismoData();

            var entity = this._mapper.MapToEntity(obj);
            this._context.T_AUTOMATISMO_DATA.Add(entity);
        }
        #endregion

        #region Update
        public virtual void Update(AutomatismoData obj)
        {
            var entity = _mapper.MapToEntity(obj);
            var attachedEntity = _context.T_AUTOMATISMO_DATA.Local.FirstOrDefault(x => x.NU_AUTOMATISMO_DATA == entity.NU_AUTOMATISMO_DATA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_AUTOMATISMO_DATA.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }
        #endregion

        #region Remove


        #endregion

        #region Dapper

        #endregion
    }
}
