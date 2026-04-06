using System;
using System.Collections.Generic;

namespace WIS.AutomationManager.Models
{
    public class EntradaStockAutomatismo
    {
        public int Empresa { get; set; }
        public string Predio { get; set; }
        public string Id { get; set; }
        public string CodigoProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public List<EntradaStockLineaAutomatismo> Detalles { get; set; }
    }

    public class EntradaStockLineaAutomatismo
    {
        public int Id { get; set; }
        public string Producto { get; set; }
        public int Empresa { get; set; }
        public string ProductoProveedor { get; set; }
        public string Carro { get; set; }
        public string UbicacionCarro { get; set; }
        public string Identificador { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal Cantidad { get; set; }
    }
}
