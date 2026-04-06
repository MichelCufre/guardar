using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

	[Table("V_PEDIDO_PREPARADO_COMPLETO")]
	public partial class V_PEDIDO_PREPARADO_COMPLETO
	{

		[Key]
		[Column(Order = 0)]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

		[Key]
		[Column(Order = 2)]
		[StringLength(6)]
		public string TP_PEDIDO { get; set; }

		public int? CD_ROTA { get; set; }

		public DateTime? DT_ENTREGA { get; set; }

		[StringLength(10)]
		public string CD_ZONA { get; set; }

		[StringLength(100)]
		public string DS_CLIENTE { get; set; }

		[Key]
		[Column(Order = 7)]
		public int? CD_EMPRESA { get; set; }

		public long? QT_CONTENEDOR { get; set; }

		[StringLength(4)]
		public string FL_EMPAQUETA_CONTENEDOR { get; set; }

		public int? BANDEJA1 { get; set; }

		public int? BANDEJA2 { get; set; }

		[StringLength(200)]
		public string DS_ANEXO1 { get; set; }

		[StringLength(200)]
		public string DS_ANEXO3 { get; set; }

		[StringLength(6)]
		public string CD_CONDICION_LIBERACION { get; set; }

		public int? CD_TRANSPORTADORA { get; set; }

		public int? NU_ULT_PREPARACION { get; set; }

		public int? NU_ORDEN { get; set; }

		[StringLength(50)]
		public string AUX_OBSERVACION { get; set; }

		[StringLength(2)]
		public string NU_PREDIO { get; set; }

	}
}