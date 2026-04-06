namespace WIS.Persistence.Database
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("V_PRD113_STOCK_INSUMOS")]
	public partial class V_PRD113_STOCK_INSUMOS
	{
		[Key]
		public string CD_PRODUTO { get; set; }

		public string DS_PRODUTO { get; set; }

		public string CD_UNIDADE_MEDIDA { get; set; }

		[Key]
		public int? CD_EMPRESA { get; set; }

		public decimal CD_FAIXA { get; set; }

		[Key]
		public string NU_IDENTIFICADOR { get; set; }

		public decimal? QT_NOTIFICADO { get; set; }

		public decimal? QT_REAL { get; set; }

		public decimal? QT_REAL_ORIGINAL { get; set; }

		public decimal? QT_DESAFECTADA { get; set; }

		public string DS_ANEXO1 { get; set; }

		public string DS_ANEXO2 { get; set; }

		public string DS_ANEXO3 { get; set; }

		public string DS_ANEXO4 { get; set; }

		public long? NU_ORDEN { get; set; }

		[Key]
		public string NU_PRDC_INGRESO { get; set; }

		[Key]
		public long NU_PRDC_INGRESO_REAL { get; set; }

		public string DS_REFERENCIA { get; set; }

		[Key]
		public string FL_CONSUMIBLE { get; set; }

	}
}
