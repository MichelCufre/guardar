using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Inventario
{
    public class InventarioAtributosQuery : QueryObject<V_INV418_ATRIBUTOS, WISDB>
    {
        protected decimal? _nuInventarioDetalle;

        public InventarioAtributosQuery(decimal? nuInventarioDetalle)
        {
            this._nuInventarioDetalle = nuInventarioDetalle;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_INV418_ATRIBUTOS.Where(d => d.NU_INVENTARIO_ENDERECO_DET == _nuInventarioDetalle);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
