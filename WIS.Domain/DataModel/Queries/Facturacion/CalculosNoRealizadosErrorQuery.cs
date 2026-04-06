using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class CalculosNoRealizadosErrorQuery : QueryObject<V_FACTURA_ERROR_RESULT_WFAC003, WISDB>
    {
        protected int _nuEjecucion;
        public CalculosNoRealizadosErrorQuery(int nuEjecucion = -1)
        {
            this._nuEjecucion = nuEjecucion;
        }

        public override void BuildQuery(WISDB context)
        {
            if (this._nuEjecucion != -1)
                this.Query = context.V_FACTURA_ERROR_RESULT_WFAC003.Where(w => w.NU_EJECUCION == this._nuEjecucion);
            else
                this.Query = context.V_FACTURA_ERROR_RESULT_WFAC003;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
