using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

	[Table("V_CONTENEDOR_PEDIDO_CONSULTA")]
	public partial class V_CONTENEDOR_PEDIDO_CONSULTA
	{

		[Key]
		[Column(Order = 0)]
		public int NU_CONTENEDOR { get; set; }

		[Key]
		[Column(Order = 1)]
		public int NU_PREPARACION { get; set; }

		public int? CD_CAMION { get; set; }

		[Key]
		[Column(Order = 3)]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		[Key]
		[Column(Order = 4)]
		public int CD_EMPRESA { get; set; }

		[Key]
		[Column(Order = 5)]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

		public short? CD_SITUACAO { get; set; }

		[StringLength(1)]
		public string ID_CONTENEDOR_EMPAQUE { get; set; }

		[StringLength(40)]
		public string CD_ENDERECO { get; set; }

		public int? QT_BULTO { get; set; }

		[StringLength(9)]
		public string AUX_ESTADO { get; set; }

		[StringLength(50)]
		[Column]
		public string ID_EXTERNO_CONTENEDOR { get; set; }
	}
}
