using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Automatismo
{
	public class AtomatismoConfirmacionEntrada
	{
		public long Ejecucion { get; set; }
		public string EjecucionEntrada { get; set; }
		public string UltimaOperacion { get; set; }
		public int Empresa { get; set; }
		public string DsReferencia { get; set; }
		public string Archivo { get; set; }
		public string Ubicacion { get; set; }
		public string UbicacionDestino { get; set; }
		public string CodigoProducto { get; set; }
		public string Identificador { get; set; }
		public decimal? Cantidad { get; set; }
		public string LoginName { get; set; }
		public string UltimaOperacionDetalle { get; set; }
		public int LineaEntrada { get; set; }
	}
}
