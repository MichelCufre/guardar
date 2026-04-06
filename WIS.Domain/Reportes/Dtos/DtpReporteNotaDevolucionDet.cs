using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Reportes.Dtos
{
	public class DtpReporteNotaDevolucionDet
	{
		public int RecepcionFactura { get; set; }
		public string Serie { get; set; }
		public string Factura { get; set; }
		public string TipoFactura { get; set; }
		public int Empresa { get; set; }
		public int? Agenda { get; set; }
		public DateTime? FechaEmision { get; set; }
		public decimal? ImporteTotalDigitado { get; set; }
		public string Cliente { get; set; }
		public string Origen { get; set; }
		public DateTime? FechaCreacion { get; set; }
		public DateTime? FechaModificacion { get; set; }
		public DateTime? Vencimiento { get; set; }
		public string Moneda { get; set; }
		public short? Situacion { get; set; }
		public string Predio { get; set; }
		public int RecepcionFacturaDet { get; set; }
		public string Producto { get; set; }
		public decimal Faixa { get; set; }
		public string Lote { get; set; }
		public decimal? CantidadFacturada { get; set; }
		public decimal? CantidadValidada { get; set; }
		public decimal? CantidadRecibida { get; set; }
		public decimal? ImporteUnitarioDigitado { get; set; }


	}
}
