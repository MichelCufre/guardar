using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Documento
{
    public class LineaReservaCrossDocking
    {
        public string NroDocumento { get; set; }
        public string TipoDocumento { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public string Identificidaor { get; set; }
        public string EspecificaIdentificidaor { get; set; }
        public decimal Cantidad { get; set; }
    }
}
