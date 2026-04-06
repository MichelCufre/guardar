using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class UbicacionSuperClaseQuery : QueryObject<V_REG036_SUB_CLASSE, WISDB>
    {
        public UbicacionSuperClaseQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG036_SUB_CLASSE.Select(d => d);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
