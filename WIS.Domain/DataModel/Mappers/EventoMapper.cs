using System.Text;
using WIS.Domain.Eventos;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Mappers
{
    public class EventoMapper : Mapper
    {
        public EventoMapper()
        {

        }

        public virtual Bandeja MapBandejaToObject(T_EVENTO_BANDEJA e, bool addNavegables = true)
        {
            var obj = new Bandeja
            {
                ND_ESTADO = Bandeja.GetEstado(e.ND_ESTADO),
                NU_EVENTO = e.NU_EVENTO,
                NU_EVENTO_BANDEJA = e.NU_EVENTO_BANDEJA,
                VL_SEREALIZADO = e.VL_SEREALIZADO,
                DT_UPDROW = e.DT_UPDROW,
                DT_ADDROW = e.DT_ADDROW,
            };
            if (addNavegables)
            {
                obj.Evento = MapEventoToObject(e.T_EVENTO);
                foreach (var i in e.T_EVENTO_BANDEJA_INSTANCIA)
                    obj.LstInstanciaBandeja.Add(MapInstanciaBandejaToObject(i));
            }
            return obj;
        }

        public virtual T_EVENTO_BANDEJA_INSTANCIA MapInstanciaBandejaToEntity(InstanciaBandeja e, bool addNavegables = true)
        {
            var obj = new T_EVENTO_BANDEJA_INSTANCIA
            {
                ND_ESTADO = e.ND_ESTADO.ToString(),
                NU_EVENTO_BANDEJA = e.NU_EVENTO_BANDEJA,
                NU_EVENTO_BANDEJA_INSTANCIA = e.NU_EVENTO_BANDEJA_INSTANCIA,
                NU_EVENTO_INSTANCIA = e.NU_EVENTO_INSTANCIA,
                DT_ADDROW = e.DT_ADDROW,
                DT_UPDROW = e.DT_UPDROW,
            };

            return obj;
        }

        public virtual T_EVENTO_BANDEJA MapBandejaToEntity(Bandeja e, bool addNavegables = true)
        {
            var obj = new T_EVENTO_BANDEJA
            {
                ND_ESTADO = e.ND_ESTADO.ToString(),
                NU_EVENTO = e.NU_EVENTO,
                NU_EVENTO_BANDEJA = e.NU_EVENTO_BANDEJA,
                VL_SEREALIZADO = e.VL_SEREALIZADO,
                DT_ADDROW = e.DT_ADDROW,
                DT_UPDROW = e.DT_UPDROW,
            };

            return obj;
        }

        public virtual InstanciaBandeja MapInstanciaBandejaToObject(T_EVENTO_BANDEJA_INSTANCIA e, bool addNavegables = true)
        {
            var obj = new InstanciaBandeja
            {
                ND_ESTADO = Bandeja.GetEstado(e.ND_ESTADO),
                NU_EVENTO_BANDEJA = e.NU_EVENTO_BANDEJA,
                NU_EVENTO_BANDEJA_INSTANCIA = e.NU_EVENTO_BANDEJA_INSTANCIA,
                NU_EVENTO_INSTANCIA = e.NU_EVENTO_INSTANCIA,

            };
            if (addNavegables)
            {
                obj.Instancia = MapInstanciaToObject(e.T_EVENTO_INSTANCIA);
             }
            return obj;
        }

        public virtual Evento MapEventoToObject(T_EVENTO entity, bool addNavegables = true)
        {
            var obj = new Evento
            {
                Id = entity.NU_EVENTO,
                Descripcion = entity.DS_EVENTO,
                EsProgramado = MapStringToBoolean(entity.FL_PROGRAMADO),
                Nombre = entity.NM_EVENTO,
            };
            return obj;
        }

        public virtual EventoTemplate MapEventoTemplateToObject(T_EVENTO_TEMPLATE entity, bool addNavegables = true)
        {
            EventoTemplate obj = null;

            if (entity != null)
            {
                obj = new EventoTemplate
                {
                    nuEvento = entity.NU_EVENTO,
                    dsEstilo = entity.DS_LABEL_ESTILO,
                    CdEstilo = entity.CD_LABEL_ESTILO,
                    Asunto = entity.VL_ASUNTO,
                    TipoNotificacion = Notificacion.GetTipoNotificacion(entity.TP_NOTIFICACION),
                    FechaAlta = entity.DT_ADDROW,
                    FechaModificacion = entity.DT_UPDROW,
                    NumeroTransaccion = entity.NU_TRANSACCION,
                    NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE
                };

                if (!string.IsNullOrEmpty(entity.FL_HTML))
                    obj.IsHtml = entity.FL_HTML.ToUpper() == "S";

                if (entity.VL_CUERPO != null)
                    obj.Cuerpo = Encoding.UTF8.GetString(entity.VL_CUERPO);
                else
                    obj.Cuerpo = "";
            }

            return obj;
        }

        public virtual T_EVENTO_TEMPLATE MapEventoTemplateToEntity(EventoTemplate e, bool addNavegables = true)
        {
            var obj = new T_EVENTO_TEMPLATE
            {
                NU_EVENTO = e.nuEvento,
                DS_LABEL_ESTILO = e.dsEstilo,
                CD_LABEL_ESTILO = e.CdEstilo,
                VL_CUERPO = Encoding.UTF8.GetBytes(e.Cuerpo),
                VL_ASUNTO = e.Asunto,
                TP_NOTIFICACION = e.TipoNotificacion.ToString(),
                DT_ADDROW = e.FechaAlta,
                DT_UPDROW = e.FechaModificacion,
                NU_TRANSACCION = e.NumeroTransaccion,
                NU_TRANSACCION_DELETE = e.NumeroTransaccionDelete
            };

            if (e.IsHtml.HasValue)
                obj.FL_HTML = e.IsHtml.Value ? "S" : "N";

            return obj;
        }

        public virtual Evento MapNotificacionEmailToObject(T_EVENTO entity, bool addNavegables = true)
        {
            var obj = new Evento
            {
                Id = entity.NU_EVENTO,
                Descripcion = entity.DS_EVENTO,
                EsProgramado = MapStringToBoolean(entity.FL_PROGRAMADO),
                Nombre = entity.NM_EVENTO,
            };

            return obj;
        }



        public virtual T_EVENTO MapEventoToEntity(Evento obj, bool addNavegables = true)
        {
            var entity = new T_EVENTO
            {
                NU_EVENTO = obj.Id,
                DS_EVENTO = obj.Descripcion,
                FL_PROGRAMADO = MapBooleanToString(obj.EsProgramado),
                NM_EVENTO = obj.Nombre,
            };

            return entity;
        }

        public virtual Instancia MapInstanciaToObject(T_EVENTO_INSTANCIA entity, bool addNavegables = true)
        {
            var obj = new Instancia
            {
                Id = entity.NU_EVENTO_INSTANCIA,
                Descripcion = entity.DS_INSTANCIA,
                EsHabilitado = MapStringToBoolean(entity.FL_HABILITADO),
                NumeroEvento = entity.NU_EVENTO,
                NombreEvento = entity.T_EVENTO?.NM_EVENTO,
                Plantilla = entity.CD_LABEL_ESTILO,
                TipoNotificacion = Notificacion.GetTipoNotificacion(entity.TP_NOTIFICACION),
                IdTipoNotificacion = entity.TP_NOTIFICACION,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE
            };

            if (addNavegables)
            {
                foreach (var p in entity.T_EVENTO_PARAMETRO_INSTANCIA)
                    obj.Parametros.Add(MapParametroInstanciaToObject(p));

                obj.Template = MapEventoTemplateToObject(entity.T_EVENTO_TEMPLATE);
            }

            return obj;
        }

        public virtual T_EVENTO_INSTANCIA MapInstanciaToEntity(Instancia obj, bool addNavegables = true)
        {
            var entity = new T_EVENTO_INSTANCIA
            {
                NU_EVENTO_INSTANCIA = obj.Id,
                DS_INSTANCIA = obj.Descripcion,
                FL_HABILITADO = MapBooleanToString(obj.EsHabilitado),
                NU_EVENTO = obj.NumeroEvento,
                CD_LABEL_ESTILO = obj.Plantilla,
                TP_NOTIFICACION = Notificacion.GetTipoNotificacion(obj.TipoNotificacion),
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete
            };

            return entity;
        }

        public virtual T_EVENTO_INSTANCIA_EJECUCION MapToEntity(EjecucionInstancia ejecucionInstancia)
        {
            var entity = new T_EVENTO_INSTANCIA_EJECUCION
            {
                NU_EVENTO_INSTANCIA = ejecucionInstancia.NumeroInstancia,
                DT_ULT_EJECUCION = ejecucionInstancia.FechaUltimaEjecucion,
            };

            return entity;
        }

        public virtual ParametroEvento MapParametroToObject(T_EVENTO_PARAMETRO entity, bool addNavegables = true)
        {
            var obj = new ParametroEvento
            {
                Codigo = entity.CD_EVENTO_PARAMETRO,
                Descripcion = entity.DS_EVENTO_PARAMETRO,
                TipoNotificacion = Notificacion.GetTipoNotificacion(entity.TP_NOTIFICACION),
                ExpresionRegular = entity.VL_EXPRESION_REGULAR,
                NumeroEvento = entity.NU_EVENTO,
                EsRequerido = MapStringToBoolean(entity.FL_REQUERIDO),
            };

            return obj;
        }

        public virtual T_EVENTO_PARAMETRO MapParametroToEntity(ParametroEvento obj, bool addNavegables = true)
        {
            var entity = new T_EVENTO_PARAMETRO
            {
                CD_EVENTO_PARAMETRO = obj.Codigo,
                DS_EVENTO_PARAMETRO = obj.Descripcion,
                VL_EXPRESION_REGULAR = obj.ExpresionRegular,
                TP_NOTIFICACION = Notificacion.GetTipoNotificacion(obj.TipoNotificacion),
                NU_EVENTO = obj.NumeroEvento,
                FL_REQUERIDO = MapBooleanToString(obj.EsRequerido),
            };

            return entity;
        }

        public virtual ParametroInstancia MapParametroInstanciaToObject(T_EVENTO_PARAMETRO_INSTANCIA entity, bool addNavegables = true)
        {
            var obj = new ParametroInstancia
            {
                Codigo = entity.CD_EVENTO_PARAMETRO,
                NumeroInstancia = entity.NU_EVENTO_INSTANCIA,
                Valor = entity.VL_PARAMETRO,
                NuEvento = entity.NU_EVENTO,
                FechaAlta = entity.DT_ADDROW,
                FechaModificacion = entity.DT_UPDROW,
                NumeroTransaccion = entity.NU_TRANSACCION,
                NumeroTransaccionDelete = entity.NU_TRANSACCION_DELETE,
            };

            return obj;
        }

        public virtual T_EVENTO_PARAMETRO_INSTANCIA MapParametroInstanciaToEntity(ParametroInstancia obj, bool addNavegables = true)
        {
            var entity = new T_EVENTO_PARAMETRO_INSTANCIA
            {
                CD_EVENTO_PARAMETRO = obj.Codigo,
                NU_EVENTO_INSTANCIA = obj.NumeroInstancia,
                VL_PARAMETRO = obj.Valor,
                TP_NOTIFICACION = Notificacion.GetTipoNotificacion(obj.TipoNotificacion),
                NU_EVENTO = obj.NuEvento,
                DT_ADDROW = obj.FechaAlta,
                DT_UPDROW = obj.FechaModificacion,
                NU_TRANSACCION = obj.NumeroTransaccion,
                NU_TRANSACCION_DELETE = obj.NumeroTransaccionDelete,
            };

            return entity;
        }

        public virtual EjecucionInstancia MapToObject(T_EVENTO_INSTANCIA_EJECUCION entity)
        {
            var ejecucionInstancia = new EjecucionInstancia
            {
                NumeroInstancia = entity.NU_EVENTO_INSTANCIA,
                FechaUltimaEjecucion = entity.DT_ULT_EJECUCION,
            };

            return ejecucionInstancia;
        }
    }
}
