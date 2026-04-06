using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class LineasIngresoPorDocumentoDOC363Query : QueryObject<V_DOC363_SALDO_LINEA_INGRESO, WISDB>
    {
        protected readonly string _nuDocumento;
        protected readonly string _tpDocumento;

        public LineasIngresoPorDocumentoDOC363Query(string nuDocumento, string tpDocumento)
        {
            this._nuDocumento = nuDocumento;
            this._tpDocumento = tpDocumento;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DOC363_SALDO_LINEA_INGRESO
                .Where(l => l.NU_DOCUMENTO == this._nuDocumento && l.TP_DOCUMENTO == this._tpDocumento);
        }
    }
}
