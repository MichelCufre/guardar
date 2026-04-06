using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Stock
{
    public class STO211Query : QueryObject<V_LOG_STOCK_ENVASE, WISDB>
    {
        protected readonly string _idEnvase;
        protected readonly string _ndTpEnvase;

        public STO211Query()
        {

        }
        public STO211Query(string idEnvase, string ndTpEnvase)
        {
            this._idEnvase = idEnvase;
            this._ndTpEnvase = ndTpEnvase;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_LOG_STOCK_ENVASE.AsNoTracking();

            if (!string.IsNullOrEmpty(_idEnvase) && !string.IsNullOrEmpty(_ndTpEnvase))
            {
                this.Query = this.Query.Where(w => w.ID_ENVASE == _idEnvase && w.ND_TP_ENVASE == _ndTpEnvase);
            }

        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }

}
