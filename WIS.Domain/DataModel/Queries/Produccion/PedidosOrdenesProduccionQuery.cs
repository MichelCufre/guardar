using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class PedidosOrdenesProduccionQuery : QueryObject<V_PEDIDO_SAIDA_KIT130, WISDB>
    {
        public PedidosOrdenesProduccionQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PEDIDO_SAIDA_KIT130
                .AsNoTracking()
                .Where(w => !string.IsNullOrEmpty(w.CD_ORIGEN)
                    && w.CD_ORIGEN == "PRD110");
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
