using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries
{
    public class GetAlmacenajeSeguroQuery : QueryObject<T_TIPO_ALMACENAJE_SEGURO, WISDB>
    {
        public GetAlmacenajeSeguroQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.T_TIPO_ALMACENAJE_SEGURO;
        }
    }
}
