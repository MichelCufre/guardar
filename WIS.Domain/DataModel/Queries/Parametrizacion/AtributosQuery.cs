using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Parametrizacion
{
    public class AtributosQuery : QueryObject<V_PAR400_ATRIBUTOS, WISDB>
    {
        public AtributosQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PAR400_ATRIBUTOS;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}