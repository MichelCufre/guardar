using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel.Mappers.Automatismo;
using WIS.Domain.Extensions;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class AutomatismoInterfazRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly AutomatismoInterfazMapper _mapper;

        public AutomatismoInterfazRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._dapper = dapper;
            this._mapper = new AutomatismoInterfazMapper();
        }

        #region Any
        public virtual bool AnyServicioIntegracion(int id)
        {
            return _context.T_AUTOMATISMO_INTERFAZ.Any(i => i.NU_INTEGRACION == id);
        }

        public virtual bool AutomatismoHasAnyInterfaz(int nuAutomatismo)
        {
            return _context.T_AUTOMATISMO_INTERFAZ.Any(i => i.NU_AUTOMATISMO == nuAutomatismo);
        }
        #endregion

        #region Get
        public virtual IQueryable<T_AUTOMATISMO_INTERFAZ> GetQueryWithAllIncludes()
        {
            return this._context.T_AUTOMATISMO_INTERFAZ.Include("T_INTEGRACION_SERVICIO").AsNoTracking();
        }
        public virtual AutomatismoInterfaz GetAutomatismoInterfazById(int id)
        {
            return this._mapper.Map(this._context.T_AUTOMATISMO_INTERFAZ.FirstOrDefault(f => f.NU_AUTOMATISMO_INTERFAZ == id));
        }
        public virtual AutomatismoInterfaz GetByAutomatismoAndCodigoExterno(int? automatismo, int codigoExterno)
        {
            return this._mapper.Map(this._context.T_AUTOMATISMO_INTERFAZ.FirstOrDefault(f => f.NU_AUTOMATISMO == automatismo && f.CD_INTERFAZ_EXTERNA == codigoExterno));
        }
        public virtual List<AutomatismoInterfaz> GetAutomatismosInterfaz()
        {
            return this._mapper.Map(this.GetQueryWithAllIncludes().ToList());
        }
        public virtual List<int> GetCodigoInterfazByTipoAutomatismo(string tipo)
        {
            return _context.T_INTERFAZ.Where(i => i.ND_TIPO_AUTOMATISMO == tipo)?.Select(s => s.CD_INTERFAZ)?.ToList() ?? new List<int>();
        }

        public virtual int GetNextNuAutomatismoInterfaz()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_NU_AUTOMATISMO_INTERFAZ);
        }
        #endregion

        #region Add
        public virtual void Add(AutomatismoInterfaz obj)
        {
            T_AUTOMATISMO_INTERFAZ interfaz = this._mapper.Map(obj);
            this._context.T_AUTOMATISMO_INTERFAZ.Add(interfaz);
        }

        #endregion

        #region Update
        public virtual void Update(AutomatismoInterfaz interfaz)
        {
            var entity = _mapper.Map(interfaz);
            var attachedEntity = _context.T_AUTOMATISMO_INTERFAZ.Local.FirstOrDefault(x => x.NU_AUTOMATISMO_INTERFAZ == entity.NU_AUTOMATISMO_INTERFAZ);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_AUTOMATISMO_INTERFAZ.Attach(entity);
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
