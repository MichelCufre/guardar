using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.DocumentoVistaQuery
{
    public class DocumentoVistaQuery : QueryObject<V_DOCUMENTO_DOC095, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DOCUMENTO_DOC095;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
