using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Interfaces;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class InterfazRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly InterfazMapper _mapper;
        protected readonly EstanPedidoSalidaMapper _estanPedidoSalidaMapper;
        protected readonly IDapper _dapper;

        public InterfazRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new InterfazMapper();
            this._estanPedidoSalidaMapper = new EstanPedidoSalidaMapper();
            this._dapper = dapper;
        }

        #region Any

        #endregion

        #region Get

        public virtual List<InterfazError> GetInterfazError(long nuInterfazEjecucion)
        {
            return _context.T_INTERFAZ_EJECUCION_ERROR
                .AsNoTracking()
                .Where(x => x.NU_INTERFAZ_EJECUCION == nuInterfazEjecucion)
                .Select(x => _mapper.MapToObject(x))
                .ToList();
        }

        public virtual InterfazEjecucion GetInterfaz(long nuInterfazEjecucion)
        {
            var inter = this._context.T_INTERFAZ_EJECUCION
                .Include("T_INTERFAZ_EXTERNA")
                .Include("T_INTERFAZ_EXTERNA.T_INTERFAZ")
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == nuInterfazEjecucion);

            return this._mapper.MapToObject(inter);
        }

        public virtual List<EstanPedidoSalida> GetIntzPedidos(long nuInterfazEjecucion)
        {
            return _context.I_E_ESTAN_PEDIDO_SAIDA
                .AsNoTracking()
                .Where(i => i.NU_INTERFAZ_EJECUCION == nuInterfazEjecucion)
                .Select(i => _estanPedidoSalidaMapper.MapToObject(i))
                .ToList();
        }

        public virtual List<EstanPedidoSalidaDet> GetIntzPedidosDetalles(long nuInterfazEjecucion)
        {
            return _context.I_E_ESTAN_PEDIDO_SAIDA_DET
                .AsNoTracking()
                .Where(i => i.NU_INTERFAZ_EJECUCION == nuInterfazEjecucion)
                .Select(i=>_estanPedidoSalidaMapper.MapToObject(i))
                .ToList();
        }

        public virtual List<Interfaz> GetInterfaces()
        {
            return _context.T_INTERFAZ
                .AsNoTracking()
                .Select(i=> _mapper.MapToObject(i))
                .ToList();
        }

        public virtual List<InterfazExterna> GetInterfacesExternas()
        {
            return _context.T_INTERFAZ_EXTERNA
                .Include("T_INTERFAZ")
                .AsNoTracking()
                .Select(i => _mapper.MapToObject(i))
                .ToList();
        }

        public virtual List<InterfazExterna> GetInterfacesExternasByCodigoInterfaz(int cdInterfaz)
        {
            return _context.T_INTERFAZ_EXTERNA
                .Include("T_INTERFAZ")
                .AsNoTracking()
                .Where(i => i.CD_INTERFAZ == cdInterfaz)
                .Select(i => _mapper.MapToObject(i))
                .ToList();
        }

        public virtual InterfazData GetEjecucionData(long nuInterfazEjecucion)
        {
            return _mapper.MapToObject(_context.T_INTERFAZ_EJECUCION_DATA.AsNoTracking().FirstOrDefault(i => i.NU_INTERFAZ_EJECUCION == nuInterfazEjecucion));
        }

        public virtual InterfazExterna GetInterfazExterna(int cdInterfazExterna)
        {
            return _context.T_INTERFAZ_EXTERNA
                .Include("T_INTERFAZ")
                .AsNoTracking()
                .Where(i => i.CD_INTERFAZ_EXTERNA == cdInterfazExterna)
                .Select(i => _mapper.MapToObject(i))
                .FirstOrDefault();
        }

        #endregion

        #region Add

        public virtual void AddInterfazEjecucion(InterfazEjecucion interfaz)
        {
            interfaz.Id = this._context.GetNextSequenceValueLong(_dapper, "S_INTERFAZ_EJECUCION");
            var entity = this._mapper.MapToEntity(interfaz);
            this._context.T_INTERFAZ_EJECUCION.Add(entity);
        }

        public virtual void AddInterfazEjecucionError(InterfazError interfazError)
        {
            var entity = this._mapper.MapToEntity(interfazError);

            if (entity.DS_ERROR.Length > 1000)
                entity.DS_ERROR = entity.DS_ERROR.Substring(0, 1000);

            this._context.T_INTERFAZ_EJECUCION_ERROR.Add(entity);
        }

        #endregion

        #region Update
        
        public virtual void Update(InterfazEjecucion interfaz)
        {
            var entity = this._mapper.MapToEntity(interfaz);
            var attachedEntity = _context.T_INTERFAZ_EJECUCION.Local
                .FirstOrDefault(i => i.NU_INTERFAZ_EJECUCION == entity.NU_INTERFAZ_EJECUCION);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_INTERFAZ_EJECUCION.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateColPedido(EstanPedidoSalida obj)
        {
            var entity = this._estanPedidoSalidaMapper.MapToEntity(obj);
            var attachedEntity = this._context.I_E_ESTAN_PEDIDO_SAIDA.Local
                .FirstOrDefault(i => i.NU_INTERFAZ_EJECUCION == entity.NU_INTERFAZ_EJECUCION
                    && i.NU_REGISTRO == entity.NU_REGISTRO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.I_E_ESTAN_PEDIDO_SAIDA.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateColDetPedido(EstanPedidoSalidaDet obj)
        {
            var entity = this._estanPedidoSalidaMapper.MapToEntity(obj);
            var attachedEntity = this._context.I_E_ESTAN_PEDIDO_SAIDA_DET.Local
                .FirstOrDefault(i => i.NU_INTERFAZ_EJECUCION == entity.NU_INTERFAZ_EJECUCION
                    && i.NU_REGISTRO == entity.NU_REGISTRO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.I_E_ESTAN_PEDIDO_SAIDA_DET.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }
        
        #endregion

        #region Remove

        public virtual void RemoveInterfazError(InterfazError err)
        {
            var entity = this._mapper.MapToEntity(err);
            var attachedEntity = _context.T_INTERFAZ_EJECUCION_ERROR.Local
                .FirstOrDefault(x => x.NU_INTERFAZ_EJECUCION == entity.NU_INTERFAZ_EJECUCION
                    && x.NU_ERROR == entity.NU_ERROR);

            if (attachedEntity != null)
            {
                this._context.T_INTERFAZ_EJECUCION_ERROR.Remove(attachedEntity);
            }
            else
            {
                this._context.T_INTERFAZ_EJECUCION_ERROR.Attach(entity);
                this._context.T_INTERFAZ_EJECUCION_ERROR.Remove(entity);
            }
        }

        #endregion

    }
}
