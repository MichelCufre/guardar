using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class ProductosSinUbicacionDePreparacionQuery : QueryObject<V_PRODS_SIN_PICKING_WREG060, WISDB>
    {

        public ProductosSinUbicacionDePreparacionQuery()
        {

        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRODS_SIN_PICKING_WREG060;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
