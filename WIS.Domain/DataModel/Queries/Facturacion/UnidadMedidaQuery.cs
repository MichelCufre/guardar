using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class UnidadMedidaQuery : QueryObject<V_FACTURA_UNIDAD_MEDID_WFAC200, WISDB>
    {
        public UnidadMedidaQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_FACTURA_UNIDAD_MEDID_WFAC200;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
