namespace WIS.Persistence.Database
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("V_CONTROL_CONT_ENV_WPRE220")]
	public partial class V_CONTROL_CONT_ENV_WPRE220
	{
		[Key]
		public long? NU_LOG_CONT_PICKEO { get; set; }

		[Key]
		public int? NU_CONTENEDOR { get; set; }

		[Key]
		public int? NU_PREPARACION { get; set; }

		[Key]
		public int? CD_EMPRESA { get; set; }

		[Key]
		[StringLength(40)]
		[Column]
		public string CD_PRODUTO { get; set; }

		[Key]
		public decimal? CD_FAIXA { get; set; }

		[Key]
		[StringLength(40)]
		[Column]
		public string NU_IDENTIFICADOR { get; set; }


		[StringLength(40)]
		[Column]
		public string CD_AGENTE { get; set; }

		[StringLength(10)]
		[Column]
		public string CD_CLIENTE { get; set; }

		[StringLength(40)]
		[Column]
		public string CD_FUNC_CONTROL { get; set; }

		[StringLength(40)]
		[Column]
		public string CD_FUNC_PICKEO { get; set; }

		[StringLength(100)]
		[Column]
		public string DS_CLIENTE { get; set; }

		[StringLength(100)]
		[Column]
		public string DS_CONTROL { get; set; }

		[StringLength(65)]
		[Column]
		public string DS_PRODUTO { get; set; }

		public DateTime? DT_PICKEO { get; set; }

		public DateTime? DT_PRIMER_CTRL { get; set; }

		public DateTime? DT_ULTIMO_CTRL { get; set; }

		[StringLength(1)]
		[Column]
		public string FL_ERROR_CONTROL { get; set; }

		[StringLength(20)]
		[Column]
		public string ID_PRECINTO_1 { get; set; }

		[StringLength(20)]
		[Column]
		public string ID_PRECINTO_2 { get; set; }

		[StringLength(50)]
		[Column]
		public string LOGINNAME_CONTROL { get; set; }

		[StringLength(50)]
		[Column]
		public string LOGINNAME_PICKEO { get; set; }

		[StringLength(55)]
		[Column]
		public string NM_EMPRESA { get; set; }

		[StringLength(30)]
		[Column]
		public string NM_FUNC_CONTROL { get; set; }

		[StringLength(30)]
		[Column]
		public string NM_FUNC_PICKEO { get; set; }

		public int? NU_CONTENEDOR_DST { get; set; }

		[StringLength(10)]
		[Column]
		public string NU_PREDIO { get; set; }

		public decimal? QT_CONTROL { get; set; }

		public decimal? QT_PREPARADO { get; set; }

		[StringLength(3)]
		[Column]
		public string TP_AGENTE { get; set; }

		[StringLength(3)]
		[Column]
		public string VL_CONTROL { get; set; }

		[StringLength(50)]
		[Column]
		public string ID_EXTERNO_CONTENEDOR { get; set; }

	}
}
