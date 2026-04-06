using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class StockProducirBlackBoxProduccionQuery : QueryObject<V_STOCK_PRODUCIR_BB_KIT220, WISDB>
    {
        protected readonly string _nroIngreso;

        public StockProducirBlackBoxProduccionQuery()
        {
            this._nroIngreso = null;
        }

        public StockProducirBlackBoxProduccionQuery(string nroIngreso)
        {
            this._nroIngreso = nroIngreso;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STOCK_PRODUCIR_BB_KIT220
                .Where(d => d.NU_PRDC_INGRESO == this._nroIngreso 
                    && d.QT_PRODUCIDO > 0);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
