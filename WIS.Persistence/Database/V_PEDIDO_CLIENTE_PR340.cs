using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
	[Table("V_PEDIDO_CLIENTE_PR340")]
	public partial class V_PEDIDO_CLIENTE_PR340
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

		public int? CD_ROTA { get; set; }

		public short? CD_SITUACAO { get; set; }

		public DateTime? DT_LIBERAR_DESDE { get; set; }

		public DateTime? DT_LIBERAR_HASTA { get; set; }

		public DateTime? DT_ENTREGA { get; set; }

		[StringLength(1)]
		public string ID_MANUAL { get; set; }

		[StringLength(1)]
		public string ID_AGRUPACION { get; set; }

		public DateTime? DT_EMITIDO { get; set; }

		public DateTime? DT_ADDROW { get; set; }

		public DateTime? DT_UPDROW { get; set; }

		public short? NU_ORDEN_LIBERACION { get; set; }

		public int? NU_ULT_PREPARACION { get; set; }

		public DateTime? DT_ULT_PREPARACION { get; set; }

		[StringLength(1000)]
		public string DS_MEMO { get; set; }

		[StringLength(10)]
		public string NU_PRDC_INGRESO { get; set; }

		public int? NU_PREPARACION_MANUAL { get; set; }

		[StringLength(30)]
		public string CD_ORIGEN { get; set; }

		[StringLength(4000)]
		public string VL_SERIALIZADO_1 { get; set; }

		[Required]
		[StringLength(6)]
		public string TP_PEDIDO { get; set; }

		[StringLength(10)]
		public string CD_ZONA { get; set; }

		[StringLength(400)]
		public string DS_ENDERECO { get; set; }

		[StringLength(6)]
		public string TP_EXPEDICION { get; set; }

		[Required]
		[StringLength(6)]
		public string CD_CONDICION_LIBERACION { get; set; }

		[StringLength(200)]
		public string DS_ANEXO4 { get; set; }

		[StringLength(100)]
		public string DS_CLIENTE { get; set; }

		public int? CD_TRANSPORTADORA { get; set; }

		[Required]
		[StringLength(55)]
		public string NM_EMPRESA { get; set; }

		[StringLength(30)]
		public string DS_ROTA { get; set; }

		[Required]
		[StringLength(30)]
		public string DS_SITUACAO { get; set; }

	}
}
