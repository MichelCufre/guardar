using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Eventos;
using WIS.Domain.Extensions;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class NotificacionRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly NotificacionMapper _mapper;
        protected readonly IDapper _dapper;

        public NotificacionRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new NotificacionMapper();
            this._dapper = dapper;
        }

        #region Any

        #endregion

        #region Get

        public virtual long GetNextNuNotificacion()
        {
            return this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_NOTIFICACIONES);
        }

        public virtual int GetNextNuNotificacionArchivo()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_NU_EVENTO_NOTI_ARCHIVO);
        }

        public virtual List<NotificacionEmail> GetNotificacionesEmailPendientes()
        {
            var notificaciones = new List<NotificacionEmail>();
            var entities = _context.T_EVENTO_NOTIFICACION_EMAIL
                .Where(s => s.ND_ESTADO == EstadoNotificacion.EST_PEND.ToString())
                .Include("T_EVENTO_NOTIFICACION")
                .Include("T_EVENTO_NOTIFICACION.T_EVENTO_INSTANCIA")
                .Include("T_EVENTO_NOTIFICACION.T_EVENTO_INSTANCIA.T_EVENTO")
                .Include("T_EVENTO_NOTIFICACION.T_EVENTO_NOTIFICACION_ARCHIVO").AsNoTracking().ToList();

            foreach (var entity in entities)
                notificaciones.Add(_mapper.MapNotificacionEmailToObject(entity));

            return notificaciones;
        }

        public virtual NotificacionEmail GetNotificacionEmail(long nuNotificacion)
        {
            return _context.T_EVENTO_NOTIFICACION_EMAIL
                .AsNoTracking()
                .Where(x => x.NU_EVENTO_NOTIFICACION == nuNotificacion)
                .Select(x => _mapper.MapNotificacionEmailToObject(x, false))
                .FirstOrDefault();
        }

        public virtual NotificacionArchivo GetNotificacionArchivoData(int nuNotificacionArchivo, long nuNotificacion)
        {
            return _context.T_EVENTO_NOTIFICACION_ARCHIVO
                .AsNoTracking()
                .Where(x => x.NU_EVENTO_NOTIFICACION_ARCHIVO == nuNotificacionArchivo
                    && x.NU_EVENTO_NOTIFICACION == nuNotificacion)
                .Select(x => new NotificacionArchivo
                {
                    IdReferencia = x.ID_REFERENCIA,
                    TpReferencia = x.TP_REFERENCIA,
                    DsArchivo = x.DS_ARCHIVO,
                    VlData = x.VL_DATA
                })
                .FirstOrDefault();
        }

        #endregion

        #region Add

        public virtual void AddNotificacionEmail(NotificacionEmail obj)
        {
            obj.Id = GetNextNuNotificacion();

            T_EVENTO_NOTIFICACION_EMAIL entity = this._mapper.MapNotificacionEmailToEntity(obj);

            AddNotificacion(obj);

            foreach (var a in obj.Archivos)
            {
                a.NuEventoNotificacion = obj.Id;
                AddNotificacionArchivo(a);
            }

            this._context.T_EVENTO_NOTIFICACION_EMAIL.Add(entity);
        }

        public virtual void AddNotificacionArchivo(NotificacionArchivo obj)
        {
            obj.Id = GetNextNuNotificacionArchivo();

            if (obj.TpReferencia == EventoArchivoTipoReferenciaDb.NOTIFICACION)
            {
                obj.IdReferencia = $"{obj.Id}.{obj.NuEventoNotificacion}";
            }

            T_EVENTO_NOTIFICACION_ARCHIVO entity = this._mapper.MapNotificacionArchivoToEntity(obj);

            this._context.T_EVENTO_NOTIFICACION_ARCHIVO.Add(entity);
        }

        public virtual void AddNotificacion(Notificacion obj)
        {
            T_EVENTO_NOTIFICACION entity = this._mapper.MapNotificacionToEntity(obj);

            this._context.T_EVENTO_NOTIFICACION.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateNotificacionEmail(NotificacionEmail obj)
        {
            T_EVENTO_NOTIFICACION_EMAIL entity = this._mapper.MapNotificacionEmailToEntity(obj);
            T_EVENTO_NOTIFICACION_EMAIL attachedEntity = _context.T_EVENTO_NOTIFICACION_EMAIL.Local
                .FirstOrDefault(w => w.NU_EVENTO_NOTIFICACION == entity.NU_EVENTO_NOTIFICACION);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_EVENTO_NOTIFICACION_EMAIL.Attach(entity);
                _context.Entry<T_EVENTO_NOTIFICACION_EMAIL>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        #endregion

        #region Dapper

        #endregion
    }
}
