
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class ParametrosQuery : QueryObject<V_REC275_PARAMETROS, WISDB>
    {
        protected short _numeroLogica;

        public ParametrosQuery(short numeroLogica = 0)
        {
            this._numeroLogica = numeroLogica;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC275_PARAMETROS.AsNoTracking();

            if (this._numeroLogica != 0)
            {
                this.Query = this.Query.Where(d => d.NU_ALM_LOGICA == this._numeroLogica);
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