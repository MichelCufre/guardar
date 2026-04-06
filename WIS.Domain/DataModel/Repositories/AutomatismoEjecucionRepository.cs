using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.Automatismo;
using WIS.Domain.DataModel.Mappers.Automatismo;
using WIS.Domain.Extensions;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
	public class AutomatismoEjecucionRepository
	{
		protected readonly WISDB _context;
		protected readonly IDapper _dapper;
		protected readonly int _userId;
		protected readonly string _cdAplicacion;
		protected readonly AutomatismoEjecucionMapper _mapper;
		protected readonly AutomatismoDataRepository _dataRepository;

		public AutomatismoEjecucionRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
		{
			_context = context;
			_dapper = dapper;
			_userId = userId;
			_cdAplicacion = cdAplicacion;
			_mapper = new AutomatismoEjecucionMapper();
			_dataRepository = new AutomatismoDataRepository(context, cdAplicacion, userId, dapper);

		}

		#region Any
		public virtual bool EjecucionTieneDatos(int id)
		{
			return _context.T_AUTOMATISMO_DATA.Any(i => i.NU_AUTOMATISMO_EJECUCION == id);
		}
		#endregion

		#region Get
		public virtual AutomatismoEjecucion GetAutomatismoEjecucionById(int id)
		{
			return this._mapper.Map(this._context.T_AUTOMATISMO_EJECUCION.AsNoTracking().FirstOrDefault(f => f.NU_AUTOMATISMO_EJECUCION == id));
		}

		public virtual AutomatismoEjecucion GetAutomatismoEjecucionWithData(int id)
		{
			var entity = _context.T_AUTOMATISMO_EJECUCION.AsNoTracking().Include("T_AUTOMATISMO_DATA").AsNoTracking().FirstOrDefault(i => i.NU_AUTOMATISMO_EJECUCION == id);
			return _mapper.Map(entity, true);
		}

        public virtual List<AtomatismoConfirmacionEntrada> GetAutomatismoConfirmacionAutomatismo(string idEntrada)
		{
			var list = _context.T_AUTOMATISMO_CONF_ENTRADA.AsNoTracking().Where(x => x.NU_INTERFAZ_EJECUCION_ENT == idEntrada).ToList();
			return _mapper.Map(list);

		}

		public virtual int GetNextNuAutomatismoEjecucion()
		{
			return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_NU_AUTOMATISMO_EJECUCION);
		}
		#endregion

		#region Add

		public virtual void Add(AutomatismoEjecucion obj)
		{
			var entity = this._mapper.Map(obj);
			this._context.T_AUTOMATISMO_EJECUCION.Add(entity);
		}

		#endregion

		#region Update

		public virtual void Update(AutomatismoEjecucion obj)
		{
			var entity = this._mapper.Map(obj);
			var attachedEntity = this._context.T_AUTOMATISMO_EJECUCION.Local.FirstOrDefault(d => d.NU_AUTOMATISMO_EJECUCION == entity.NU_AUTOMATISMO_EJECUCION);

			if (attachedEntity != null)
			{
				var attachedEntry = _context.Entry(attachedEntity);
				attachedEntry.CurrentValues.SetValues(entity);
				attachedEntry.State = EntityState.Modified;
			}
			else
			{
				this._context.T_AUTOMATISMO_EJECUCION.Attach(entity);
				this._context.Entry(entity).State = EntityState.Modified;
			}

			var newData = obj.AutomatismoData.Where(w => w.Id == 0).ToList();

			foreach (var data in newData)
			{
				this._dataRepository.Add(data);
			}

		}

        public virtual void AddConfAutomatismoEntrada(long ejecucion, string loginName, TransferenciaStockRequest request, bool confEntrada = false)
        {
            List<T_AUTOMATISMO_CONF_ENTRADA> entity = _mapper.Map(ejecucion, loginName, request, confEntrada);
            this._context.T_AUTOMATISMO_CONF_ENTRADA.AddRange(entity);
        }

        public virtual void RemoveConfirmacionAutomatismoEntrada(string idEntrada)
		{
			var confirmaciones = this._context.T_AUTOMATISMO_CONF_ENTRADA.Where(x => x.NU_INTERFAZ_EJECUCION_ENT == idEntrada);
			this._context.T_AUTOMATISMO_CONF_ENTRADA.RemoveRange(confirmaciones);
		}

		#endregion

		#region Remove

		#endregion

		#region Dapper

		#endregion
	}
}
