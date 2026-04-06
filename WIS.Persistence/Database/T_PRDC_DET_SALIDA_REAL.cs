using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
	[Table("T_PRDC_DET_SALIDA_REAL")]
	public class T_PRDC_DET_SALIDA_REAL
	{
		[Key]
		public long NU_PRDC_SALIDA_REAL { get; set; }

		public long? NU_PRDC_DET_TEORICO { get; set; }

		public string NU_PRDC_INGRESO { get; set; }

		public string CD_PRODUTO { get; set; }

		public int? CD_EMPRESA { get; set; }

		public decimal? CD_FAIXA { get; set; }

		public decimal? QT_PRODUCIDO { get; set; }

		public decimal? QT_NOTIFICADO { get; set; }

		public string NU_IDENTIFICADOR { get; set; }

		public string ND_MOTIVO { get; set; }

		public string DS_MOTIVO { get; set; }

		public long? NU_TRANSACCION { get; set; }

		public DateTime? DT_ADDROW { get; set; }

		public DateTime? DT_VENCIMIENTO { get; set; }

		public long? NU_ORDEN { get; set; }

		public string ND_ESTADO { get; set; }

		public string DS_ANEXO1 { get; set; }

		public string DS_ANEXO2 { get; set; }

		public string DS_ANEXO3 { get; set; }

		public string DS_ANEXO4 { get; set; }
	}
}
