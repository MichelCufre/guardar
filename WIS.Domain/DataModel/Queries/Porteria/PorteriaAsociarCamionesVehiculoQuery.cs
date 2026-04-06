using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Porteria
{
    public class PorteriaAsociarCamionesVehiculoQuery : QueryObject<V_PORTERIA_SALIDA_SIN_EGRESO, WISDB>
    {
        public PorteriaAsociarCamionesVehiculoQuery()
        {

        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PORTERIA_SALIDA_SIN_EGRESO;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
