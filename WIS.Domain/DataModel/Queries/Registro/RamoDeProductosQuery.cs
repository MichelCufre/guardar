using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class RamoDeProductosQuery : QueryObject<V_REG090_RAMO_PRODUTO, WISDB>
    {

        public RamoDeProductosQuery()
        {

        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG090_RAMO_PRODUTO;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
