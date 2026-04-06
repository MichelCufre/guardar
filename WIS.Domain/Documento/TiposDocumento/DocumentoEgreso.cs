using System.Collections.Generic;

namespace WIS.Domain.Documento.TiposDocumento
{
    public class DocumentoEgreso : Documento, IDocumentoEgreso
    {
        public List<DocumentoLineaEgreso> OutDetail { get; set; }

        public DocumentoEgreso()
        {
            this.OutDetail = new List<DocumentoLineaEgreso>();
        }

        public override void Cancelar()
        {
            base.Cancelar();
            Camion = null;
        }
    }
}