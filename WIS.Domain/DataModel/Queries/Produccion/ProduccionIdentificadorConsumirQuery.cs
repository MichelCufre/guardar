using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class ProduccionIdentificadorConsumirQuery : QueryObject<V_PRDC_EGR_IDENT_KIT191, WISDB>
    {
        protected readonly string _ubicacion;

        public ProduccionIdentificadorConsumirQuery(string ubicacion)
        {
            this._ubicacion = ubicacion;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRDC_EGR_IDENT_KIT191;

            if (!string.IsNullOrEmpty(_ubicacion))
                this.Query = this.Query.Where(d => d.CD_ENDERECO == this._ubicacion);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
