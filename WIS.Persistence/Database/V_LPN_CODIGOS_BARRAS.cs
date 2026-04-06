using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;


namespace WIS.Persistence.Database
{

	[Table("V_LPN_CODIGOS_BARRAS")]
	public partial class V_LPN_CODIGOS_BARRAS
	{

		[Key]
		[Column(Order = 0)]
		public long NU_LPN { get; set; }

		[Required]
		[StringLength(10)]
		public string TP_LPN_TIPO { get; set; }

		[Required]
		[StringLength(50)]
		public string ID_LPN_EXTERNO { get; set; }

		[Required]
		[StringLength(100)]
		public string CD_BARRAS { get; set; }

		[Key]
		[Column(Order = 4)]
		public int ID_LPN_BARRAS { get; set; }

		[Required]
		[StringLength(2)]
		public string TP_BARRAS { get; set; }

	}
}
