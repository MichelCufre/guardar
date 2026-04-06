using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Recepcion
{
    public class REC300MotivoAlmacenamientoQuery : QueryObject<V_REC300_MOTIVO_ALMACENAMIENTO, WISDB>
    {
        public REC300MotivoAlmacenamientoQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_REC300_MOTIVO_ALMACENAMIENTO;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
