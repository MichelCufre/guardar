using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.OrdenTarea;
using WIS.Domain.OrdenTarea.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class TareaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly TareaMapper _mapper;
        protected readonly OrdenTareaFuncionarioMapper _mapperOTF;
        protected readonly OrdenTareaEquipoMapper _mapperOTE;
        protected readonly OrdenTareaDatoMapper _mapperOTD;
        protected readonly IDapper _dapper;

        public TareaRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new TareaMapper();
            this._mapperOTF = new OrdenTareaFuncionarioMapper();
            this._mapperOTE = new OrdenTareaEquipoMapper();
            this._mapperOTD = new OrdenTareaDatoMapper();
            this._dapper = dapper;
        }

        #region Tarea

        #region Add
        public virtual void AddTarea(Tarea tarea)
        {
            T_ORT_TAREA entity = this._mapper.MapToEntity(tarea);
            this._context.T_ORT_TAREA.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateTarea(Tarea tarea)
        {
            T_ORT_TAREA entity = this._mapper.MapToEntity(tarea);
            T_ORT_TAREA attachedEntity = _context.T_ORT_TAREA.Local
                .FirstOrDefault(w => w.CD_TAREA == entity.CD_TAREA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_ORT_TAREA.Attach(entity);
                _context.Entry<T_ORT_TAREA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateOrdenTarea(OrdenTareaObjeto tarea)
        {
            T_ORT_ORDEN_TAREA entity = this._mapper.MapToEntity(tarea);
            T_ORT_ORDEN_TAREA attachedEntity = _context.T_ORT_ORDEN_TAREA.Local
                .FirstOrDefault(w => w.NU_ORT_ORDEN == tarea.NuOrden
                    && w.NU_ORDEN_TAREA == tarea.NuTarea);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_ORT_ORDEN_TAREA.Attach(entity);
                _context.Entry<T_ORT_ORDEN_TAREA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateOrdenTareaFuncionario(OrdenTareaFuncionario ordenTareaFuncionario)
        {
            T_ORT_ORDEN_TAREA_FUNCIONARIO entity = this._mapperOTF.MapToEntity(ordenTareaFuncionario);
            T_ORT_ORDEN_TAREA_FUNCIONARIO attachedEntity = _context.T_ORT_ORDEN_TAREA_FUNCIONARIO.Local
                .FirstOrDefault(w => w.NU_ORT_ORDEN_TAREA_FUNC == ordenTareaFuncionario.NuOrtOrdenTareaFuncionario);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_ORT_ORDEN_TAREA_FUNCIONARIO.Attach(entity);
                _context.Entry<T_ORT_ORDEN_TAREA_FUNCIONARIO>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Any
        public virtual bool AnyTarea(string cdTarea)
        {
            return this._context.T_ORT_TAREA.Any(cb => cb.CD_TAREA == cdTarea);
        }

        #endregion

        #region Get

        public virtual Tarea GetTarea(string cdTarea)
        {
            return this._mapper.MapToObject(this._context.T_ORT_TAREA.AsNoTracking().FirstOrDefault(x => x.CD_TAREA == cdTarea));
        }

        public virtual List<Tarea> GetTareasManuales()
        {
            List<Tarea> tareas = new List<Tarea>();

            List<T_ORT_TAREA> entities = this._context.T_ORT_TAREA.Where(x => x.TP_TAREA == "MANUAL"
                && x.CD_SITUACAO == SituacionDb.Activo).AsNoTracking().ToList();

            foreach (var entity in entities)
            {
                tareas.Add(this._mapper.MapToObject(entity));
            }

            return tareas;
        }

        public virtual string GetTipoTarea(string cdTarea)
        {
            return this._context.T_ORT_TAREA
                        .Where(x => x.CD_TAREA == cdTarea)
                        .Select(x => x.TP_TAREA)
                        .FirstOrDefault();
        }

        public virtual List<Tarea> GetTareasManualesConRegistroHorasS()
        {
            List<Tarea> tareas = new List<Tarea>();

            List<T_ORT_TAREA> entities = this._context.T_ORT_TAREA.AsNoTracking()
                .Where(x => x.TP_TAREA == OrdenTareaDb.TIPO_TAREA_MANUAL
                    && x.CD_SITUACAO == SituacionDb.Activo
                    && x.FL_REGISTRO_HORAS == "S")
                .ToList();

            foreach (var entity in entities)
            {
                tareas.Add(this._mapper.MapToObject(entity));
            }

            return tareas;
        }

        public virtual string GetDescripcionTarea(string cdTarea)
        {
            return this._context.T_ORT_TAREA.FirstOrDefault(x => x.CD_TAREA == cdTarea).DS_TAREA;
        }

        #endregion

        #region Remove

        public virtual void DeleteTarea(string cdTarea)
        {
            var entity = this._context.T_ORT_TAREA
                .FirstOrDefault(x => x.CD_TAREA == cdTarea);
            var attachedEntity = _context.T_ORT_TAREA.Local
                .FirstOrDefault(w => w.CD_TAREA == entity.CD_TAREA);

            if (attachedEntity != null)
                _context.T_ORT_TAREA.Remove(attachedEntity);
            else
                _context.T_ORT_TAREA.Remove(entity);
        }

        #endregion

        #endregion

        #region OrdenTarea

        public virtual bool AnyOrdenTarea(string cdTarea)
        {
            return this._context.T_ORT_ORDEN_TAREA.Any(x => x.CD_TAREA == cdTarea);
        }

        public virtual void AddOrdenTarea(OrdenTareaObjeto nuevaOrdenTarea)
        {
            nuevaOrdenTarea.NuTarea = this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_NU_ORDEN_TAREA);
            T_ORT_ORDEN_TAREA entity = this._mapper.MapToEntity(nuevaOrdenTarea);
            this._context.T_ORT_ORDEN_TAREA.Add(entity);
        }

        public virtual bool AnyOrdenTarea(int nuOrden, string cdTarea, int cdEmpresa)
        {
            return this._context.T_ORT_ORDEN_TAREA
                .Any(x => x.NU_ORT_ORDEN == nuOrden
                    && x.CD_TAREA == cdTarea
                    && x.CD_EMPRESA == cdEmpresa);
        }

        public virtual OrdenTareaObjeto GetOrdenTarea(long numeroOrdenTarea)
        {
            return this._mapper.MapToObject(this._context.T_ORT_ORDEN_TAREA
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_ORDEN_TAREA == numeroOrdenTarea));
        }

        public virtual OrdenTareaObjeto GetOrdenTarea(int nuOrden, string cdTarea, int cdEmpresa)
        {
            return this._mapper.MapToObject(this._context.T_ORT_ORDEN_TAREA
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_ORT_ORDEN == nuOrden && x.CD_TAREA == cdTarea && x.CD_EMPRESA == cdEmpresa));
        }

        public virtual int GetCantOrdenTarea(int id)
        {
            return this._context.T_ORT_ORDEN_TAREA.Where(x => x.NU_ORT_ORDEN == id).Count();
        }

        public virtual bool IsOrdenTareaResuelta(long nuOrdenTarea)
        {
            return this._context.T_ORT_ORDEN_TAREA
                .Where(x => x.NU_ORDEN_TAREA == nuOrdenTarea)
                .Select(x => x.FL_RESUELTA)
                .FirstOrDefault() == "S";
        }

        public virtual List<OrdenTareaObjeto> GetTareasSinFinalizar(int idOrden)
        {
            return this._context.T_ORT_ORDEN_TAREA.AsNoTracking().Where(x => x.NU_ORT_ORDEN == idOrden &&
                x.FL_RESUELTA == "N").Select(a => this._mapper.MapToObject(a))
                .ToList();
        }

        public virtual OrdenTareaFuncionario GetOrdenTareaFuncionarioAmigable(long nuOrdenTarea)
        {
            return this._context.T_ORT_ORDEN_TAREA_FUNCIONARIO
                .Where(x => x.NU_ORDEN_TAREA == nuOrdenTarea && x.DT_HASTA == null)
                .Select(a => this._mapperOTF.MapToObject(a))
                .FirstOrDefault();
        }

        public virtual int GetCantOrdenTareasNoResueltas(int idOrden)
        {
            return this._context.T_ORT_ORDEN_TAREA.Where(x => x.NU_ORT_ORDEN == idOrden && x.FL_RESUELTA == "N").Count();
        }

        public virtual int GetCantidadTareasAmigablesPorCerrar(int idOrden)
        {
            return this._context.T_ORT_ORDEN_TAREA_FUNCIONARIO.Include("T_ORT_ORDEN_TAREA")
                .Where(x => x.DT_HASTA == null &&
                    x.T_ORT_ORDEN_TAREA.NU_ORT_ORDEN == idOrden &&
                    x.T_ORT_ORDEN_TAREA.FL_RESUELTA == "N").Count();
        }

        public virtual void DeleteOrdenTarea(int numeroTarea)
        {
            T_ORT_ORDEN_TAREA entity = this._context.T_ORT_ORDEN_TAREA
                .FirstOrDefault(x => x.NU_ORDEN_TAREA == numeroTarea);
            T_ORT_ORDEN_TAREA attachedEntity = _context.T_ORT_ORDEN_TAREA.Local
                .FirstOrDefault(x => x.NU_ORDEN_TAREA == numeroTarea);

            if (attachedEntity != null)
            {
                _context.T_ORT_ORDEN_TAREA.Remove(attachedEntity);
            }
            else
            {
                _context.T_ORT_ORDEN_TAREA.Attach(entity);
                _context.T_ORT_ORDEN_TAREA.Remove(entity);
            }
        }

        #endregion

        #region OrdenTareaFuncionario

        public virtual void AddOrdenTareaFuncionario(OrdenTareaFuncionario ordenTareaFuncionario)
        {
            T_ORT_ORDEN_TAREA_FUNCIONARIO entity = this._mapperOTF.MapToEntity(ordenTareaFuncionario);
            entity.NU_ORT_ORDEN_TAREA_FUNC = _context.GetNextSequenceValueLong(_dapper, Secuencias.S_ORT_ORDEN_TAREA_FUNC);
            this._context.T_ORT_ORDEN_TAREA_FUNCIONARIO.Add(entity);
        }

        public virtual OrdenTareaFuncionario GetOrdenTareaFuncionario(long numeroOrdenTareaFuncionario)
        {
            return this._mapperOTF.MapToObject(this._context.T_ORT_ORDEN_TAREA_FUNCIONARIO
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_ORT_ORDEN_TAREA_FUNC == numeroOrdenTareaFuncionario));
        }

        public virtual int GetCantOrdenTareaFuncionario(int id)
        {
            return this._context.T_ORT_ORDEN_TAREA_FUNCIONARIO.Include("T_ORT_ORDEN_TAREA").Where(x => x.T_ORT_ORDEN_TAREA.NU_ORT_ORDEN == id).Count();
        }

        public virtual OrdenTareaFuncionario GetOrdenTareaFuncionarioAmigableByUserId(int userId)
        {
            OrdenTareaFuncionario objeto = null;
            var entidad = this._context.T_ORT_ORDEN_TAREA_FUNCIONARIO
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_FUNCIONARIO == userId && x.DT_HASTA == null);

            if (entidad != null)
                objeto = this._mapperOTF.MapToObject(entidad);

            return objeto;
        }

        public virtual KeyValuePair<string, DateTime?> GetTareaFechaOrdenTareaFuncionario(int id)
        {
            T_ORT_ORDEN_TAREA_FUNCIONARIO entity = this._context.T_ORT_ORDEN_TAREA_FUNCIONARIO.Include("T_ORT_ORDEN_TAREA").Where(x => x.T_ORT_ORDEN_TAREA.NU_ORT_ORDEN == id).OrderByDescending(x => x.DT_HASTA).FirstOrDefault();

            return new KeyValuePair<string, DateTime?>(entity.T_ORT_ORDEN_TAREA.CD_TAREA, entity.DT_HASTA);
        }

        public virtual bool AnyOrdenTareaFuncionarioByOrdenTarea(long nuOrdenTarea)
        {
            return this._context.T_ORT_ORDEN_TAREA_FUNCIONARIO.Any(x => x.NU_ORDEN_TAREA == nuOrdenTarea);
        }

        public virtual bool AnyOrdenTareaFuncionario(long nuOrdenTareaFun)
        {
            return this._context.T_ORT_ORDEN_TAREA_FUNCIONARIO.Any(x => x.NU_ORT_ORDEN_TAREA_FUNC == nuOrdenTareaFun);
        }

        public virtual bool AnyTareaConInsumoManipuleo(string cdInsumoManipuleo)
        {
            return this._context.V_ORT_ORDEN_TAREA_DATO_WORT070.Any(x => x.CD_INSUMO_MANIPULEO == cdInsumoManipuleo);
        }

        public virtual bool AnyOrdenTareaFuncionarioSinFinalizar(long nuOrdenTarea)
        {
            return this._context.T_ORT_ORDEN_TAREA_FUNCIONARIO.Any(x => x.NU_ORDEN_TAREA == nuOrdenTarea && x.DT_HASTA == null);
        }

        public virtual bool AnySesionActivaFuncionarioAuxiliar(int userId, DateTime fecha)
        {
            return _context.T_ORT_ORDEN_SESION_EQUIPO
                .Join(_context.T_EQUIPO,
                    ose => new { ose.CD_EQUIPO },
                    e => new { e.CD_EQUIPO },
                    (ose, e) => new { OrdenSesionEquipo = ose, Equipo = e })
                .Join(_context.T_ORT_ORDEN_SESION,
                    osee => osee.OrdenSesionEquipo.NU_ORT_ORDEN_SESION,
                    os => os.NU_ORT_ORDEN_SESION,
                    (osee, os) => new { OrdenSesionEquipo = osee.OrdenSesionEquipo, Equipo = osee.Equipo, OrdenSesion = os })
                .Join(_context.USERS,
                    osee => new { CD_FUNCIONARIO = osee.Equipo.CD_FUNCIONARIO },
                    u => new { CD_FUNCIONARIO = (int?)u.USERID },
                    (osseos, u) => new { OrdenSesionEquipo = osseos.OrdenSesionEquipo, Equipo = osseos.Equipo, OrdenSesion = osseos.OrdenSesion, Usuario = u })
                .Any(x => x.OrdenSesion.DT_FIN == null
                    && x.OrdenSesion.DT_INICIO <= fecha
                    && x.Usuario.USERID == userId
                    && x.OrdenSesionEquipo.DT_FIN == null);
        }

        public virtual bool AnySesionActivaFuncionario(int userId, DateTime fecha)
        {
            return _context.T_ORT_ORDEN_SESION
                .Any(x => x.DT_FIN == null
                    && x.DT_INICIO <= fecha
                    && x.CD_FUNCIONARIO == userId);

        }

        public virtual bool AnySesionActivaFuncionarioAuxiliar(int userId, out string userSesionActva)
        {
            var funcionarioAuxSesionActiva = _context.T_ORT_ORDEN_SESION_EQUIPO
                .Join(_context.T_EQUIPO,
                    ose => new { ose.CD_EQUIPO },
                    e => new { e.CD_EQUIPO },
                    (ose, e) => new { OrdenSesionEquipo = ose, Equipo = e })
                .Join(_context.T_ORT_ORDEN_SESION,
                    osee => osee.OrdenSesionEquipo.NU_ORT_ORDEN_SESION,
                    os => os.NU_ORT_ORDEN_SESION,
                    (osee, os) => new { OrdenSesionEquipo = osee.OrdenSesionEquipo, Equipo = osee.Equipo, OrdenSesion = os })
                .Join(_context.USERS,
                    osee => new { CD_FUNCIONARIO = osee.OrdenSesion.CD_FUNCIONARIO },
                    u => new { CD_FUNCIONARIO =u.USERID },
                    (osseos, u) => new { OrdenSesionEquipo = osseos.OrdenSesionEquipo, Equipo = osseos.Equipo, OrdenSesion = osseos.OrdenSesion, UsuarioSesionActiva = u ,})
                .Join(_context.USERS,
                    osee => new { CD_FUNCIONARIO = osee.Equipo.CD_FUNCIONARIO },
                    u => new { CD_FUNCIONARIO = (int?)u.USERID },
                    (osseos, u) => new { OrdenSesionEquipo = osseos.OrdenSesionEquipo, Equipo = osseos.Equipo, OrdenSesion = osseos.OrdenSesion, Usuario = u , UsuarioSesionActiva = osseos.UsuarioSesionActiva })
                .FirstOrDefault(x => x.OrdenSesion.DT_FIN == null
                    && x.Usuario.USERID == userId
                    && x.OrdenSesionEquipo.DT_FIN == null);

            userSesionActva = funcionarioAuxSesionActiva == null ? string.Empty : funcionarioAuxSesionActiva.UsuarioSesionActiva.LOGINNAME;

            return funcionarioAuxSesionActiva != null;
        }

        public virtual bool AnySesionActivaFuncionario(int userId)
        {
            return _context.T_ORT_ORDEN_SESION
                .Any(x => x.CD_FUNCIONARIO == userId
                    && x.DT_FIN == null);

        }

        public virtual bool AnySolapamientoRegistroTareaFuncionario(int userId, DateTime? fecha, long nuOrdenTareaFuncionario)
        {
            return _context.T_ORT_ORDEN_TAREA_FUNCIONARIO
                 .Any(x => x.NU_ORT_ORDEN_TAREA_FUNC != nuOrdenTareaFuncionario
                    && x.CD_FUNCIONARIO == userId
                    && x.DT_DESDE <= fecha
                    && x.DT_HASTA >= fecha);
        }

        public virtual bool AnySolapamientoRegistroTareaAmigableFuncionario(int userId, DateTime? fecha, long nuOrdenTareaFuncAmigable)
        {
            return _context.T_ORT_ORDEN_TAREA_FUNCIONARIO
                  .Any(x => x.NU_ORT_ORDEN_TAREA_FUNC != nuOrdenTareaFuncAmigable
                     && x.CD_FUNCIONARIO == userId
                     && x.DT_DESDE <= fecha
                     && x.DT_HASTA == null);
        }

        public virtual void DeleteOrdenTareaFuncionario(OrdenTareaFuncionario detalle)
        {
            T_ORT_ORDEN_TAREA_FUNCIONARIO entity = this._mapperOTF.MapToEntity(detalle);
            T_ORT_ORDEN_TAREA_FUNCIONARIO attachedEntity = _context.T_ORT_ORDEN_TAREA_FUNCIONARIO.Local
                .FirstOrDefault(w => w.NU_ORT_ORDEN_TAREA_FUNC == entity.NU_ORT_ORDEN_TAREA_FUNC);

            if (attachedEntity != null)
            {
                this._context.T_ORT_ORDEN_TAREA_FUNCIONARIO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_ORT_ORDEN_TAREA_FUNCIONARIO.Attach(entity);
                this._context.T_ORT_ORDEN_TAREA_FUNCIONARIO.Remove(entity);
            }
        }

        #endregion

        #region OrdenTareaEquipo

        public virtual void AddOrdenTareaEquipo(OrdenTareaEquipo ordenTareaEquipo)
        {
            T_ORT_ORDEN_TAREA_EQUIPO entity = this._mapperOTE.MapToEntity(ordenTareaEquipo);
            entity.NU_ORT_ORDEN_TAREA_EQUIPO = _context.GetNextSequenceValueLong(_dapper, "S_ORT_ORDEN_TAREA_EQUIPO");
            this._context.T_ORT_ORDEN_TAREA_EQUIPO.Add(entity);
        }

        public virtual void UpdateOrdenTareaEquipo(OrdenTareaEquipo ordenTareaEquipo)
        {
            T_ORT_ORDEN_TAREA_EQUIPO entity = this._mapperOTE.MapToEntity(ordenTareaEquipo);
            T_ORT_ORDEN_TAREA_EQUIPO attachedEntity = _context.T_ORT_ORDEN_TAREA_EQUIPO.Local
                .FirstOrDefault(w => w.NU_ORT_ORDEN_TAREA_EQUIPO == ordenTareaEquipo.NuOrtOrdenTareaEquipo);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_ORT_ORDEN_TAREA_EQUIPO.Attach(entity);
                _context.Entry<T_ORT_ORDEN_TAREA_EQUIPO>(entity).State = EntityState.Modified;
            }
        }

        public virtual OrdenTareaEquipo GetOrdenTareaEquipo(long numeroOrdenTareaEquipo)
        {
            return this._mapperOTE.MapToObject(this._context.T_ORT_ORDEN_TAREA_EQUIPO
               .AsNoTracking()
               .FirstOrDefault(x => x.NU_ORT_ORDEN_TAREA_EQUIPO == numeroOrdenTareaEquipo));
        }

        public virtual bool AnyOrdenTareaEquipoByOrdenTarea(long nuOrdenTarea)
        {
            return this._context.T_ORT_ORDEN_TAREA_EQUIPO.Any(x => x.NU_ORDEN_TAREA == nuOrdenTarea);
        }

        public virtual bool AnyOrdenTareaEquipo(long nuOrdenTareaEquipo)
        {
            return this._context.T_ORT_ORDEN_TAREA_EQUIPO.Any(x => x.NU_ORT_ORDEN_TAREA_EQUIPO == nuOrdenTareaEquipo);
        }

        public virtual bool AnySesionActivaEquipo(int cdEquipo, DateTime fecha)
        {
            return _context.T_ORT_ORDEN_SESION_EQUIPO
                .Join(_context.T_EQUIPO,
                    ose => new { ose.CD_EQUIPO },
                    e => new { e.CD_EQUIPO },
                    (ose, e) => new { OrdenSesionEquipo = ose, Equipo = e })
                .Join(_context.T_ORT_ORDEN_SESION,
                    osee => osee.OrdenSesionEquipo.NU_ORT_ORDEN_SESION,
                    os => os.NU_ORT_ORDEN_SESION,
                    (osee, os) => new { OrdenSesionEquipo = osee.OrdenSesionEquipo, Equipo = osee.Equipo, OrdenSesion = os })
                .Any(x => x.OrdenSesion.DT_FIN == null
                    && x.OrdenSesion.DT_INICIO <= fecha
                    && x.Equipo.CD_FUNCIONARIO == null
                    && x.OrdenSesionEquipo.CD_EQUIPO == cdEquipo
                    && x.OrdenSesionEquipo.DT_FIN == null);

        }

        public virtual bool AnySolapamientoRegistroTareaEquipo(int cdEquipo, DateTime? fecha, long nuOrdenTareaEquipo)
        {
            return _context.T_ORT_ORDEN_TAREA_EQUIPO
                .Any(x => x.NU_ORT_ORDEN_TAREA_EQUIPO != nuOrdenTareaEquipo
                    && x.CD_EQUIPO == cdEquipo
                    && x.DT_DESDE <= fecha
                    && x.DT_HASTA >= fecha);
        }

        public virtual void DeleteOrdenTareaEquipo(OrdenTareaEquipo detalle)
        {
            T_ORT_ORDEN_TAREA_EQUIPO entity = this._mapperOTE.MapToEntity(detalle);
            T_ORT_ORDEN_TAREA_EQUIPO attachedEntity = _context.T_ORT_ORDEN_TAREA_EQUIPO.Local
                .FirstOrDefault(w => w.NU_ORT_ORDEN_TAREA_EQUIPO == entity.NU_ORT_ORDEN_TAREA_EQUIPO);

            if (attachedEntity != null)
            {
                this._context.T_ORT_ORDEN_TAREA_EQUIPO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_ORT_ORDEN_TAREA_EQUIPO.Attach(entity);
                this._context.T_ORT_ORDEN_TAREA_EQUIPO.Remove(entity);
            }
        }

        #endregion

        #region OrdenTareaDato
        public virtual void AddOrdenManipuleoInsumo(OrdenTareaManipuleoInsumo ordenManipuleo)
        {
            T_ORT_ORDEN_TAREA_DATO entity = this._mapperOTD.MapToEntity(ordenManipuleo);
            entity.NU_ORT_ORDEN_TAREA_DATO = _context.GetNextSequenceValueLong(_dapper, "S_ORT_ORDEN_TAREA_DATO");

            this._context.T_ORT_ORDEN_TAREA_DATO.Add(entity);
        }

        public virtual bool AnyOrdenTareaDato(long numeroOrdenTareaDato)
        {
            return this._context.T_ORT_ORDEN_TAREA_DATO
                .Any(x => x.NU_ORT_ORDEN_TAREA_DATO == numeroOrdenTareaDato);
        }

        public virtual bool AnyOrdenTareaDatoByOrdenTarea(long nuOrdenTarea)
        {
            return this._context.T_ORT_ORDEN_TAREA_DATO.Any(x => x.NU_ORDEN_TAREA == nuOrdenTarea);
        }

        public virtual OrdenTareaManipuleoInsumo GetManipuleoInsumo(long numeroOrdenTareaDato)
        {
            return this._mapperOTD.MapToObject(this._context.T_ORT_ORDEN_TAREA_DATO
                .AsNoTracking()
                .FirstOrDefault(x => x.NU_ORT_ORDEN_TAREA_DATO == numeroOrdenTareaDato));
        }

        public virtual void UpdateOrdenTareaManipuleoInsumo(OrdenTareaManipuleoInsumo ordenManipuleoInsumo)
        {
            T_ORT_ORDEN_TAREA_DATO entity = this._mapperOTD.MapToEntity(ordenManipuleoInsumo);
            T_ORT_ORDEN_TAREA_DATO attachedEntity = _context.T_ORT_ORDEN_TAREA_DATO.Local
                .FirstOrDefault(w => w.NU_ORT_ORDEN_TAREA_DATO == ordenManipuleoInsumo.NumeroOrdenTareaDato);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_ORT_ORDEN_TAREA_DATO.Attach(entity);
                _context.Entry<T_ORT_ORDEN_TAREA_DATO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void DeleteOrdenTareaDato(OrdenTareaManipuleoInsumo detalle)
        {
            T_ORT_ORDEN_TAREA_DATO entity = this._mapperOTD.MapToEntity(detalle);
            T_ORT_ORDEN_TAREA_DATO attachedEntity = _context.T_ORT_ORDEN_TAREA_DATO.Local
                .FirstOrDefault(w => w.NU_ORT_ORDEN_TAREA_DATO == entity.NU_ORT_ORDEN_TAREA_DATO);

            if (attachedEntity != null)
            {
                this._context.T_ORT_ORDEN_TAREA_DATO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_ORT_ORDEN_TAREA_DATO.Attach(entity);
                this._context.T_ORT_ORDEN_TAREA_DATO.Remove(entity);
            }
        }

        #endregion
    }
}
