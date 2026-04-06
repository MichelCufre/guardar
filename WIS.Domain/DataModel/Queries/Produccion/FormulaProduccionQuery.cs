using Microsoft.EntityFrameworkCore;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class FormulaProduccionQuery : QueryObject<V_PRDC_DEFINICION_KIT100, WISDB>
    {
        public FormulaProduccionQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRDC_DEFINICION_KIT100.AsNoTracking();
        }

        public virtual int GetCount()
        {
            return this.Query.Count();
        }
    }
}
