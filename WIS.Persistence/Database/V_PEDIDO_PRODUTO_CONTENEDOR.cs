using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

	[Table("V_PEDIDO_PRODUTO_CONTENEDOR")]
	public partial class V_PEDIDO_PRODUTO_CONTENEDOR
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
		[Required]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Key]
		[Column(Order = 4)]
		[Required]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		[Key]
		[Column(Order = 5)]
		[Required]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

		[StringLength(200)]
		public string VL_COMPARTE_CONTENEDOR_ENTREGA { get; set; }

		[StringLength(100)]
		public string DS_ENTREGA { get; set; }

		[StringLength(6)]
		public string TP_EXPEDICION { get; set; }

		public decimal? QT_PRODUTO { get; set; }

		[StringLength(1000)]
		public string DS_MEMO { get; set; }

	}
}
