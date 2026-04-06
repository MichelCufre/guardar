namespace WIS.Persistence.Database
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("V_PRD112_INSUMOS_SOLICITADOS_EN_COMUN")]
	public partial class V_PRD112_INSUMOS_SOLICITADOS_EN_COMUN
	{
		public string NU_PRDC_INGRESO { get; set; }

		[Key]
		public string CD_PRDC_LINEA { get; set; }

		[Key]
		public string NU_PEDIDO { get; set; }
		
		[Key]
		public string CD_PRODUTO { get; set; }

		[Key]
		public int CD_EMPRESA { get; set; }

		public string CD_CLIENTE { get; set; }

		[Key]
		public decimal CD_FAIXA { get; set; }

		[Key]
		public string NU_IDENTIFICADOR { get; set; }

		public decimal QT_PEDIDO { get; set; }

		public decimal QT_PREPARADO { get; set; }

		public string DS_ESTADO { get; set; }
	}
}
