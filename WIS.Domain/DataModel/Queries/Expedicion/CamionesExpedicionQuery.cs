using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class CamionesExpedicionQuery : QueryObject<V_EXP040_CAMIONES, WISDB>
    {
        protected readonly int? _camion;
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_EXP040_CAMIONES;
            if (_camion != null)
            {
                this.Query = this.Query.Where(x => x.CD_CAMION == _camion);
            }
        }

        public CamionesExpedicionQuery(int? camion = null)
        {
            this._camion = camion;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
