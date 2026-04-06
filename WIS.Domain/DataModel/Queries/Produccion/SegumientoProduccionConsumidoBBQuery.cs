using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class SegumientoProduccionConsumidoBBQuery : QueryObject<V_PRDC_CONSUMIDO_BB_KIT151, WISDB>
    {
        protected string _nuPrdcIngreso { get; }

        public SegumientoProduccionConsumidoBBQuery(string nuPrdcIngreso)
        {
            this._nuPrdcIngreso = nuPrdcIngreso;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRDC_CONSUMIDO_BB_KIT151;

            if (string.IsNullOrEmpty(this._nuPrdcIngreso))
                this.Query = this.Query
                    .Where(i => i.NU_PRDC_INGRESO == this._nuPrdcIngreso);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
