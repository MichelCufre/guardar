using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class IngresosProduccionQuery : QueryObject<V_PRDC_INGRESO_KIT110, WISDB>
    {
        public IngresosProduccionQuery()
        {

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRDC_INGRESO_KIT110;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual void ForceBlankResult()
        {
            this.Query = this.Query
                .Where(d => d.CD_PRDC_DEFINICION == null);
        }
    }
}
