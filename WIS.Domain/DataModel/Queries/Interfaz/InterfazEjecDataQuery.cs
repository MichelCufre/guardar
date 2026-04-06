using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Interfaz
{
    public class InterfazEjecDataQuery : QueryObject<V_INTERFAZ_EJEC_DATA, WISDB>
    {
        protected long _nuInterfaz;

        public InterfazEjecDataQuery(long nuInterfaz)
        {
            this._nuInterfaz = nuInterfaz;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_INTERFAZ_EJEC_DATA.Where(w => w.NU_INTERFAZ_EJECUCION == this._nuInterfaz);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
