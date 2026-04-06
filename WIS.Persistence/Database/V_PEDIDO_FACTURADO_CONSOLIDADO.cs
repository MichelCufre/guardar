using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

	[Table("V_PEDIDO_FACTURADO_CONSOLIDADO")]
	public partial class V_PEDIDO_FACTURADO_CONSOLIDADO
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
		public int? CD_EMPRESA { get; set; }

		[Key]
		[Column(Order = 3)]
		public int? CD_CAMION { get; set; }

		public DateTime? DT_FACTURACION { get; set; }

		public long? NU_INTERFAZ_EJECUCION { get; set; }

		public short? CD_SITUACAO { get; set; }

		public DateTime? DT_UPDROW { get; set; }

	}
}
