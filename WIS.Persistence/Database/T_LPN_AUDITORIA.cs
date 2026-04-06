using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

	[Table("T_LPN_AUDITORIA")]
	public partial class T_LPN_AUDITORIA
	{

		[Key]
		[Column(Order = 0)]
		public long NU_AUDITORIA { get; set; }

		public int? ID_LPN_DET { get; set; }

		public long NU_LPN { get; set; }

		[Required]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		public decimal CD_FAIXA { get; set; }

		public int CD_EMPRESA { get; set; }

		[Required]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		public long? NU_TRANSACCION { get; set; }

		public decimal QT_ESTOQUE { get; set; }

		public decimal? QT_AUDITADA { get; set; }

		public decimal? QT_DIFERENCIA { get; set; }

		[StringLength(6)]
		public string ID_ESTADO { get; set; }

		public DateTime? DT_ADDROW { get; set; }

		public DateTime? DT_UPDROW { get; set; }

		public int? CD_FUNC_ADDROW { get; set; }

		public int? CD_FUNC_UPDROW { get; set; }

		public int? CD_FUNC_UPDROW_ESTADO { get; set; }

		[StringLength(6)]
		public string ID_NIVEL { get; set; }
		public DateTime? DT_FABRICACAO { get; set; }
		public long? NU_AUDITORIA_AGRUPADOR { get; set; }

		public long? NU_TRANSACCION_DELETE { get; set; }

	}
}