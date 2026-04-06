using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries
{
    public class GetDespachanteQueryByCd : QueryObject<T_DESPACHANTE, WISDB>
    {
        protected readonly short? _filtro;

        public GetDespachanteQueryByCd(short? filtro)
        {
            this._filtro = filtro;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.T_DESPACHANTE.Where(e => e.CD_DESPACHANTE == this._filtro);
        }
    }
}
