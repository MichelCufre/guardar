using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

	[Table("V_CONTENEDOR_WISEX150")]
	public partial class V_CONTENEDOR_WISEX150
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
		public int NU_CONTENEDOR { get; set; }

		[Key]
		[Column(Order = 4)]
		public int NU_PREPARACION { get; set; }

		[StringLength(40)]
		public string CD_ENDERECO { get; set; }

		public int? CD_FUNC_PICKEO { get; set; }

		[StringLength(1)]
		public string ID_CONTENEDOR_EMPAQUE { get; set; }

		public int? QT_PRODUCTOS_DIFERENTES { get; set; }

		public DateTime? DT_ULTIMO_PICKEO { get; set; }

		[Required]
		[StringLength(30)]
		public string NM_FUNCIONARIO { get; set; }

		[StringLength(8)]
		public string DS_ESTADO { get; set; }

		[StringLength(50)]
		[Column]
		public string ID_EXTERNO_CONTENEDOR { get; set; }
	}
}