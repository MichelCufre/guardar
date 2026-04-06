namespace WIS.Persistence.Database
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("V_PRD113_PRODUCIR")]
	public partial class V_PRD113_PRODUCIR
	{
		[Key]
		public string NU_PRDC_INGRESO { get; set; }

		[Key]
		public long NU_PRDC_DET_TEORICO { get; set; }

		public string CD_PRODUTO { get; set; }

		public string DS_PRODUTO { get; set; }

		public string NU_IDENTIFICADOR { get; set; }

		public decimal? QT_PRODUCIDO { get; set; }

		public decimal? QT_TEORICO { get; set; }

		public decimal? CD_FAIXA { get; set; }

        public int? CD_EMPRESA { get; set; }
    }
}
