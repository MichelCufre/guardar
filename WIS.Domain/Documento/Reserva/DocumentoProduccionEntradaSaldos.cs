using System.Collections.Generic;

namespace WIS.Domain.Documento.Reserva
{
    public class DocumentoProduccionEntradaSaldos
    {
        public List<DocumentoProduccionEntrada> LineasEliminadas { get; set; }
        public List<DocumentoProduccionEntrada> LineasModificadas { get; set; }

        public DocumentoProduccionEntradaSaldos()
        {
            LineasEliminadas = new List<DocumentoProduccionEntrada>();
            LineasModificadas = new List<DocumentoProduccionEntrada>();
        }

        public virtual void AddModificados(DocumentoProduccionEntrada modificado)
        {
            LineasModificadas.Add(modificado);
        }

        public virtual void AddEliminados(DocumentoProduccionEntrada eliminado)
        {
            if (LineasModificadas.Contains(eliminado))
            {
                LineasModificadas.Remove(eliminado);
            }
            LineasEliminadas.Add(eliminado);
        }
    }
}
