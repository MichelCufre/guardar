using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class NotificacionesEventosQuery : QueryObject<V_EVENTO_NOTIFICACION_WEVT050, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_EVENTO_NOTIFICACION_WEVT050;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
