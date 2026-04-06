using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Extensions;
using WIS.Domain.OrdenTarea;
using WIS.Domain.OrdenTarea.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class OrdenRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly OrdenMapper _mapper;
        protected readonly IDapper _dapper;

        public OrdenRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new OrdenMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyOrdenActiva(int nuOrden)
        {
            return this._context.T_ORT_ORDEN.Any(x => x.NU_ORT_ORDEN == nuOrden && x.ID_ESTADO == OrdenTareaDb.ESTADO_ORDEN_ACTIVA);
        }

        #endregion

        #region Get

        public virtual Orden GetOrden(int id)
        {
            return this._mapper.MapToObject(this._context.T_ORT_ORDEN
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_ORT_ORDEN == id));
        }

        public virtual string GetDescripcionOrden(int nuOrden)
        {
            return this._context.T_ORT_ORDEN
                .Where(x => x.NU_ORT_ORDEN == nuOrden)
                .Select(x => x.DS_ORT_ORDEN)
                .FirstOrDefault();
        }


        public virtual bool OrdenEsCerrable(int idOrden)
        {
            return !this._context.T_ORT_ORDEN_TAREA.Any(t => t.NU_ORT_ORDEN == idOrden && t.FL_RESUELTA == "N");
        }

        public virtual List<OrdenSesionEquipo> GetSesionEquipoAuxiliar(long sesionActivaFuncionario)
        {
            return this._context.T_ORT_ORDEN_SESION_EQUIPO.AsNoTracking().Where(x => x.NU_ORT_ORDEN_SESION == sesionActivaFuncionario &&
                x.DT_FIN == null).Select(a => this._mapper.MapToObject(a))
                .ToList();
        }

        public virtual List<OrdenSesion> GetSesionActivaFuncionario(int idOrden)
        {
            return this._context.T_ORT_ORDEN_SESION.AsNoTracking().Where(x => x.NU_ORT_ORDEN == idOrden &&
                x.DT_FIN == null).Select(a => this._mapper.MapToObject(a))
                .ToList();
        }

        public virtual List<Orden> GetOrdenesActivas()
        {
            return this._context.T_ORT_ORDEN.AsNoTracking()
                .Where(x => x.ID_ESTADO == OrdenTareaDb.ESTADO_ORDEN_ACTIVA)
                .Select(x => this._mapper.MapToObject(x))
                .ToList();
        }

        #endregion

        #region Add

        public virtual void AddOrden(Orden orden)
        {
            orden.Id = this._context.GetNextSequenceValueInt(_dapper, "S_ORT_ORDEN");
            T_ORT_ORDEN entity = this._mapper.MapToEntity(orden);
            this._context.T_ORT_ORDEN.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateOrden(Orden orden)
        {
            T_ORT_ORDEN entity = this._mapper.MapToEntity(orden);
            T_ORT_ORDEN attachedEntity = _context.T_ORT_ORDEN.Local
                .FirstOrDefault(w => w.NU_ORT_ORDEN == entity.NU_ORT_ORDEN);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_ORT_ORDEN.Attach(entity);
                _context.Entry<T_ORT_ORDEN>(entity).State = EntityState.Modified;
            }
        }      

        public virtual void UpdateEquipoSesion(OrdenSesionEquipo equipoSesion)
        {
            T_ORT_ORDEN_SESION_EQUIPO entity = this._mapper.MapToEntity(equipoSesion);
            T_ORT_ORDEN_SESION_EQUIPO attachedEntity = _context.T_ORT_ORDEN_SESION_EQUIPO.Local
                .FirstOrDefault(w => w.NU_ORT_ORDEN_SESION_EQUIPO == entity.NU_ORT_ORDEN_SESION_EQUIPO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_ORT_ORDEN_SESION_EQUIPO.Attach(entity);
                _context.Entry<T_ORT_ORDEN_SESION_EQUIPO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateSesionFuncionario(OrdenSesion sesionFuncionario)
        {
            T_ORT_ORDEN_SESION entity = this._mapper.MapToEntity(sesionFuncionario);
            T_ORT_ORDEN_SESION attachedEntity = _context.T_ORT_ORDEN_SESION.Local
                .FirstOrDefault(w => w.NU_ORT_ORDEN_SESION == entity.NU_ORT_ORDEN_SESION);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_ORT_ORDEN_SESION.Attach(entity);
                _context.Entry<T_ORT_ORDEN_SESION>(entity).State = EntityState.Modified;
            }
        }

        #endregion
               
        #region Remove

        #endregion
    }
}
