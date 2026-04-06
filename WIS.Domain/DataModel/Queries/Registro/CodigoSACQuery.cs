using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class CodigoSACQuery : QueryObject<V_NAM_WREG410, WISDB>
    {
        public CodigoSACQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_NAM_WREG410;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
