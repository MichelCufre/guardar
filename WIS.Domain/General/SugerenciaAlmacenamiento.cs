using iText.Layout.Element;
using System;
using System.Collections.Generic;
using WIS.Domain.Recepcion;

namespace WIS.Domain.General
{
	public class SugerenciaAlmacenamiento
	{
        public long NuAlmSugerencia { get; set; }
        public int Estrategia { get; set; }
		public OperativaAlmacenaje Operativa { get; set; }
		public string Predio { get; set; }
		public string Clase { get; set; }
		public string Grupo { get; set; }
		public int Empresa { get; set; }
		public string Producto { get; set; }
		public string Referencia { get; set; }
		public string Agrupador { get; set; }
		public string UbicacionSugerida { get; set; }
		public decimal TiempoCalculo { get; set; }
		public string Estado { get; set; }
		public string MotivoRechazo { get; set; }
		public DateTime FechaRegistro { get; set; }
		public DateTime? FechaModificacion { get; set; }
		public int? Funcionario { get; set; }
		public long? Transaccion { get; set; }
		public string Lote { get; set; }
		public decimal CantidadSeparar { get; set; }
		public decimal CantidadAuditada { get; set; }
		public decimal CantidadClasificada { get; set; }
		public DateTime? Vencimiento { get; set; }
		public bool IgnorarStock { get; set; }
		public InstanciaLogica Instancia { get; set; }
		public List<SugerenciaAlmacenamientoDetalle> Detalles { get; set; }

		public SugerenciaAlmacenamiento()
		{
			Instancia = new InstanciaLogica();
			Operativa = new OperativaAlmacenaje();
			Detalles = new List<SugerenciaAlmacenamientoDetalle>();
		}
	}
}
