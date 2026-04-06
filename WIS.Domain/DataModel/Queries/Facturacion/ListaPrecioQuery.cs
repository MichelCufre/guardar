using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class ListaPrecioQuery : QueryObject<V_FACTURAC_LISTA_PREC_WFAC255, WISDB>
    {
        public ListaPrecioQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_FACTURAC_LISTA_PREC_WFAC255;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
