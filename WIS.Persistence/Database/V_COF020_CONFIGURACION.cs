namespace WIS.Persistence.Database
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("V_COF020_CONFIGURACION")]
	public partial class V_COF020_CONFIGURACION
	{
		[Key]
		[StringLength(15)]
		[Column]
		public string CD_LABEL_ESTILO { get; set; }

		[StringLength(30)]
		[Column]
		public string DS_LABEL_ESTILO { get; set; }

		[Required]
		[StringLength(20)]
		[Column]
		public string TP_LABEL { get; set; }

		[StringLength(100)]
		[Column]
		public string DS_DOMINIO_VALOR { get; set; }

		[Required]
		[StringLength(1)]
		[Column]
		public string FL_HABILITADO { get; set; }
	}
}
