using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class DetalleInterfacesSalidaBBPRD260Query : QueryObject<V_DET_INT_SALIDA_BB_KIT260, WISDB>
    {
        protected readonly long _ejecucion = -1;

        public DetalleInterfacesSalidaBBPRD260Query(long ejecucion)
        {
            this._ejecucion = ejecucion;
        }

        public DetalleInterfacesSalidaBBPRD260Query()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DET_INT_SALIDA_BB_KIT260
                .Where(d => d.NU_INTERFAZ_EJECUCION == this._ejecucion);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
