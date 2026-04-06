using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

	[Table("V_PRD_PEDIDO_LOTE_CONTENEDOR")]
	public partial class V_PRD_PEDIDO_LOTE_CONTENEDOR
	{

		public int? NU_CONTENEDOR { get; set; }

		[Key]
		[Column(Order = 1)]
		public int NU_PREPARACION { get; set; }

		[Key]
		[Column(Order = 2)]
		public int CD_EMPRESA { get; set; }

		[Key]
		[Column(Order = 3)]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		[Key]
		[Column(Order = 4)]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

		[StringLength(400)]
		public string DS_ENDERECO { get; set; }

		[StringLength(200)]
		public string VL_COMPARTE_CONTENEDOR_PICKING { get; set; }

		[StringLength(200)]
		public string VL_COMPARTE_CONTENEDOR_ENTREGA { get; set; }

		[Key]
		[Column(Order = 8)]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Key]
		[Column(Order = 9)]
		public decimal CD_FAIXA { get; set; }

		[Key]
		[Column(Order = 10)]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		public decimal? QT_PRODUTO { get; set; }

		[Required]
		[StringLength(65)]
		public string DS_PRODUTO { get; set; }

		[StringLength(200)]
		public string DS_ANEXO1 { get; set; }

		[StringLength(200)]
		public string DS_ANEXO2 { get; set; }

		[StringLength(200)]
		public string DS_ANEXO3 { get; set; }

		[StringLength(200)]
		public string DS_ANEXO4 { get; set; }

		[StringLength(1000)]
		public string DS_MEMO { get; set; }

		public int? CD_TRANSPORTADORA { get; set; }

		[StringLength(100)]
		public string DS_TRANSPORTADORA { get; set; }
		public decimal? PS_LIQUIDO_TOTAL { get; set; }
		public decimal? PS_BRUTO_TOTAL { get; set; }

		[Column]
		[StringLength(50)]
		public string ID_EXTERNO_CONTENEDOR { get; set; }

	}
}
