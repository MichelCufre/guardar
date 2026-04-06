using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

	[Table("V_EXP110_LABEL_ESTILO")]
	public partial class V_EXP110_LABEL_ESTILO
	{

		[StringLength(30)]
		public string DS_LABEL_ESTILO { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(15)]
		public string CD_LABEL_ESTILO { get; set; }

		[Required]
		[StringLength(20)]
		public string TP_LABEL { get; set; }

		[Required]
		[StringLength(10)]
		public string CD_LENGUAJE_IMPRESION { get; set; }

	}
}
