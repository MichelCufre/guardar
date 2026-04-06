using System.Collections.Generic;

namespace WIS.Domain.Documento
{
    public class DocumentoLineaBalanceo
    {
        public List<DocumentoLinea> LineasEliminadas { get; set; }
        public List<DocumentoLinea> LineasModificadas { get; set; }
        public List<DocumentoLinea> LineasAgregadas { get; set; }

        public DocumentoLineaBalanceo()
        {
            LineasEliminadas = new List<DocumentoLinea>();
            LineasModificadas = new List<DocumentoLinea>();
            LineasAgregadas = new List<DocumentoLinea>();
        }
    }
}
