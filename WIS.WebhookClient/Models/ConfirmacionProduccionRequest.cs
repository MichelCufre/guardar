using System.Collections.Generic;
using WIS.Common.API.Attributes;

namespace WIS.WebhookClient.Models
{
    public class ConfirmacionProduccionRequest
    {
        public string IdProduccionExterno { get; set; }

        public int Empresa { get; set; }
        
        public string Tipo { get; set; }
        
        public string Predio { get; set; }
        
        public string EspacioProduccion { get; set; }
        
        public string FechaInicioProduccion { get; set; }
        
        public string FechaFinProduccion { get; set; }
        
        public string Anexo1 { get; set; }
        
        public string Anexo2 { get; set; }
        
        public string Anexo3 { get; set; }
        
        public string Anexo4 { get; set; }
        
        public string Anexo5 { get; set; }
        
        public string FinProduccion { get; set; }

        public List<InsumoProduccionRequest> Insumos { get; set; }

        public List<ProductoProduccionRequest> Productos { get; set; }
    }
}
