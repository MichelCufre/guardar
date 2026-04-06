
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;


namespace WIS.Persistence.Database
{

	[Table("V_PRE154_DETALLE_ATRIBUTOS_LPN_DETALLE_PEDIDO")]
	public partial class V_PRE154_DETALLE_ATRIBUTOS_LPN_DETALLE_PEDIDO
	{

		[Key]
		[Column(Order = 0)]
		[StringLength(50)]
		public string ID_LPN_EXTERNO { get; set; }

		[Required]
		[StringLength(10)]
		public string TP_LPN_TIPO { get; set; }

		[Key]
		[Column(Order = 2)]
		public long NU_DET_PED_SAI_ATRIB { get; set; }

		public DateTime DT_ADDROW { get; set; }

		public DateTime? DT_UPDROW { get; set; }

		[Key]
		[Column(Order = 5)]
		public int ID_ATRIBUTO { get; set; }

		[Key]
		[Column(Order = 6)]
		[StringLength(1)]
		public string FL_CABEZAL { get; set; }

		[Required]
		[StringLength(400)]
		public string VL_ATRIBUTO { get; set; }

		[Required]
		[StringLength(50)]
		public string NM_ATRIBUTO { get; set; }

		[StringLength(400)]
		public string DS_ATRIBUTO { get; set; }

	}
}
