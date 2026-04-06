using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class PanelProduccionQuery : QueryObject<V_PRDC_INGRESO_KIT170, WISDB>
    {
        public PanelProduccionQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRDC_INGRESO_KIT170;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
