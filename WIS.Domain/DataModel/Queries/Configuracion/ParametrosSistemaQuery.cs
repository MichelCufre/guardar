using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Configuracion
{
    public class ParametrosSistemaQuery : QueryObject<V_LPARAMETROS_LCON010, WISDB>
    {
        public ParametrosSistemaQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_LPARAMETROS_LCON010;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
