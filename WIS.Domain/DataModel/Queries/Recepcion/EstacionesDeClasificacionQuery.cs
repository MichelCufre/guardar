using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class EstacionesDeClasificacionQuery : QueryObject<V_REC400_ESTACIONES_DE_CLASIFICACION, WISDB>
    {
        public EstacionesDeClasificacionQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {

            this.Query = context.V_REC400_ESTACIONES_DE_CLASIFICACION;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
