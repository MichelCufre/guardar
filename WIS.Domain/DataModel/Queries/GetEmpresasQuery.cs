using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries
{
    public class GetEmpresasQuery : QueryObject<T_EMPRESA, WISDB>
    {
        protected readonly string _filtro;

        public GetEmpresasQuery(string filtro)
        {
            this._filtro = filtro;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.T_EMPRESA.Where(e => e.NM_EMPRESA.ToLower().Contains(this._filtro.ToLower()));
        }
    }
}
