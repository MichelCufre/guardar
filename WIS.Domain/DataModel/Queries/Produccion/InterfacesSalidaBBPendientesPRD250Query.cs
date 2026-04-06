using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class InterfacesSalidaBBPendientesPRD250Query : QueryObject<V_INTERFACES_SALIDA_BB_KIT250, WISDB>
    {
        public InterfacesSalidaBBPendientesPRD250Query()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_INTERFACES_SALIDA_BB_KIT250;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
