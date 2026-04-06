
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;


namespace WIS.Persistence.Database
{

	[Table("V_PRE170_ANALISIS_RECHAZO_LPN")]
	public partial class V_PRE170_ANALISIS_RECHAZO_LPN
	{
        [Key]
        public long NU_ANALISIS_RECHAZO { get; set; }
        [Key]
		public int NU_PREPARACION { get; set; }

		[Key]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Required]
		[StringLength(3)]
		public string DS_PRODUTO { get; set; }

		[Key]
		public decimal CD_FAIXA { get; set; }

		[Key]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		[Key]
		public int CD_EMPRESA { get; set; }


		[Key]
		[StringLength(1)]
		public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        [Key]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }


        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }
        public decimal? QT_PEDIDO { get; set; }

		public decimal? QT_RECHAZADO { get; set; }

		public decimal? QT_ESTOQUE { get; set; }

		public decimal? QT_RESERVA_SAIDA { get; set; }

		public decimal? QT_SALDO_DOC { get; set; }

		public long? NU_LPN { get; set; }

		[StringLength(100)]
		public string DS_SITUACION_LPN { get; set; }

		public decimal? QT_DISPONIBLE_LPN { get; set; }



		[StringLength(100)]
		public string DS_CLIENTE { get; set; }

		[StringLength(50)]
		public string ID_LPN_EXTERNO { get; set; }

		[StringLength(10)]
		public string TP_LPN_TIPO { get; set; }

		[StringLength(40)]
		public string CD_ENDERECO { get; set; }

		public decimal? QT_RESERVA_ATRIBUTOS { get; set; }
		public long? NU_DET_PED_SAI_ATRIB { get; set; }

	}
}
