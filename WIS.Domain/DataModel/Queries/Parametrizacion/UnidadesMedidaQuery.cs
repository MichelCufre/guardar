using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Parametrizacion
{
    public class UnidadesMedidaQuery : QueryObject<V_UNIDAD_DE_MEDIDA_WPAR110, WISDB>
    {
        public UnidadesMedidaQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_UNIDAD_DE_MEDIDA_WPAR110;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
