using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
	[Table("V_PRD113_PRODUCTOS_ESPERADOS")]
	public class V_PRD113_PRODUCTOS_ESPERADOS
	{
		[Key]
		public long NU_PRDC_DET_TEORICO { get; set; }

		public string NU_PRDC_INGRESO { get; set; }

		public long? NU_ORDEN { get; set; }

		public string CD_ENDERECO_ENTRADA { get; set; }

		public string CD_ENDERECO_PRODUCCION { get; set; }

		public string CD_ENDERECO_SALIDA { get; set; }

		public string CD_PRDC_LINEA { get; set; }

        [Key]
        public string CD_PRODUTO { get; set; }

		public string DS_PRODUTO { get; set; }

        [Key]
		public int? CD_EMPRESA { get; set; }

        [Key]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        public decimal? CD_FAIXA { get; set; }

        public string DS_ANEXO1 { get; set; }

		public string DS_ANEXO2 { get; set; }

		public string DS_ANEXO3 { get; set; }

		public string DS_ANEXO4 { get; set; }

		public decimal? QT_PRODUCIDO { get; set; }

		public string NU_PREDIO { get; set; }

		public DateTime? DT_ADDROW { get; set; }

		public DateTime? DT_VENCIMIENTO { get; set; }

		public string FL_DIFERENCIA { get; set; }
	}
}
