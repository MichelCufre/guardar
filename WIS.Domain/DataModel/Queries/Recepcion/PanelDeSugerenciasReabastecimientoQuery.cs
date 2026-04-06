using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class PanelDeSugerenciasReabastecimientoQuery : QueryObject<V_REC280_PANEL_SUGERENCIA_REABASTECIMIENTO, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC280_PANEL_SUGERENCIA_REABASTECIMIENTO;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
