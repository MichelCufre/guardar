using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class StockPickingMalAsignadoQuery : QueryObject<V_PRE360_STOCK_PICKING_MAL, WISDB>
    {
        protected readonly int? _idEmpresa;
        protected readonly string _ubicacion;
        protected readonly string _producto;
        public StockPickingMalAsignadoQuery()
        {

        }
        public StockPickingMalAsignadoQuery(int idEmpresa, string ubicacion, string producto)
        {
            this._idEmpresa = idEmpresa;
            this._producto = producto;
            this._ubicacion = ubicacion;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRE360_STOCK_PICKING_MAL;
            if(this._idEmpresa != null && !string.IsNullOrEmpty(this._ubicacion) && !string.IsNullOrEmpty(this._producto))
            {
                this.Query = this.Query.Where(x => x.CD_EMPRESA == this._idEmpresa && x.CD_PRODUTO == this._producto && x.CD_ENDERECO == this._ubicacion);
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
