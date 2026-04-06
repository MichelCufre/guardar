using System.Collections.Generic;

namespace WIS.Domain.Documento
{
    public class DocumentoReservaDesafectada
    {
        public List<DocumentoPreparacionReserva> ReservasModificadas { get; set; }
        public List<DocumentoPreparacionReserva> ReservasEliminadas { get; set; }

        public DocumentoReservaDesafectada()
        {
            this.ReservasEliminadas = new List<DocumentoPreparacionReserva>();
            this.ReservasModificadas = new List<DocumentoPreparacionReserva>();
        }
    }
}
