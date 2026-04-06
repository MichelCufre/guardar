using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Inventario
{
    public class INV100GridQuery : QueryObject<V_INV100_ANALISIS_INVENTARIO, WISDB>
    {
        protected decimal? _nuInventario;

        public INV100GridQuery(decimal? nuInventario)
        {
            _nuInventario = nuInventario;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_INV100_ANALISIS_INVENTARIO
                .Where(w => (_nuInventario.HasValue ? w.NU_INVENTARIO == _nuInventario : true));

        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
