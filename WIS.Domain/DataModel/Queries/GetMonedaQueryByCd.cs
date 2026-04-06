using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries
{
    public class GetMonedaQueryByCd : QueryObject<T_MONEDA, WISDB>
    {
        protected readonly string _filtro;

        public GetMonedaQueryByCd(string filtro)
        {
            this._filtro = filtro;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.T_MONEDA.Where(e => e.CD_MONEDA.ToLower().Contains(this._filtro.ToLower()));
        }
    }
}
