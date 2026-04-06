using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Parametrizacion
{
    public class TiposDeUbicacionesQuery : QueryObject<V_TIPO_ENDERECO_WPAR050, WISDB>
    {
        public TiposDeUbicacionesQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_TIPO_ENDERECO_WPAR050;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}