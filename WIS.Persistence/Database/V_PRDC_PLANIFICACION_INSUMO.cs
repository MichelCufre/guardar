using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
	[Table("V_PRDC_PLANIFICACION_INSUMO")]
	public partial class V_PRDC_PLANIFICACION_INSUMO
	{
        [Key]
        public string NU_PRDC_INGRESO { get; set; }

        [Key]
		public string CD_ENDERECO { get; set; }

		[Key]
		public int CD_EMPRESA { get; set; }

		[Key]
		public decimal CD_FAIXA { get; set; }

		[Key]
		public string CD_PRODUTO { get; set; }

		public string DS_PRODUTO { get; set; }

		[Key]
		public string NU_IDENTIFICADOR { get; set; }

		public string NU_IDENTIFICADOR_TEORICO { get; set; }

		public decimal? QT_DISPONIBLE { get; set; }

		public decimal? QT_RESERVA { get; set; }

        public decimal? QT_PENDIENTE { get; set; }

        public decimal? QT_PEDIR { get; set; }

		public string REQUERIDO { get; set; }

		public decimal? QT_RESERVAR { get; set; }

        public decimal? QT_PEDIR_SUGERIDA { get; set; }

        public decimal? QT_TEORICO { get; set; }
    }
}
