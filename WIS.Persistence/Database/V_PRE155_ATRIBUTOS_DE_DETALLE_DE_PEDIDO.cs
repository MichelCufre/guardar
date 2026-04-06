
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;


namespace WIS.Persistence.Database
{

	[Table("V_PRE155_ATRIBUTOS_DE_DETALLE_DE_PEDIDO")]
	public partial class V_PRE155_ATRIBUTOS_DE_DETALLE_DE_PEDIDO
	{

		[Key]
		[Column(Order = 0)]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		[StringLength(100)]
		public string DS_CLIENTE { get; set; }

		[Key]
		[Column(Order = 3)]
		public int CD_EMPRESA { get; set; }

		[Required]
		[StringLength(55)]
		public string NM_EMPRESA { get; set; }

		[Key]
		[Column(Order = 5)]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Required]
		[StringLength(65)]
		public string DS_PRODUTO { get; set; }

		[Key]
		[Column(Order = 7)]
		public decimal CD_FAIXA { get; set; }

		[Key]
		[Column(Order = 8)]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		public decimal QT_PEDIDO { get; set; }

		public DateTime DT_ADDROW { get; set; }

		public DateTime? DT_UPDROW { get; set; }

		public decimal? QT_LIBERADO { get; set; }

		public decimal? QT_ANULADO { get; set; }

		[Key]
		[Column(Order = 14)]
		[StringLength(1)]
		public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

		[Key]
		[Column(Order = 15)]
		public long NU_DET_PED_SAI_ATRIB { get; set; }

	}
}
