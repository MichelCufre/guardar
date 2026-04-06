using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

	[Table("V_CONSUMIDOS_PRODUCCION")]
	public partial class V_CONSUMIDOS_PRODUCCION
	{
		[Key]
		[Column(Order = 0)]
		[StringLength(50)]
		public string NU_PRDC_INGRESO { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Key]
		[Column(Order = 2)]
		public int? CD_EMPRESA { get; set; }

		public decimal? QT_TEORICO { get; set; }

		public decimal? QT_REAL { get; set; }

		public decimal? QT_CONSUMIDO { get; set; }

		public string FL_DIFERENCIA { get; set; }

		[StringLength(65)]
		public string DS_PRODUTO { get; set; }

	}
}