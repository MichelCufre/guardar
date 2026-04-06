using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DocumentoAgrupadorDOC320Query : QueryObject<V_DOCUMENTO_AGRUPADOR_DOC320, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DOCUMENTO_AGRUPADOR_DOC320;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
