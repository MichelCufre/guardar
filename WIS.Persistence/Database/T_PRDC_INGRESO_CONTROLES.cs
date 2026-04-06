using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
	[Table("T_PRDC_INGRESO_CONTROLES")]
	public class T_PRDC_INGRESO_CONTROLES
	{
		[Key]
		public long? NU_PRDC_DET_TEORICO { get; set; }

		public string NU_PRDC_INGRESO { get; set; }

		[Key]
		public int? CD_CONTROL { get; set; }

		public int? NU_CTR_CALIDAD_PENDIENTE { get; set; }
	}
}
