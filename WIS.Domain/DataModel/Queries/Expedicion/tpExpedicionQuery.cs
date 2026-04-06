using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class TpExpedicionQuery : QueryObject<V_TIPO_EXPEDICION, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_TIPO_EXPEDICION;
        }

        public virtual List<string> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.Query.Select(r => r.TP_EXPEDICION).Intersect(keysToSelect).ToList();
        }

        public virtual List<string> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.Query.Select(r => r.TP_EXPEDICION).Except(keysToExclude).ToList();
        }
    }
}
