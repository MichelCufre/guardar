using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ConsultaLogsCabezalesAtributosQuery : QueryObject<V_STO710_LT_LPN_ATRIBUTO_CABEZAL, WISDB>
    {
        protected long? _numeroLPN;

        public ConsultaLogsCabezalesAtributosQuery(long? numeroLPN = null)
        {
            this._numeroLPN = numeroLPN;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO710_LT_LPN_ATRIBUTO_CABEZAL
                .AsNoTracking()
                .Where(d => _numeroLPN.HasValue ? d.NU_LPN == this._numeroLPN : true);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}