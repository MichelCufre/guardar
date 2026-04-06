using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class StockEntradaBlackBoxPRD200Query : QueryObject<V_PRDC_KIT200_STOCK_ENTRADA_BB, WISDB>
    {
        protected string _ubicacionEntradaBB { get; }

        public StockEntradaBlackBoxPRD200Query(string ubicacionEntradaBB)
        {
            this._ubicacionEntradaBB = ubicacionEntradaBB;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRDC_KIT200_STOCK_ENTRADA_BB
                .Where(s => s.QT_RESERVA_SAIDA > 0 
                    && s.CD_ENDERECO == this._ubicacionEntradaBB);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
