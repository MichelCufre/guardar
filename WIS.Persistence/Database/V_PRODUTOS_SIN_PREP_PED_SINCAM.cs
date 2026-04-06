using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

	[Table("V_PRODUTOS_SIN_PREP_PED_SINCAM")]
	public partial class V_PRODUTOS_SIN_PREP_PED_SINCAM
	{

		[Key]
		[Column(Order = 0)]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		[Key]
		[Column(Order = 2)]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

		[Key]
		[Column(Order = 3)]
		public decimal CD_FAIXA { get; set; }

		[Key]
		[Column(Order = 4)]
		public int CD_EMPRESA { get; set; }

		[Key]
		[Column(Order = 5)]
		public int NU_PREPARACION { get; set; }

		public long? NU_CARGA { get; set; }

		[Key]
		[Column(Order = 7)]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		public decimal? QT_PRODUTO { get; set; }

		public int? CD_ROTA { get; set; }

		[StringLength(10)]
		public string CD_ZONA { get; set; }

		[Required]
		[StringLength(65)]
		public string DS_PRODUTO { get; set; }

	}
}
