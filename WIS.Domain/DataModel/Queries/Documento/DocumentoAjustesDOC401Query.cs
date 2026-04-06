using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public   class DocumentoAjustesDOC401Query : QueryObject<V_CAMBIO_DOC_DOC401, WISDB>
    {
        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_CAMBIO_DOC_DOC401;
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
