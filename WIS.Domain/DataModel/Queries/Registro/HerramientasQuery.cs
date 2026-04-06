using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class HerramientasQuery : QueryObject<V_REG010_EQUIPO, WISDB>
    {
        public HerramientasQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG010_EQUIPO.Select(d => d);

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
