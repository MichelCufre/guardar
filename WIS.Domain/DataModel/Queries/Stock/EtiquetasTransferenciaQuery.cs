using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class EtiquetasTransferenciaQuery : QueryObject<V_STO498_PALLET_TRANSFERENCIA, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO498_PALLET_TRANSFERENCIA;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
