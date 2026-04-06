using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

	[Table("V_REC400_ESTACIONES_DE_CLASIFICACION")]
	public partial class V_REC400_ESTACIONES_DE_CLASIFICACION
	{

		[Required]
		[StringLength(40)]
		public string CD_ENDERECO { get; set; }

		[Required]
		[Key]
		[Column(Order = 1)]
		public int CD_ESTACION { get; set; }

		[Required]
		[StringLength(50)]
		public string DS_ESTACION { get; set; }

		public DateTime DT_ADDROW { get; set; }

		public DateTime? DT_UPDROW { get; set; }

		[Required]
		[StringLength(10)]
		public string NU_PREDIO { get; set; }

	}
}
