using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DOC100Query : QueryObject<V_DOC100_DOC_PREPARACION, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DOC100_DOC_PREPARACION;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
