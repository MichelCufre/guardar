using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class StockConsumirBlackBoxProduccionQuery : QueryObject<V_STOCK_CONSUMIR_BB_KIT210, WISDB>
    {
        protected readonly string _nroIngreso;

        public StockConsumirBlackBoxProduccionQuery()
        {
            this._nroIngreso = null;
        }

        public StockConsumirBlackBoxProduccionQuery(string nroIngreso)
        {
            this._nroIngreso = nroIngreso;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STOCK_CONSUMIR_BB_KIT210
                .Where(d => d.QT_RESERVA_SAIDA > 0 || d.QT_CONSUMIDO > 0);

            if (!string.IsNullOrEmpty(this._nroIngreso))
                this.Query = this.Query.Where(d => d.NU_PRDC_INGRESO == null || d.NU_PRDC_INGRESO == this._nroIngreso);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

        public virtual decimal GetMaxStockDisponibleProducto(string producto, int empresa, string identificador, decimal faixa)
        {
            return this.Query
                .Where(d => d.CD_PRODUTO == producto 
                    && d.CD_EMPRESA == empresa 
                    && d.NU_IDENTIFICADOR == identificador 
                    && d.CD_FAIXA == faixa)
                .Select(d => (d.QT_RESERVA_SAIDA ?? 0) + (d.QT_CONSUMIDO ?? 0)).FirstOrDefault();
        }
    }
}
