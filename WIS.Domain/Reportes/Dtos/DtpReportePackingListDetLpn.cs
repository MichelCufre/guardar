using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Reportes.Dtos
{
    public class DtpReportePackingListDetLpn
    {
        public int Camion { get; set; }
        public int IdLpnDet { get; set; }
        public string Producto { get; set; }
        public int Empresa { get; set; }
        public decimal Faixa { get; set; }
        public string Lote { get; set; }
        public decimal? CantidadExpedida { get; set; }
        public string NombreAtributo { get; set; }
        public string ValorAtributo { get; set; }
    }
}
