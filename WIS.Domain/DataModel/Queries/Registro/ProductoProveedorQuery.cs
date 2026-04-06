
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Registro
{
    public class ProductoProveedorQuery : QueryObject<V_REG015_PRODUTOS_PROVEEDOR, WISDB>
    {

        public ProductoProveedorQuery()
        {

        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REG015_PRODUTOS_PROVEEDOR;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
