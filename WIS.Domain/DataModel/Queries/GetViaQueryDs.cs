using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries
{
    public class GetViaQueryDs : QueryObject<T_VIA, WISDB>
    {
        protected readonly string _filtro;

        public GetViaQueryDs(string filtro)
        {
            this._filtro = filtro;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.T_VIA.Where(e => e.DS_VIA.ToLower().Contains(this._filtro.ToLower()));
        }
    }
}
