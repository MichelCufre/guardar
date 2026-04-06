using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

	[Table("V_CONT_SIN_EMBARCAR_PED_SINCAM")]
	public partial class V_CONT_SIN_EMBARCAR_PED_SINCAM
	{

		[Key]
		[Column(Order = 0)]
		public int NU_CONTENEDOR { get; set; }

		[Key]
		[Column(Order = 1)]
		public int NU_PREPARACION { get; set; }

		[Key]
		[Column(Order = 2)]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

		[StringLength(40)]
		public string CD_ENDERECO { get; set; }

		[StringLength(1)]
		public string ID_CONTENEDOR_EMPAQUE { get; set; }

		[Key]
		[Column(Order = 5)]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		[Key]
		[Column(Order = 6)]
		public int CD_EMPRESA { get; set; }

		public int? CD_ROTA { get; set; }

		[StringLength(10)]
		public string CD_ZONA { get; set; }

		[StringLength(50)]
		[Column]
		public string ID_EXTERNO_CONTENEDOR { get; set; }
	}
}
