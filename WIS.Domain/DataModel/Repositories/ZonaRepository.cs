using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ZonaRepository
    {
        protected WISDB _context;
        protected string _application;
        protected int _userId;
        protected readonly ZonaMapper _mapper;

        public ZonaRepository(WISDB context, string application, int userId)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new ZonaMapper();
        }

        #region ANY

        public virtual bool AnyZona(string cdZona)
        {
            return _context.T_ZONA.Any(s => s.CD_ZONA == cdZona);
        }
        public virtual bool AnyPedidoAsociado(string cdZona)
        {
            return _context.T_PEDIDO_SAIDA.Any(x => x.CD_ZONA == cdZona);
        }

        #endregion

        #region GET

        public virtual Zona GetZona(string cdZona)
        {
            return _mapper.MapToObject(_context.T_ZONA.FirstOrDefault(s => s.CD_ZONA == cdZona));
        }

        public virtual List<Zona> GetZonas()
        {
            return _context.T_ZONA.ToList().Select(s => _mapper.MapToObject(s)).ToList();
        }

        #endregion

        #region ADD

        public virtual void AddZona(Zona zona)
        {
            _context.T_ZONA.Add(_mapper.MapToEntity(zona));
        }

        #endregion

        #region UPDATE

        public virtual void UpdateZona(Zona zona)
        {
            T_ZONA entity = this._mapper.MapToEntity(zona);
            T_ZONA attachedEntity = _context.T_ZONA.Local.FirstOrDefault(x => x.CD_ZONA == zona.CdZona);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_ZONA.Attach(entity);
                _context.Entry<T_ZONA>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region DELETE

        public virtual void DeleteZona(string cdZona)
        {
            var entity = this._context.T_ZONA
                .FirstOrDefault(x => x.CD_ZONA == cdZona);
            var attachedEntity = _context.T_ZONA.Local
                .FirstOrDefault(w => w.CD_ZONA == entity.CD_ZONA);

            if (attachedEntity != null)
                _context.T_ZONA.Remove(attachedEntity);
            else
                _context.T_ZONA.Remove(entity);
        }

        #endregion

    }
}
