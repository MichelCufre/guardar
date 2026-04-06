using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{
	[Table("T_PRDC_DET_INGRESO_TEORICO")]
	public class T_PRDC_DET_INGRESO_TEORICO
	{
		[Key]
		public long? NU_PRDC_DET_TEORICO { get; set; }

		public string NU_PRDC_INGRESO { get; set; }

		public string TP_REGISTRO { get; set; }

		public string CD_PRODUTO { get; set; }

		public int? CD_EMPRESA { get; set; }

		public decimal? CD_FAIXA { get; set; }

		public decimal? QT_TEORICO { get; set; }

		public decimal? QT_PEDIDO_GENERADO { get; set; }

		public decimal? QT_ABASTECIDO { get; set; }

		public decimal? QT_CONSUMIDO { get; set; }

		public string NU_IDENTIFICADOR { get; set; }

		public long? NU_TRANSACCION { get; set; }

		public DateTime? DT_ADDROW { get; set; }

		public string DS_ANEXO1 { get; set; }

		public string DS_ANEXO2 { get; set; }

		public string DS_ANEXO3 { get; set; }

		public string DS_ANEXO4 { get; set; }
	}
}
