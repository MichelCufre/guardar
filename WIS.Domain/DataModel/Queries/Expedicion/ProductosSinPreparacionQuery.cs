using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Expedicion
{
    public class ProductosSinPreparacionQuery : QueryObject<V_EXP045_PRODUCTOS_SIN_PREP, WISDB>
    {
        protected readonly int? _camion;
        public ProductosSinPreparacionQuery()
        {

        }
        public ProductosSinPreparacionQuery(int camion)
        {
            this._camion = camion;
        }
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_EXP045_PRODUCTOS_SIN_PREP;
            if(this._camion != null)
            {
                this.Query = this.Query.Where(x => x.CD_CAMION == this._camion);
            }
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }

    }
}
