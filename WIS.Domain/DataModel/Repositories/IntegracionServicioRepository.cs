using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Integracion;
using WIS.Domain.Extensions;
using WIS.Domain.Integracion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class IntegracionServicioRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly IntegracionServicioMapper _mapper;

        public IntegracionServicioRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._dapper = dapper;
            this._mapper = new IntegracionServicioMapper();
        }

        #region Any
        public virtual bool AnyIntegracionServicio(int value)
        {
            return _context.T_INTEGRACION_SERVICIO.Any(i => i.NU_INTEGRACION == value);
        }

        #endregion

        #region Get

        public virtual IntegracionServicio GetIntegrationByCodigo(string codigo)
        {
            var nose = _context.T_INTEGRACION_SERVICIO.AsNoTracking().FirstOrDefault(w => w.CD_INTEGRACION == codigo);

            return this._mapper.Map(nose);
        }
        
        public virtual IntegracionServicio GetIntegrationById(int id)
        {
            return _mapper.Map(_context.T_INTEGRACION_SERVICIO.FirstOrDefault(i => i.NU_INTEGRACION == id));
        }

        public virtual int GetNextNumeroIntegracionService()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_NU_INTEGRACION_SERVICIO);
        }

        public virtual List<IntegracionServicio> GetByDescriptionOrCodePartial(string searchValue)
        {
            if (int.TryParse(searchValue, out int id))
            {
                return _context.T_INTEGRACION_SERVICIO.Where(i => i.NU_INTEGRACION == id || i.DS_INTEGRACION.ToUpper().Contains(searchValue.ToUpper()))
                    .Select(i => _mapper.Map(i)).ToList();
            }
            else
            {
                return _context.T_INTEGRACION_SERVICIO.Where(i => i.DS_INTEGRACION.ToUpper().Contains(searchValue.ToUpper()))
                    .Select(i => _mapper.Map(i)).ToList();
            }
        }
        
        #endregion

        #region Add
       
        public virtual void Add(IntegracionServicio integracionServicio)
        {
            if (integracionServicio.Numero == 0)
                integracionServicio.Numero = this.GetNextNumeroIntegracionService();

            this._context.T_INTEGRACION_SERVICIO.Add(_mapper.MapToEntity(integracionServicio));
        }
       
        #endregion

        #region Update
       
        public virtual void Update(IntegracionServicio integracionServicio)
        {
            var entity = _mapper.MapToEntity(integracionServicio);
            var attachedEntity = _context.T_INTEGRACION_SERVICIO.Local.FirstOrDefault(x => x.NU_INTEGRACION == entity.NU_INTEGRACION);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_INTEGRACION_SERVICIO.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }
       
        #endregion

        #region Remove
       
        public virtual void Remove(IntegracionServicio integracionServicio)
        {
            var entity = this._mapper.MapToEntity(integracionServicio);
            var attachedEntity = _context.T_INTEGRACION_SERVICIO.Local.FirstOrDefault(w => w.NU_INTEGRACION == entity.NU_INTEGRACION);

            if (attachedEntity != null)
            {
                this._context.T_INTEGRACION_SERVICIO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_INTEGRACION_SERVICIO.Attach(entity);
                this._context.T_INTEGRACION_SERVICIO.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        #endregion
    }
}
