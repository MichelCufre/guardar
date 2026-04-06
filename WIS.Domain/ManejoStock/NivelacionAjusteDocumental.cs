using System.Collections.Generic;

namespace WIS.Domain.ManejoStock
{
    public class NivelacionAjusteDocumental
    {
        public NivelacionAjusteDocumental()
        {
            this.ajustesAgregar = new List<DocumentoAjusteStock>();
            this.ajustesEliminar = new List<DocumentoAjusteStock>();
            this.ajustesModificar = new List<DocumentoAjusteStock>();
            this.ajustesHistoricosAgregar = new List<DocumentoAjusteStockHistorico>();
            this.ajustesHistoricosEliminar = new List<DocumentoAjusteStockHistorico>();
        }

        public List<DocumentoAjusteStock> ajustesAgregar { get; set; }
        public List<DocumentoAjusteStock> ajustesModificar { get; set; }
        public List<DocumentoAjusteStock> ajustesEliminar { get; set; }
        public List<DocumentoAjusteStockHistorico> ajustesHistoricosAgregar { get; set; }
        public List<DocumentoAjusteStockHistorico> ajustesHistoricosEliminar { get; set; }
    }
}
