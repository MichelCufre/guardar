using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

	[Table("V_EXP110_CONT_PREP")]
	public partial class V_EXP110_CONT_PREP
	{

		[Key]
		[Column(Order = 0)]
		public int NU_CONTENEDOR { get; set; }

		[Key]
		[Column(Order = 1)]
		public int NU_PREPARACION { get; set; }

		[Key]
		[Column(Order = 2)]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		[Key]
		[Column(Order = 3)]
		public int CD_EMPRESA { get; set; }

		[Key]
		[Column(Order = 4)]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

		public decimal? QT_PREPARADO { get; set; }

		public decimal? PS_BRUTO { get; set; }

		public decimal? PS_NETO { get; set; }

		[StringLength(50)]
		[Column]
		public string ID_EXTERNO_CONTENEDOR { get; set; }
	}
}
