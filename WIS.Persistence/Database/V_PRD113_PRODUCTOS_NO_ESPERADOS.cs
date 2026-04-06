using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
	[Table("V_PRD113_PRODUCTOS_NO_ESPERADOS")]
	public partial class V_PRD113_PRODUCTOS_NO_ESPERADOS
	{
		[Key]
		[Column(Order = 0)]
		public long NU_PRDC_SALIDA_REAL { get; set; }

		public string NU_PRDC_INGRESO { get; set; }

		public long? NU_ORDEN { get; set; }

		public string CD_ENDERECO_ENTRADA { get; set; }

		public string CD_ENDERECO_PRODUCCION { get; set; }

		public string CD_ENDERECO_SALIDA { get; set; }

		public string CD_PRDC_LINEA { get; set; }

		public string CD_PRODUTO { get; set; }

		public string DS_PRODUTO { get; set; }

		public int CD_EMPRESA { get; set; }

		public string NU_IDENTIFICADOR { get; set; }

		public decimal CD_FAIXA { get; set; }

		public string DS_ANEXO1 { get; set; }

		public string DS_ANEXO2 { get; set; }

		public string DS_ANEXO3 { get; set; }

		public string DS_ANEXO4 { get; set; }

		public decimal? QT_PRODUCIDO { get; set; }

		public string NU_PREDIO { get; set; }

		public DateTime? DT_ADDROW { get; set; }

		public DateTime? DT_VENCIMIENTO { get; set; }
	}
}
