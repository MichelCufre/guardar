using System;
using System.Linq;
using WIS.Data;
using WIS.Domain.Documento;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Queries
{
    public class DocumentoDetalleQuery : QueryObject<V_DET_DOCUMENTO_DOC081, WISDB>
    {
        protected readonly string NroDocumento;
        protected readonly string TipoDocumento;

        public DocumentoDetalleQuery(string nroDocumento, string tipoDocumento)
        {
            this.NroDocumento = nroDocumento;
            this.TipoDocumento = tipoDocumento;
        }

        public override void BuildQuery(WISDB context)
        {
            this.Query = context.V_DET_DOCUMENTO_DOC081
                .Where(d => d.NU_DOCUMENTO == this.NroDocumento && d.TP_DOCUMENTO == this.TipoDocumento);
        }

        public virtual ValoresTotalesDetalleDocumento GetValoresTotales(IDocumentoIngreso documento)
        {
            if (this.Query != null) {
                var detalles = this.Query.ToList();

                decimal fob = detalles.Sum(d => d.VL_MERCADERIA ?? 0) * (documento.ValorArbitraje ?? 1);
                decimal cif = fob + (documento.ValorFlete ?? 0) + (documento.ValorSeguro ?? 0) + (documento.ValorOtrosGastos ?? 0);

                return new ValoresTotalesDetalleDocumento {
                    FOB = fob,
                    CIF = cif,
                    Lineas = cif * (documento.ValorArbitraje ?? 1)
                };
            }

            throw new InvalidOperationException("Query no esta cargada. Utilizar HandleQuery antes de ejecutar esta operación");
        }
        public virtual int GetCount()
        {
            if (this.Query == null)
                throw new InvalidOperationException("La query no esta lista para hacer conteo");

            return this.Query.Count();
        }
    }
}
