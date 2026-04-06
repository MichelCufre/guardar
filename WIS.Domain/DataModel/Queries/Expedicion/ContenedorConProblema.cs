using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class ContenedorConProblema : QueryObject<V_CONTENEDOR_CON_PROBLEMA, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_CONTENEDOR_CON_PROBLEMA;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
