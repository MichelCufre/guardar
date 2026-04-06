using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Evento
{
    public class NotificacionesArchivosQuery : QueryObject<V_EVENTO_ARCHIVO_WEVT050, WISDB>
    {
        protected int _nuEventoNotificacion;

        public NotificacionesArchivosQuery(int nuEventoNotificacion)
        { 
            this._nuEventoNotificacion = nuEventoNotificacion;  
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_EVENTO_ARCHIVO_WEVT050
                .Where(x => x.NU_EVENTO_NOTIFICACION == _nuEventoNotificacion);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
