using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DocumentoInfoDOC096Query : QueryObject<V_DOCUMENTO_DOC095, WISDB>
    {
        protected string _nuDocumento;
        protected string _tpDocumento;

        public DocumentoInfoDOC096Query(string nuDocumento, string tpDocumento)
        {
            this._nuDocumento = nuDocumento;
            this._tpDocumento = tpDocumento;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DOCUMENTO_DOC095
                .Where(d => d.NU_DOCUMENTO == this._nuDocumento && d.TP_DOCUMENTO == this._tpDocumento);
        }
    }
}
