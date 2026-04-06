namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_DET_DOCUMENTO_EGRESO")]
    public partial class V_DET_DOCUMENTO_EGRESO
    {
		[Key]
		[Column(Order = 0)]
		public int? NU_SECUENCIA { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(6)]
        public string TP_DOCUMENTO_EGRESO { get; set; }

		[Key]
		[Column(Order = 2)]
		[StringLength(8)]
        public string NU_DOCUMENTO_EGRESO { get; set; }

        [StringLength(6)]
        public string TP_DOCUMENTO_INGRESO { get; set; }

        [StringLength(10)]
        public string NU_DOCUMENTO_INGRESO { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_NAM { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_DUA_INGRESO { get; set; }

        [StringLength(25)]
        [Column]
        public string NU_DUA_INGRESO { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        public decimal? VL_FOB_INGRESO { get; set; }

        public decimal? VL_CIF_INGRESO { get; set; }

        public decimal? QT_EGRESO { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

	}
}
