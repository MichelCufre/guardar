using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Seguridad
{
    public class ResourcesQuery : QueryObject<V_RESOURCES_WSEG020, WISDB>
    {
        public ResourcesQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_RESOURCES_WSEG020;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
