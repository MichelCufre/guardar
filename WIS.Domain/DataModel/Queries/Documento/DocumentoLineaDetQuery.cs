using System;
using System.Linq;
using WIS.Data;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries.Documento
{
    public class DocumentoLineaDetQuery : QueryObject<V_DOCUMENTO_LINEA_DET_DOC082, WISDB>
    {

        protected readonly string NroDocumento;
        protected readonly string TipoDocumento;
        protected readonly string producto;
        protected readonly string lote;
        protected readonly int empresa;
        protected readonly decimal faixa;

        public DocumentoLineaDetQuery(string producto, string lote, decimal faixa, int empresa, string nroDocumento, string tipoDocumento)
        {
            this.NroDocumento = nroDocumento;
            this.TipoDocumento = tipoDocumento;
            this.producto = producto;
            this.lote = lote;
            this.empresa = empresa;
            this.faixa = faixa;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DOCUMENTO_LINEA_DET_DOC082
                .Where(d => d.CD_FAIXA == faixa
                    && d.CD_EMPRESA == empresa
                    && d.CD_PRODUTO == producto
                    && d.NU_IDENTIFICADOR == lote
                    && d.NU_DOCUMENTO == NroDocumento
                    && d.TP_DOCUMENTO == TipoDocumento);
        }

        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
