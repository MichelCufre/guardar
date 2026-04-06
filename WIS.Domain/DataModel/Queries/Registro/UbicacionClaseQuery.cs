using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class UbicacionClaseQuery : QueryObject<V_REG035_CLASSE, WISDB>
    {
        public UbicacionClaseQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG035_CLASSE.Select(d => d);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
