using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries
{
    public class GetDespachanteQueryByNm : QueryObject<T_DESPACHANTE, WISDB>
    {
        protected readonly string _filtro;

        public GetDespachanteQueryByNm(string filtro)
        {
            this._filtro = filtro;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.T_DESPACHANTE.Where(e => e.NM_DESPACHANTE.ToLower().Contains(this._filtro.ToLower()));
        }
    }
}
