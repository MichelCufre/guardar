using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries
{
    public class GetAlmacenjeSeguroQueryByDs : QueryObject<T_TIPO_ALMACENAJE_SEGURO, WISDB>
    {
        protected readonly string _filtro;

        public GetAlmacenjeSeguroQueryByDs(string filtro)
        {
            this._filtro = filtro;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.T_TIPO_ALMACENAJE_SEGURO.Where(e => e.DS_ALMACENAJE_Y_SEGURO.ToLower().Contains(this._filtro.ToLower()));
        }
    }
}
