using System;
using System.Collections.Generic;

namespace WIS.AutomationManager.Models
{
    public class SalidaStockAutomatismo
    {
        public string Predio { get; set; }
        public string Id { get; set; }
        public string CodigoDestinatario { get; set; }
        public string DescripcionDestinatario { get; set; }
        public int Prioridad { get; set; }
        public DateTime? FechaServicio { get; set; }
        public string TipoSalida { get; set; }
        public string ModoLanzamiento { get; set; }
        public int Empresa { get; set; }
        public int Preparacion { get; set; }
        public string Pedido { get; set; }
        public string Observaciones { get; set; }
        public List<SalidaStockLineaAutomatismo> Detalles { get; set; }
    }

    public class SalidaStockLineaAutomatismo
    {
        public int Id { get; set; }
        public string Producto { get; set; }
        public int Empresa { get; set; }
        public string Lote { get; set; }
        public decimal Cantidad { get; set; }
    }
}
