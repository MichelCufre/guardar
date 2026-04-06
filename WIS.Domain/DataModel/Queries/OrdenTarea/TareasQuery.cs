using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.OrdenTarea
{
    public class TareasQuery : QueryObject<V_ORT_TAREA_WORT010, WISDB>
    {
        public TareasQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ORT_TAREA_WORT010;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
