using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class TareasOrdenQuery : QueryObject<V_ORT_ORDEN_TAREA_WORT040, WISDB>
    {
        protected string numeroOrden;
        public TareasOrdenQuery()
        {
        }
        public TareasOrdenQuery(string _numeroOrden)
        {
            this.numeroOrden = _numeroOrden;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_ORT_ORDEN_TAREA_WORT040.AsNoTracking().Select(d => d).OrderBy(x => x.NU_ORDEN_TAREA);

            if (numeroOrden != null)
                this.Query = this.Query.Where(o => o.NU_ORT_ORDEN == int.Parse(this.numeroOrden));
            else
                this.Query = context.V_ORT_ORDEN_TAREA_WORT040;

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
