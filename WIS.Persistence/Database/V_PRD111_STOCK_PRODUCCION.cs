using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
	[Table("V_PRD111_STOCK_PRODUCCION")]
	public partial class V_PRD111_STOCK_PRODUCCION
    {
        [StringLength(10)]
        public string CD_PRDC_LINEA { get; set; }

        [StringLength(2000)]
        public string DS_PRDC_LINEA { get; set; }

        [Key]		
		[StringLength(40)]
		public string CD_ENDERECO { get; set; }

		[Key]
		public int CD_EMPRESA { get; set; }

		[StringLength(55)]
		public string NM_EMPRESA { get; set; }

		[Key]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[StringLength(65)]
		public string DS_PRODUTO { get; set; }

		[Key]
		public decimal CD_FAIXA { get; set; }

		[Key]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		public DateTime? DT_FABRICACAO { get; set; }

		public decimal? QT_ESTOQUE { get; set; }

		public decimal? QT_RESERVA_SAIDA { get; set; }

		public decimal? QT_DISPONIBLE { get; set; }

		public decimal? QT_AJUSTAR { get; set; }

		public DateTime? DT_UPDROW { get; set; }

	}
}
