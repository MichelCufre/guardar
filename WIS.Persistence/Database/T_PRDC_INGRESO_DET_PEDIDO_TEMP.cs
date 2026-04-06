using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
	[Table("T_PRDC_INGRESO_DET_PEDIDO_TEMP")]
	public class T_PRDC_INGRESO_DET_PEDIDO_TEMP
	{
	
		public long? NU_PRDC_DET_TEORICO { get; set; }

		[Key]
		public string NU_PRDC_INGRESO { get; set; }

		[Key]
		public string CD_PRODUTO { get; set; }

		[Key]
		public string NU_IDENTIFICADOR { get; set; }

		[Key]
		public int CD_EMPRESA { get; set; }

		public decimal CD_FAIXA { get; set; }

		public decimal QT_PEDIR { get; set; }

		public long? NU_TRANSACCION { get; set; }

		public long? NU_TRANSACCION_DELETE { get; set; }

		public DateTime? DT_ADDROW { get; set; }
	}
}
