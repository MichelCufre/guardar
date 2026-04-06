using System.Collections.Generic;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Inventario
{
    public class TpPedidoQuery : QueryObject<V_TIPO_PEDIDO, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_TIPO_PEDIDO;
        }

        public virtual List<string> GetSelectedKeys(List<string> keysToSelect)
        {
            return this.Query.Select(r => r.TP_PEDIDO).Intersect(keysToSelect).ToList();
        }

        public virtual List<string> GetSelectedKeysAndExclude(List<string> keysToExclude)
        {
            return this.Query.Select(r => r.TP_PEDIDO).Except(keysToExclude).ToList();
        }
    }
}
