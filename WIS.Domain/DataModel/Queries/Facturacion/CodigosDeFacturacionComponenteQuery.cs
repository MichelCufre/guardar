using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class CodigosDeFacturacionComponenteQuery : QueryObject<V_FACTURAC_CODIGO_COMP_WFAC250, WISDB>
    {
        protected string _CodigoFactura;
        public CodigosDeFacturacionComponenteQuery(string _CodigoFactura = "NoParametros")
        {
            this._CodigoFactura = _CodigoFactura;
        }

        public override void BuildQuery(WISDB context)
        {
            if (this._CodigoFactura != "NoParametros")
                this.Query = context.V_FACTURAC_CODIGO_COMP_WFAC250.Where(w => w.CD_FACTURACION.ToUpper().Trim() == _CodigoFactura.ToUpper().Trim());
            else
                this.Query = context.V_FACTURAC_CODIGO_COMP_WFAC250;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
