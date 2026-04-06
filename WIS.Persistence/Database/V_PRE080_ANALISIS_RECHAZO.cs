
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
	[Table("V_PRE080_ANALISIS_RECHAZO")]
	public partial class V_PRE080_ANALISIS_RECHAZO
	{
		[Key]
		[Column(Order = 0)]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		[StringLength(100)]
		public string DS_CLIENTE { get; set; }

		[Key]
		[Column(Order = 3)]
		public int NU_PREPARACION { get; set; }

		[Key]
		[Column(Order = 4)]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Required]
		[StringLength(65)]
		public string DS_PRODUTO { get; set; }

		public decimal CD_FAIXA { get; set; }

		[Key]
		[Column(Order = 7)]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		[Key]
		[Column(Order = 8)]
		public int CD_EMPRESA { get; set; }

		[Required]
		[StringLength(55)]
		public string NM_EMPRESA { get; set; }

		[Key]
		[Column(Order = 10)]
		[StringLength(1)]
		public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

		public decimal? QT_RECHAZADO { get; set; }

		public decimal? QT_ESTOQUE { get; set; }

		public decimal? QT_AVERIA { get; set; }

		public decimal? QT_PREPARACION { get; set; }

		public decimal? QT_DIFERENCIA { get; set; }

		public decimal? QT_CTRL_CALIDAD { get; set; }

		public decimal? QT_PENDIENTE_ALMACENAR { get; set; }

		public decimal? QT_TRANSFERENCIA { get; set; }

		public decimal? QT_CONTENEDOR { get; set; }

		public decimal? QT_DUAS { get; set; }

		public decimal? QT_EN_AREA_NO_DISP { get; set; }

		public decimal? QT_VENCIDO { get; set; }

		public decimal? QT_DESPREPARADO { get; set; }

		public decimal? QT_OTRO_PREDIO { get; set; }

		public decimal? QT_ESTOQUE_ALMACEN { get; set; }

		public decimal? QT_ESTOQUE_MENUDENCIA { get; set; }

		public decimal? QT_DUAS_FILTRO { get; set; }

		public decimal? QT_RECHAZADO_PEDIDO { get; set; }

        public decimal? QT_VENCIDO_POR_VENTANA { get; set; }
        public int? QT_DIAS_VALIDADE_LIBERACION { get; set; }
        public string CD_VENTANA_LIBERACION { get; set; }
    }
}
