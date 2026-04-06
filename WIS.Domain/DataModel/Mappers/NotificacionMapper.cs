using System.Text;
using WIS.Domain.Eventos;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class NotificacionMapper : Mapper
    {
        public NotificacionMapper()
        {

        }

        public virtual T_EVENTO_NOTIFICACION MapNotificacionToEntity(Notificacion obj, bool addNavegables = true)
        {
            var entity = new T_EVENTO_NOTIFICACION
            {
                NU_EVENTO_NOTIFICACION = obj.Id,
                NU_EVENTO_INSTANCIA = obj.NumeroInstancia,
            };
            return entity;
        }

        public virtual NotificacionEmail MapNotificacionEmailToObject(T_EVENTO_NOTIFICACION_EMAIL entity, bool addNavegables = true)
        {

            var obj = new NotificacionEmail
            {
                Id = entity.NU_EVENTO_NOTIFICACION,
                NumeroInstancia = entity.T_EVENTO_NOTIFICACION?.NU_EVENTO_INSTANCIA ?? -1,
                Asunto = entity.DS_SUBJECT,
                Cuerpo = Encoding.UTF8.GetString(entity.DS_CUERPO),
                EmailEnvia = entity.DS_EMAIL_FROM,
                EmailRecibe = entity.DS_EMAIL_TO,
                Estado = Notificacion.GetEstado(entity.ND_ESTADO),
                FechaEnvio = entity.DT_ENVIO,
                FechaRenvio = entity.DT_RENVIO,
                FechaCreacion = entity.DT_ADDROW,
                TipoNotificacion = TipoNotificacion.EMAIL
            };

            if (!string.IsNullOrEmpty(entity.FL_HTML))
                obj.IsHtml = entity.FL_HTML.ToUpper() == "S";

            if (addNavegables)
            {
                EventoMapper ev = new EventoMapper();
                obj.Instancia = ev.MapInstanciaToObject(entity.T_EVENTO_NOTIFICACION.T_EVENTO_INSTANCIA);
                obj.Instancia.Evento = ev.MapEventoToObject(entity.T_EVENTO_NOTIFICACION.T_EVENTO_INSTANCIA.T_EVENTO);
                foreach (var a in entity.T_EVENTO_NOTIFICACION.T_EVENTO_NOTIFICACION_ARCHIVO)
                    obj.Archivos.Add(MapNotificacionArchivoToObject(a));
            }

            return obj;
        }

        public virtual NotificacionArchivo MapNotificacionArchivoToObject(T_EVENTO_NOTIFICACION_ARCHIVO entity, bool addNavegables = true)
        {
            var obj = new NotificacionArchivo
            {
                DsArchivo = entity.DS_ARCHIVO,
                IdReferencia = entity.ID_REFERENCIA,
                TpReferencia = entity.TP_REFERENCIA,
                VlData = entity.VL_DATA,
                DtAddRow = entity.DT_ADDROW,
                DtUpdateRow = entity.DT_UPDROW,
                Id = entity.NU_EVENTO_NOTIFICACION_ARCHIVO,
                NuEventoNotificacion = entity.NU_EVENTO_NOTIFICACION,
            };
            return obj;
        }

        public virtual T_EVENTO_NOTIFICACION_ARCHIVO MapNotificacionArchivoToEntity(NotificacionArchivo obj, bool addNavegables = true)
        {
            var entity = new T_EVENTO_NOTIFICACION_ARCHIVO
            {
                DS_ARCHIVO = obj.DsArchivo,
                ID_REFERENCIA = obj.IdReferencia,
                TP_REFERENCIA = obj.TpReferencia,
                VL_DATA = obj.VlData,
                DT_ADDROW = obj.DtAddRow,
                DT_UPDROW = obj.DtUpdateRow,
                NU_EVENTO_NOTIFICACION_ARCHIVO = obj.Id,
                NU_EVENTO_NOTIFICACION = obj.NuEventoNotificacion,
            };
            return entity;
        }

        public virtual T_EVENTO_NOTIFICACION_EMAIL MapNotificacionEmailToEntity(NotificacionEmail obj, bool addNavegables = true)
        {
            var entity = new T_EVENTO_NOTIFICACION_EMAIL
            {
                NU_EVENTO_NOTIFICACION = obj.Id,
                DS_SUBJECT = obj.Asunto,
                ND_ESTADO = obj.Estado.ToString(),
                DS_CUERPO = Encoding.UTF8.GetBytes(obj.Cuerpo), //Ver si es el encoding correcto
                DS_EMAIL_FROM = obj.EmailEnvia,
                DS_EMAIL_TO = obj.EmailRecibe,
                DT_ENVIO = obj.FechaEnvio,
                DT_RENVIO = obj.FechaRenvio
            };

            if (obj.IsHtml.HasValue)
                entity.FL_HTML = obj.IsHtml.Value ? "S" : "N";

            return entity;
        }

    }
}
