using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class DetallesEjecucionQuery : QueryObject<V_FACTURACION_PROC_WFAC004, WISDB>
    {
        protected int _nuEjecucion;

        public DetallesEjecucionQuery(int nuEjecucion)
        {
            this._nuEjecucion = nuEjecucion;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_FACTURACION_PROC_WFAC004.Where(w => w.NU_EJECUCION == this._nuEjecucion);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
