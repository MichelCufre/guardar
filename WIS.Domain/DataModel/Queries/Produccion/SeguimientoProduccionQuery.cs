using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Produccion
{
    public class SeguimientoProduccionQuery : QueryObject<V_PRDC_INGRESO_KIT150, WISDB>
    {
        protected IUnitOfWork uow { get; }

        public SeguimientoProduccionQuery(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_PRDC_INGRESO_KIT150;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
