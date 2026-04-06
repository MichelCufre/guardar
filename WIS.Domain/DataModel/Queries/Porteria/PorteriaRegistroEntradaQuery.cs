using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Porteria
{
    public class PorteriaRegistroEntradaQuery : QueryObject<V_PORTERIA_PERSONA, WISDB>
    {
        public readonly bool _onlyToday;

        public PorteriaRegistroEntradaQuery(bool onlyToday = false)
        {
            this._onlyToday = onlyToday;
        }

        public override void BuildQuery(WISDB context)
        {
            if (_onlyToday)
            {
                DateTime ayer = DateTime.Now.AddDays(-1);
                this.Query = context.V_PORTERIA_PERSONA.Where(w => w.DT_ADDROW > ayer);
            }
            else
            {
                this.Query = context.V_PORTERIA_PERSONA;
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
