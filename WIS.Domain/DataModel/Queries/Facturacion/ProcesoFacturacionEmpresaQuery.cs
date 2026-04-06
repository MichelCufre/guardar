using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class ProcesoFacturacionEmpresaQuery : QueryObject<V_FACTURAC_PROC_EMP_WFAC005, WISDB>
    {
        public ProcesoFacturacionEmpresaQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_FACTURAC_PROC_EMP_WFAC005;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
