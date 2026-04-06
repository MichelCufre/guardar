using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries
{
    public class GetClienteProveedorQueryCd : QueryObject<V_AGENTE, WISDB>
    {
        protected readonly string _filtro;
        protected readonly int _empresa;
        public GetClienteProveedorQueryCd(string filtro,int empresa)
        {
            this._filtro = filtro;
            this._empresa = empresa;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_AGENTE.Where(c => c.CD_AGENTE.ToLower().Contains(this._filtro.ToLower()) &&
                                                c.CD_EMPRESA == this._empresa &&
                                                c.TP_AGENTE == "PRO");
        }
    }
}
