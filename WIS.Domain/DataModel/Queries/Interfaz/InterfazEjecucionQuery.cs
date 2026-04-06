using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Interfaz
{
    public class InterfazEjecucionQuery : QueryObject<V_INT050_INTERFAZ_EJECUCION, WISDB>
    {
        protected readonly long? _nuInterfaz;

        public InterfazEjecucionQuery(long nuInterfaz)
        {
            this._nuInterfaz = nuInterfaz;
        }

        public InterfazEjecucionQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_INT050_INTERFAZ_EJECUCION;

            if (this._nuInterfaz != null)
                this.Query = context.V_INT050_INTERFAZ_EJECUCION.Where(w => w.NU_INTERFAZ_EJECUCION == this._nuInterfaz);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
