using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
	public class V_PRD110_DETALLES_INGRESO
	{
		[Key]
		[Column(Order = 0)]
		public long? NU_PRDC_DET_TEORICO { get; set; }

		public string NU_PRDC_INGRESO { get; set; }

		public string CD_PRODUTO { get; set; }

		public string DS_PRODUTO { get; set; }

		public string NU_IDENTIFICADOR { get; set; }

		public decimal QT_TEORICO { get; set; }

		public int? CD_EMPRESA { get; set; }

		public decimal? CD_FAIXA { get; set; }

		public string TP_REGISTRO { get; set; }
	}
}
