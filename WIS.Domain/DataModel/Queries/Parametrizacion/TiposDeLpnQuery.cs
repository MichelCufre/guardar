using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Parametrizacion
{
    public class TiposDeLpnQuery : QueryObject<V_LPN_TIPO, WISDB>
    {
        public TiposDeLpnQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_LPN_TIPO;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}