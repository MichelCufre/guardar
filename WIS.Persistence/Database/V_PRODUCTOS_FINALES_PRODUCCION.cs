using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
	[Table("V_PRODUCTOS_FINALES_PRODUCCION")]
	public class V_PRODUCTOS_FINALES_PRODUCCION
	{
		[Key]
		public string NU_PRDC_INGRESO { get; set; }

		[Key]
		public int CD_EMPRESA { get; set; }

		[Key]
		public string CD_PRODUTO { get; set; }

		public string DS_PRODUTO { get; set; }

		public decimal? QT_TEORICO { get; set; }

		public decimal? QT_PRODUCIDO { get; set; }

		public string FL_DIFERENCIA { get; set; }

		public string FL_PRD_NO_ESPERADO { get; set; }
	}
}
