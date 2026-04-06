using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

	[Table("V_DET_PEDIDO_PRE340")]
	public partial class V_DET_PEDIDO_PRE340
	{

		[Key]
		[Column(Order = 0)]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		[Key]
		[Column(Order = 2)]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Key]
		[Column(Order = 3)]
		public int CD_EMPRESA { get; set; }

		[Key]
		[Column(Order = 4)]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		[Required]
		[StringLength(65)]
		public string DS_PRODUTO { get; set; }

		public decimal? QT_PEDIDO { get; set; }

		public decimal? QT_ANULADO { get; set; }

		public decimal? QT_EXPEDIDO { get; set; }

		public decimal? QT_FACTURADO { get; set; }

		public decimal? QT_LIBERADO { get; set; }

		public decimal? QT_PREPARADO { get; set; }

		public decimal? QT_ENTREGAR { get; set; }

		public decimal? QT_EMPACAR { get; set; }

	}
}
