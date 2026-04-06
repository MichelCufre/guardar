using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class DetallesCalculoCOR_06Query : QueryObject<V_COR_06, WISDB>
    {
        protected int _nuEjecucion;
        protected string _cdFacturacion;
        protected int _cdEmpresa;
        protected string _nuComponente;

        public DetallesCalculoCOR_06Query(int nuEjecucion, string cdFacturacion, int cdEmpresa, string nuComponente)
        {
            this._nuEjecucion = nuEjecucion;
            this._cdFacturacion = cdFacturacion;
            this._cdEmpresa = cdEmpresa;
            this._nuComponente = nuComponente;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_COR_06.Where(e => e.NU_EJECUCION == this._nuEjecucion && e.NU_COMPONENTE == this._nuComponente && e.CD_EMPRESA == this._cdEmpresa && e.CD_PROCESO == this._cdFacturacion);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
