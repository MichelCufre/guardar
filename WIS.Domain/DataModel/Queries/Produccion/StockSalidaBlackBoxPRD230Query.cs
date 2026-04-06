using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.Produccion.Enums;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class StockSalidaBlackBoxPRD230Query : QueryObject<V_PRDC_KIT230_STOCK_SALIDA_BB, WISDB>
    {
        protected TipoStockOutBB _tipoStock { get; }
        protected string _ubicacionBB { get; }

        public StockSalidaBlackBoxPRD230Query(TipoStockOutBB tipoStock,string ubicacionBB)
        {
            this._tipoStock = tipoStock;
            this._ubicacionBB = ubicacionBB;
        }

        public override void BuildQuery(WISDB context)
        {
            if (this._tipoStock == TipoStockOutBB.INSUMO)
            {
                if(string.IsNullOrEmpty(this._ubicacionBB))
                    this.Query = context.V_PRDC_KIT230_STOCK_SALIDA_BB
                        .Where(s => s.QT_RESERVA_SAIDA > 0);
                else
                    this.Query = context.V_PRDC_KIT230_STOCK_SALIDA_BB
                        .Where(s => s.QT_RESERVA_SAIDA > 0 
                            && s.CD_ENDERECO == this._ubicacionBB);
            }
            else if (this._tipoStock == TipoStockOutBB.PRODUCTO)
            {
                if (string.IsNullOrEmpty(this._ubicacionBB))
                    this.Query = context.V_PRDC_KIT230_STOCK_SALIDA_BB.Where(s => s.QT_ESTOQUE > 0 && s.QT_RESERVA_SAIDA == 0);
                else
                    this.Query = context.V_PRDC_KIT230_STOCK_SALIDA_BB.Where(s => s.QT_ESTOQUE > 0 && s.QT_RESERVA_SAIDA == 0 && s.CD_ENDERECO == this._ubicacionBB);
            }
            else
            {
                this.Query = context.V_PRDC_KIT230_STOCK_SALIDA_BB.Where(s => s.CD_EMPRESA == -999);
            }
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
