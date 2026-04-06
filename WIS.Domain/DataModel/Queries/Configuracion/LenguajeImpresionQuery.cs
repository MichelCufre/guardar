using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Configuracion
{
    public class LenguajeImpresionQuery : QueryObject<V_COF010_LENGUAJE_IMPRESION, WISDB>
    {
        public LenguajeImpresionQuery()
        {
        }
        
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_COF010_LENGUAJE_IMPRESION;
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
