using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Evento
{
    public class PlantillasNotificacionesQuery : QueryObject<V_EVT060_TEMPLATES_NOTIFICACION, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_EVT060_TEMPLATES_NOTIFICACION;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
