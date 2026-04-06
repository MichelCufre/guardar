using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
	[Table("T_PRDC_INGRESO_INSTRUCCION")]
	public class T_PRDC_INGRESO_INSTRUCCION
	{
		[Key]
		public int? NU_PRDC_INGRESO_INSTRUCCION { get; set; }

		public string NU_PRDC_INGRESO { get; set; }

		public byte[] VL_INSTRUCCIONES { get; set; }

		public string TP_VALOR { get; set; }
	}
}
