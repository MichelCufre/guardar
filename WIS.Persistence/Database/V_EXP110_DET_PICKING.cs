using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

	[Table("V_EXP110_DET_PICKING")]
	public partial class V_EXP110_DET_PICKING
	{
		[Key]
		[Column(Order = 0)]
		[Required]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		[StringLength(100)]
		public string DS_CLIENTE { get; set; }

		[Key]
		[Column(Order = 2)]
		public int CD_EMPRESA { get; set; }

		[Key]
		[Column(Order = 3)]
		[Required]
		[StringLength(40)]
		public string CD_ENDERECO { get; set; }

		[Key]
		[Column(Order = 4)]
		public decimal CD_FAIXA { get; set; }

		public int? CD_FORNECEDOR { get; set; }

		public int? CD_FUNCIONARIO { get; set; }

		public int? CD_FUNC_PICKEO { get; set; }

		[Key]
		[Column(Order = 8)]
		[Required]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Required]
		[StringLength(65)]
		public string DS_PRODUTO { get; set; }

		public DateTime? DT_ADDROW { get; set; }

		public DateTime? DT_FABRICACAO_PICKEO { get; set; }

		public DateTime? DT_PICKEO { get; set; }

		public DateTime? DT_SEPARACION { get; set; }

		public DateTime? DT_UPDROW { get; set; }

		[StringLength(1)]
		public string FL_CANCELADO { get; set; }

		[StringLength(1)]
		public string FL_ERROR_CONTROL { get; set; }

		[StringLength(1)]
		public string ID_AGRUPACION { get; set; }

		[StringLength(1)]
		public string ID_AREA_AVERIA { get; set; }

		[StringLength(1)]
		public string ID_AVERIA_PICKEO { get; set; }

		[StringLength(1)]
		public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

		[StringLength(20)]
		public string ND_ESTADO { get; set; }

		public long? NU_CARGA { get; set; }

		public int? NU_CONTENEDOR { get; set; }

		public int? NU_CONTENEDOR_PICKEO { get; set; }

		public int? NU_CONTENEDOR_SYS { get; set; }

		[Key]
		[Column(Order = 26)]
		[Required]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		[Key]
		[Column(Order = 27)]
		[Required]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

		[Key]
		[Column(Order = 28)]
		public int NU_PREPARACION { get; set; }

		[Key]
		[Column(Order = 29)]
		public int NU_SEQ_PREPARACION { get; set; }

		public long? NU_TRANSACCION { get; set; }

		public decimal? QT_CONTROL { get; set; }

		public decimal? QT_CONTROLADO { get; set; }

		public decimal? QT_PICKEO { get; set; }

		public decimal? QT_PREPARADO { get; set; }

		public decimal? QT_PRODUTO { get; set; }

		[StringLength(40)]
		public string VL_ESTADO_REFERENCIA { get; set; }

		[StringLength(30)]
		public string DS_FUNC_PICKEO { get; set; }

		[StringLength(100)]
		public string VL_DISPLAY { get; set; }

		public decimal? PS_LIQUIDO_TOTAL { get; set; }

		public decimal? PS_BRUTO_TOTAL { get; set; }

        [StringLength(200)]
        public string VL_COMPARTE_CONTENEDOR_PICKING { get; set; }

        [StringLength(200)]
        public string VL_COMPARTE_CONTENEDOR_ENTREGA { get; set; }

    }
}
