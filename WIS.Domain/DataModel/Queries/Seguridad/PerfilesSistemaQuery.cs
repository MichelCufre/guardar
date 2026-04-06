using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Seguridad
{
    public class PerfilesSistemaQuery : QueryObject<V_SEG020_PERFILES, WISDB>
    {
        public PerfilesSistemaQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_SEG020_PERFILES;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
