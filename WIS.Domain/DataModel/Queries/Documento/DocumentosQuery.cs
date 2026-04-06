using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DocumentosQuery : QueryObject<V_LT_DOCUMENTO, WISDB>
    {
        protected readonly string _nuDocumento;
        protected readonly string _tpDocumento;

        public DocumentosQuery(string nuDocumento, string tpDocumento)
        {
            this._nuDocumento = nuDocumento;
            this._tpDocumento = tpDocumento;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_LT_DOCUMENTO
                .Where(l => l.NU_DOCUMENTO == _nuDocumento
                   && l.TP_DOCUMENTO == _tpDocumento);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
