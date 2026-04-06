using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class PanelEstrategiasQuery : QueryObject<V_REC275_ESTRATEGIA, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC275_ESTRATEGIA;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
