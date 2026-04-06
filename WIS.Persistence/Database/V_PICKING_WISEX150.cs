using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

	[Table("V_PICKING_WISEX150")]
	public partial class V_PICKING_WISEX150
	{

		[Key]
		[Column(Order = 0)]
		public int NU_PREPARACION { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Required]
		[StringLength(65)]
		public string DS_PRODUTO { get; set; }

		[Key]
		[Column(Order = 3)]
		public decimal CD_FAIXA { get; set; }

		[Key]
		[Column(Order = 4)]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		[Key]
		[Column(Order = 5)]
		public int CD_EMPRESA { get; set; }

		[Required]
		[StringLength(55)]
		public string NM_EMPRESA { get; set; }

		[Key]
		[Column(Order = 7)]
		[StringLength(40)]
		public string CD_ENDERECO { get; set; }

		[Key]
		[Column(Order = 8)]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

		[Key]
		[Column(Order = 9)]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		[StringLength(100)]
		public string DS_CLIENTE { get; set; }

		[Key]
		[Column(Order = 11)]
		public int NU_SEQ_PREPARACION { get; set; }

		[StringLength(1)]
		public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

		public long? NU_CARGA { get; set; }

		[StringLength(1)]
		public string ID_AGRUPACION { get; set; }

		public decimal? QT_PRODUTO { get; set; }

		public decimal? QT_PREPARADO { get; set; }

		public int? NU_CONTENEDOR { get; set; }

		public int? CD_FUNCIONARIO { get; set; }

		public int? NU_CONTENEDOR_SYS { get; set; }

		public decimal? QT_PICKEO { get; set; }

		public DateTime? DT_PICKEO { get; set; }

		public int? NU_CONTENEDOR_PICKEO { get; set; }

		public int? CD_FUNC_PICKEO { get; set; }

		public DateTime? DT_ADDROW { get; set; }

		public DateTime? DT_UPDROW { get; set; }

		public DateTime? DT_FABRICACAO_PICKEO { get; set; }

		[StringLength(1)]
		public string ID_AVERIA_PICKEO { get; set; }

		public int? CD_FORNECEDOR { get; set; }

		[StringLength(1)]
		public string ID_AREA_AVERIA { get; set; }

		[StringLength(1)]
		public string FL_CANCELADO { get; set; }

		public decimal? QT_CONTROLADO { get; set; }

		public decimal? QT_CONTROL { get; set; }

		[StringLength(1)]
		public string FL_ERROR_CONTROL { get; set; }

		public long? NU_TRANSACCION { get; set; }

		[StringLength(20)]
		public string ND_ESTADO { get; set; }

		[StringLength(40)]
		public string VL_ESTADO_REFERENCIA { get; set; }

		public DateTime? DT_SEPARACION { get; set; }

	}
}
