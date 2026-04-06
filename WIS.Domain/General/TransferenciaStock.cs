using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.General
{
    public class TransferenciaStock
    {
        public string Ubicacion { get; set; }
        public string UbicacionDestino { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public string Identificador { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Faixa { get; set; }
		public string EtiquetaOperacion { get; set; }
		public string EtiquetaInterna { get; set; }
	}
}
