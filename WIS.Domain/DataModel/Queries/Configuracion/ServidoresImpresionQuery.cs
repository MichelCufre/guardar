using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Configuracion
{
    public class ServidoresImpresionQuery : QueryObject<V_COF060_SERVIDORES_IMPRESION, WISDB>
    {
        public ServidoresImpresionQuery()
        {
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_COF060_SERVIDORES_IMPRESION;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
