using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries
{
    public class GetMonedaQuery : QueryObject<T_MONEDA, WISDB>
    {
        public GetMonedaQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.T_MONEDA;
        }
    }
}
