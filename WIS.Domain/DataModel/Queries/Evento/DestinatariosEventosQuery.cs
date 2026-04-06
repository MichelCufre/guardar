using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class DestinatariosEventosQuery : QueryObject<V_CONTACTO_WEVT020, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_CONTACTO_WEVT020;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no está lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
