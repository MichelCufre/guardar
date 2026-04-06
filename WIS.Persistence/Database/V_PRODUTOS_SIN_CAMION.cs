using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

	[Table("V_PRODUTOS_SIN_CAMION")]
	public partial class V_PRODUTOS_SIN_CAMION
	{

		[Key]
		[Column(Order = 0)]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		[Key]
		[Column(Order = 2)]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

		public decimal CD_FAIXA { get; set; }

		[Key]
		[Column(Order = 4)]
		public int CD_EMPRESA { get; set; }

		[Key]
		[Column(Order = 5)]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		public decimal? QT_PRODUTO { get; set; }

	}
}
