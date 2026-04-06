using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

	[Table("T_ESTACION_CLASIFICACION")]
	public partial class T_ESTACION_CLASIFICACION
	{

		[Key]
		[Column(Order = 0)]
		[Required]
		public int CD_ESTACION { get; set; }

		[Required]
		[StringLength(50)]
		public string DS_ESTACION { get; set; }

		public DateTime DT_ADDROW { get; set; }

		public DateTime? DT_UPDROW { get; set; }

		[Required]
		[StringLength(10)]
		public string NU_PREDIO { get; set; }

		[Required]
		[StringLength(40)]
		public string CD_ENDERECO { get; set; }

	}
}
