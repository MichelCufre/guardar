using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
	[Table("V_STOCK_DISP_ESPECIFICAR_LOTE")]
	public class V_STOCK_DISP_ESPECIFICAR_LOTE
	{
		public string NU_PREDIO { get; set; }
		[Key]
		[Column(Order = 0)]
		public string CD_ENDERECO { get; set; }
		[Key]
		[Column(Order = 1)]
		public int CD_EMPRESA { get; set; }
		[Key]
		[Column(Order = 2)]
		public string CD_PRODUTO { get; set; }
		[Key]
		[Column(Order = 3)]
		public decimal CD_FAIXA { get; set; }
		[Key]
		[Column(Order = 4)]
		public string NU_IDENTIFICADOR { get; set; }
		public decimal? QT_ESTOQUE { get; set; }
		public decimal? QT_RESERVA_SAIDA { get; set; }
		public decimal? QT_DISP { get; set; }
		public DateTime? DT_VENCIMIENTO { get; set; }
		public string ID_ESTOQUE_GERAL { get; set; }
		public string ID_AREA_PICKING { get; set; }
		public double? VL_LARGO { get; set; }
		public double? VL_ANCHO { get; set; }
		public string ID_BOBINA_PADRE { get; set; }
		public int? NU_SEQUENCIA_BOBINA { get; set; }
		public int? NU_PALLET_SEQUENCIA { get; set; }
		public int? NU_PALLET_PADRE { get; set; }
	}
}
