using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

	[Table("V_REC410_SUGERENCIA_REABAST")]
	public partial class V_REC410_SUGERENCIA_REABAST
	{

		[Key]
		[Column(Order = 0)]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Key]
		[Column(Order = 1)]
		public int CD_EMPRESA { get; set; }

		[Key]
		[Column(Order = 2)]
		public decimal CD_FAIXA { get; set; }

		[Key]
		[Column(Order = 3)]
		[StringLength(40)]
		public string CD_ENDERECO_PI { get; set; }

		public decimal? QT_MAXIMO_REA { get; set; }

		public decimal QT_PADRAO_PI { get; set; }

		[Key]
		[Column(Order = 6)]
		[StringLength(10)]
		public string NU_PREDIO { get; set; }

	}
}
