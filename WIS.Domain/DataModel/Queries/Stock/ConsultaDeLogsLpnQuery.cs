using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class ConsultaDeLogsLpnQuery : QueryObject<V_STO721_HISTORIAL_CABEZAL, WISDB>
    {
        protected long? _numeroLpn;

        public ConsultaDeLogsLpnQuery(long? numeroLpn = null)
		{
            this._numeroLpn = numeroLpn;
		}

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_STO721_HISTORIAL_CABEZAL
                .AsNoTracking()
                .Where(d => _numeroLpn.HasValue ? d.NU_LPN == this._numeroLpn : true);                       
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}