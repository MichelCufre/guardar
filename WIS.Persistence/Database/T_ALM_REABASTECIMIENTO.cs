
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

	[Table("T_ALM_REABASTECIMIENTO")]
	public partial class T_ALM_REABASTECIMIENTO
	{

		[Key]
		[Column(Order = 0)]
		public decimal NU_ALM_REABASTECIMIENTO { get; set; }

		public int NU_ETIQUETA_LOTE { get; set; }

		[Required]
		[StringLength(40)]
		public string CD_REFERENCIA { get; set; }

		[Required]
		[StringLength(10)]
		public string NU_PREDIO { get; set; }

		[Required]
		[StringLength(40)]
		public string CD_ENDERECO_SUGERIDO { get; set; }

		[Required]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		[Required]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		public decimal CD_FAIXA { get; set; }

		public int CD_EMPRESA { get; set; }

		public DateTime? DT_ADDROW { get; set; }

		public DateTime? DT_UPDROW { get; set; }

		[Required]
		[StringLength(1)]
		public string CD_ESTADO { get; set; }

		public int? CD_FUNCIONARIO { get; set; }

		public long? NU_TRANSACCION { get; set; }

		public decimal? QT_PRODUTO { get; set; }

		public decimal? QT_AUDITADA { get; set; }

		public decimal? QT_CLASIFICADA { get; set; }

		public DateTime? DT_FABRICACAO { get; set; }

		public string FL_IGNORAR_STOCK { get; set; }
	}
}