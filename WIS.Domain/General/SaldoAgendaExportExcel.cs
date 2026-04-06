using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
  public  class SaldoAgendaExportExcel
    {
        public string Serie { get; set; }
        public string Factura { get; set; }
        public string TipoFactura { get; set; }
        public int Agenda { get; set; }
        public int Empresa { get; set; }
        public string Proveedor { get; set; }
        public string TipoProveeor { get; set; }
        public string Producto { get; set; }
        public string ProductoExterno { get; set; }
        public decimal CantAgendada { get; set; }
        public decimal CantRecibida { get; set; }
        public decimal CantRestante { get; set; }

    }
}
