using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class GruposReglasQuery : QueryObject<V_REG300_REGLAS, WISDB>
    {
        public GruposReglasQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG300_REGLAS;

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
