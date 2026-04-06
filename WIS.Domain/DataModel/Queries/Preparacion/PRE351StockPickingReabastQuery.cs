using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class PRE351StockPickingReabastQuery : QueryObject<V_PRE351_STOCK_PICKING_REABAST, WISDB>
    {
        public readonly int _cdEmpresa;
        public readonly string _cdProducto;
        public readonly string _cdEnderecoPicking;
        public readonly decimal _cdFaixa;

        public PRE351StockPickingReabastQuery() { }
        
        public PRE351StockPickingReabastQuery(int cdEmpresa, string cdProducto, string cdEnderecoPicking, decimal cdFaixa)
        {
            this._cdEmpresa = cdEmpresa;
            this._cdProducto = cdProducto;
            this._cdEnderecoPicking = cdEnderecoPicking;
            this._cdFaixa = cdFaixa;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE351_STOCK_PICKING_REABAST.AsNoTracking();

            this.Query = this.Query.Where(s => s.CD_EMPRESA == _cdEmpresa && s.CD_PRODUTO == _cdProducto && s.CD_ENDERECO_PICKING == _cdEnderecoPicking && s.CD_FAIXA == _cdFaixa);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
