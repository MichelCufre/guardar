using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class UnidadMedidaEmpresaQuery : QueryObject<V_FACTURA_UND_MEDI_EMP_WFAC230, WISDB>
    {
        public UnidadMedidaEmpresaQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_FACTURA_UND_MEDI_EMP_WFAC230;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
