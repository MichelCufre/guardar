namespace WIS.Persistence.Database
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;


	[Table("V_LOG_CONTROL_PICKEO_WPRE221")]
	public partial class V_LOG_CONTROL_PICKEO_WPRE221
	{
		public int? CD_EMPRESA { get; set; }

		public decimal? CD_FAIXA { get; set; }

		public int? CD_FUNC_CONTROL { get; set; }

		[StringLength(100)]
		[Column]
		public string CD_FUNC_PICKEO { get; set; }

		public short? CD_MOTIVO { get; set; }

		[StringLength(40)]
		[Column]
		public string CD_PRODUTO { get; set; }

		[StringLength(40)]
		[Column]
		public string DS_MOTIVO { get; set; }

		[Required]
		[StringLength(65)]
		[Column]
		public string DS_PRODUTO { get; set; }

		public DateTime? DT_ADDROW { get; set; }

		[StringLength(55)]
		[Column]
		public string NM_EMPRESA { get; set; }

		[StringLength(30)]
		[Column]
		public string NM_FUNCIONARIO { get; set; }

		public int? NU_CONTENEDOR { get; set; }

		[StringLength(40)]
		[Column]
		public string NU_IDENTIFICADOR { get; set; }

		[Key]
		public long NU_LOG_CONT_PICKEO { get; set; }

		[StringLength(2)]
		[Column]
		public string NU_PREDIO { get; set; }

		public int? NU_PREPARACION { get; set; }

		public decimal? QT_CONTROL { get; set; }

		[StringLength(1)]
		[Column]
		public string TP_ERROR { get; set; }

		[StringLength(1)]
		[Column]
		public string VL_CONFIRMACION { get; set; }
	}
}
