using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Preparacion
{
    public class ColaDeTrabajoPonderadorZonaQuery : QueryObject<V_COLA_TRABAJO_POND_ZONAS, WISDB>
    {
        protected int? _colaDeTrabajo;

        public ColaDeTrabajoPonderadorZonaQuery(int colaDeTrabajo)
        {
            this._colaDeTrabajo = colaDeTrabajo;
        }

        public override void BuildQuery(WISDB context)
        {
            if (_colaDeTrabajo != null)
                this.Query = context.V_COLA_TRABAJO_POND_ZONAS.Where(w => w.NU_COLA_TRABAJO == this._colaDeTrabajo);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
