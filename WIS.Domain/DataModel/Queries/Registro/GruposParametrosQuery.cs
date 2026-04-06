using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class GruposParametrosQuery : QueryObject<V_REG300_PARAMETROS, WISDB>
    {
        public GruposParametrosQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG300_PARAMETROS;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
