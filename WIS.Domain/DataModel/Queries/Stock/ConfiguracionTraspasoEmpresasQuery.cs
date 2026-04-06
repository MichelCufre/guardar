using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ConfiguracionTraspasoEmpresasQuery : QueryObject<V_STO800_TRASPASO_CONFIG, WISDB>
    {
        public ConfiguracionTraspasoEmpresasQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO800_TRASPASO_CONFIG.AsNoTracking().Select(x => x);
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
