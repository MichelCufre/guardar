using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

	[Table("V_PAR401_ASOCIAR_ATRIBUTO_TIPO_DET")]
	public partial class V_PAR401_ASOCIAR_ATRIBUTO_TIPO_DET
	{

		[Key]
		[Column(Order = 0)]
		public int ID_ATRIBUTO { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(50)]
		public string NM_ATRIBUTO { get; set; }

		[Key]
		[Column(Order = 2)]
		[StringLength(10)]
		public string TP_LPN_TIPO { get; set; }

	}
}
