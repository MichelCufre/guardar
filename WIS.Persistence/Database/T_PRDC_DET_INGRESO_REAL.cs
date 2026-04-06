using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
	[Table("T_PRDC_DET_INGRESO_REAL")]
	public class T_PRDC_DET_INGRESO_REAL
	{
		[Key]
		public long NU_PRDC_INGRESO_REAL { get; set; }

		public string NU_PRDC_INGRESO { get; set; }

		public string CD_PRODUTO { get; set; }

		public int? CD_EMPRESA { get; set; }

		public decimal? CD_FAIXA { get; set; }

		public decimal? QT_REAL { get; set; }

		public decimal? QT_REAL_ORIGINAL { get; set; }

		public decimal? QT_NOTIFICADO { get; set; }

		public decimal? QT_DESAFECTADA { get; set; }

		public decimal? QT_MERMA { get; set; }

		public string NU_IDENTIFICADOR { get; set; }

		public long? NU_TRANSACCION { get; set; }

		public DateTime? DT_ADDROW { get; set; }

		public long? NU_ORDEN { get; set; }

		public string DS_ANEXO1 { get; set; }

		public string DS_ANEXO2 { get; set; }

		public string DS_ANEXO3 { get; set; }

		public string DS_ANEXO4 { get; set; }

		public string DS_REFERENCIA { get; set; }

		public string ND_ESTADO { get; set; }
	}
}
