using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DocumentosAsociadosActasQuery : QueryObject<V_DOCUMENTOS_ACTA_DOC310, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DOCUMENTOS_ACTA_DOC310;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
