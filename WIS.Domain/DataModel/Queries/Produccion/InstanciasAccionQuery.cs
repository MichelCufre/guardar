using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class InstanciasAccionQuery : QueryObject<V_ACCION_INSTANCIA_KIT120, WISDB>
    {
        public InstanciasAccionQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ACCION_INSTANCIA_KIT120
                .Select(d => d);
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
                .Where(d => d.CD_ACCION == null);
        }
    }
}
