
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;


namespace WIS.Persistence.Database
{

	[Table("V_PRE153_ATRIBUTO_LPN_DETALLE_PEDIDO")]
	public partial class V_PRE153_ATRIBUTO_LPN_DETALLE_PEDIDO
	{

		[Key]
		[Column(Order = 0)]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		[Key]
		[Column(Order = 2)]
		public int CD_EMPRESA { get; set; }

		[Key]
		[Column(Order = 3)]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Key]
		[Column(Order = 4)]
		public decimal CD_FAIXA { get; set; }

		[Key]
		[Column(Order = 5)]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		[Key]
		[Column(Order = 6)]
		[StringLength(1)]
		public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

		[Key]
		[Column(Order = 7)]
		[StringLength(50)]
		public string ID_LPN_EXTERNO { get; set; }

		[Key]
		[Column(Order = 8)]
		[StringLength(10)]
		public string TP_LPN_TIPO { get; set; }

		[Key]
		[Column(Order = 9)]
		public long NU_DET_PED_SAI_ATRIB { get; set; }

		public DateTime DT_ADDROW { get; set; }
		public DateTime? DT_UPDROW { get; set; }

	}
}
