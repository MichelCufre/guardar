using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Inventario
{
    public class InventarioQuery : QueryObject<V_INV410_INVENTARIO, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_INV410_INVENTARIO;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
