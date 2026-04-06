using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Facturacion
{
    public class DetallesCalculoCOR_04Query : QueryObject<V_COR_04, WISDB>
    {
        protected int _nuEjecucion;
        protected string _cdFacturacion;
        protected int _cdEmpresa;
        protected string _nuComponente;

        public DetallesCalculoCOR_04Query(int nuEjecucion, string cdFacturacion, int cdEmpresa, string nuComponente)
        {
            this._nuEjecucion = nuEjecucion;
            this._cdFacturacion = cdFacturacion;
            this._cdEmpresa = cdEmpresa;
            this._nuComponente = nuComponente;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_COR_04.Where(e => e.NU_EJECUCION == this._nuEjecucion && e.NU_COMPONENTE == this._nuComponente && e.CD_EMPRESA == this._cdEmpresa && e.CD_PROCESO == this._cdFacturacion);
        }
    }
}
