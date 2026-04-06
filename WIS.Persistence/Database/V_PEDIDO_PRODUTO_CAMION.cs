using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

	[Table("V_PEDIDO_PRODUTO_CAMION")]
	public partial class V_PEDIDO_PRODUTO_CAMION
	{
		[Key]
		[Column(Order = 0)]
		public int? CD_CAMION { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

		[Key]
		[Column(Order = 2)]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		[Key]
		public int? CD_EMPRESA { get; set; }

		[Key]
		[Column(Order = 4)]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		public decimal? CD_FAIXA { get; set; }

		[Key]
		[Column(Order = 6)]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		public decimal? QT_PRODUTO { get; set; }

		public decimal? QT_PREPARADO { get; set; }

	}
}
