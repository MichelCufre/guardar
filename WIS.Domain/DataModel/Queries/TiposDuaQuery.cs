using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries
{
    public class TiposDuaQuery : QueryObject<T_TIPO_DUA, WISDB>
    {
        public TiposDuaQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.T_TIPO_DUA;
        }
    }
}
