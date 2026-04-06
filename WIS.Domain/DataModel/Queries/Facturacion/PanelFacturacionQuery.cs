using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class PanelFacturacionQuery : QueryObject<V_FACTURAC_EJECUCION_WFAC001, WISDB>
    {
        public PanelFacturacionQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_FACTURAC_EJECUCION_WFAC001;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
