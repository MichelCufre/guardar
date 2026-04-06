using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class ProcesoFacturacionQuery : QueryObject<V_FACTURACION_PROC_FAC_WFAC251, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {

                this.Query = context.V_FACTURACION_PROC_FAC_WFAC251;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
