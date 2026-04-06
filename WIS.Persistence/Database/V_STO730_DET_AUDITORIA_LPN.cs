
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    namespace WIS.Persistence.Database
{

	[Table("V_STO730_DET_AUDITORIA_LPN")]
	public partial class V_STO730_DET_AUDITORIA_LPN
	{

		[Key]
		[Column(Order = 0)]
		public long NU_AUDITORIA_AGRUPADOR { get; set; }

		[Key]
		[Column(Order = 1)]
		public long NU_AUDITORIA { get; set; }

		public int? ID_LPN_DET { get; set; }

		public long NU_LPN { get; set; }

		[Required]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Required]
		[StringLength(65)]
		public string DS_PRODUTO { get; set; }

		public decimal CD_FAIXA { get; set; }

		public int CD_EMPRESA { get; set; }

		[Required]
		[StringLength(55)]
		public string NM_EMPRESA { get; set; }

		[Required]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		public long? NU_TRANSACCION { get; set; }

		public decimal QT_ESTOQUE { get; set; }

		public decimal? QT_AUDITADA { get; set; }

		public decimal? QT_DIFERENCIA { get; set; }

		[StringLength(6)]
		public string ID_ESTADO { get; set; }

		[Required]
		[StringLength(100)]
		public string DS_ESTADO { get; set; }

		public DateTime? DT_ADDROW { get; set; }

		public DateTime? DT_UPDROW { get; set; }

		public int? CD_FUNC_ADDROW { get; set; }

		[Required]
		[StringLength(100)]
		public string NM_FUNC_ADDROW { get; set; }

		public int? CD_FUNC_UPDROW { get; set; }

		[StringLength(100)]
		public string NM_FUNC_UPDROW { get; set; }

		public int? CD_FUNC_UPDROW_ESTADO { get; set; }

		[StringLength(100)]
		public string NM_FUNC_UPDROW_ESTADO { get; set; }

		[StringLength(6)]
		public string ID_NIVEL { get; set; }

		[Required]
		[StringLength(100)]
		public string DS_NIVEL { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        [StringLength(50)]
        public string ID_LPN_EXTERNO { get; set; }

        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

    }
}
