using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class CotizacionListasQuery : QueryObject<V_FACTURA_COD_LIST_COT_WFAC256, WISDB>
    {
        protected int _idListaPrecio;
        protected string _nuComponente;
        protected string _cdFacturacion;
        protected string _idFacturacion;

        public CotizacionListasQuery(int idListaPrecio = -1, string nuComponente = null, string cdFacturacion = null)
        {
            this._idListaPrecio = idListaPrecio;
            this._nuComponente = nuComponente;
            this._cdFacturacion = cdFacturacion;
        }
        public CotizacionListasQuery(string _idFacturacion)
        {
            this._idFacturacion = _idFacturacion;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_FACTURA_COD_LIST_COT_WFAC256; 

            if(this._idListaPrecio != -1)
                this.Query = this.Query.Where(w => w.CD_LISTA_PRECIO == this._idListaPrecio);

            if (!string.IsNullOrEmpty(this._idFacturacion))
                this.Query = this.Query.Where(w => w.CD_FACTURACION.ToUpper().Trim() == this._idFacturacion.ToUpper().Trim());

            if (!string.IsNullOrEmpty(_nuComponente))
                this.Query = this.Query.Where(w => w.NU_COMPONENTE.ToUpper().Trim() == this._nuComponente.ToUpper().Trim() && w.CD_FACTURACION.ToUpper().Trim() == this._cdFacturacion.ToUpper().Trim());
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
