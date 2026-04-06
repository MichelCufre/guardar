using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class StockLpnQuery : QueryObject<V_STO151_STOCK_LPN, WISDB>
    {
        protected readonly int _empresa;
        protected readonly string _ubicacion;
        protected readonly string _producto;
        protected readonly decimal _faixa;
        protected readonly string _identificador;

        public StockLpnQuery(int empresa, string ubicacion, string producto, decimal faixa, string identificador)
        {
            this._empresa = empresa;
            this._ubicacion = ubicacion;
            this._producto = producto;
            this._faixa = faixa;
            this._identificador = identificador;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO151_STOCK_LPN.Where(d =>
                d.CD_EMPRESA == this._empresa
                && d.CD_PRODUTO == this._producto
                && d.CD_FAIXA == this._faixa
                && d.NU_IDENTIFICADOR == this._identificador
                && d.CD_ENDERECO == this._ubicacion);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
