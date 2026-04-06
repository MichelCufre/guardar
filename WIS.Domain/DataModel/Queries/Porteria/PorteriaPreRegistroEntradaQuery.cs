using Microsoft.EntityFrameworkCore;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Porteria
{
    public class PorteriaPreRegistroEntradaQuery : QueryObject<V_PORTERIA_VEHICULO, WISDB>
    {
        public PorteriaPreRegistroEntradaQuery()
        {

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PORTERIA_VEHICULO.AsNoTracking()
                .Where(w => w.DT_PORTERIA_ENTRADA == null && w.DT_PORTERIA_SALIDA == null);
        }

    }
}
