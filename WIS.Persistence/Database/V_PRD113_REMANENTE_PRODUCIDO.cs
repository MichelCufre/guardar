using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

	[Table("V_PRD113_REMANENTE_PRODUCIDO")]
	public partial class V_PRD113_REMANENTE_PRODUCIDO
	{

		[Key]
		[Column(Order = 0)]
		[StringLength(40)]
		public string CD_ENDERECO { get; set; }

		[Key]
		[Column(Order = 1)]
		public int? CD_EMPRESA { get; set; }

		[Key]
		[Column(Order = 2)]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Required]
		[StringLength(65)]
		public string DS_PRODUTO { get; set; }

		[Key]
		[Column(Order = 4)]
		public decimal? CD_FAIXA { get; set; }

		[Key]
		[Column(Order = 5)]
		[StringLength(100)]
		public string NU_IDENTIFICADOR { get; set; }

		public decimal? QT_ESTOQUE { get; set; }

	}
}
