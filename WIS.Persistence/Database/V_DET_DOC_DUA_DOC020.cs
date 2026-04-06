namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_DET_DOC_DUA_DOC020")]
    public partial class V_DET_DOC_DUA_DOC020
    {
        [StringLength(6)]
        [Column]
        public string TP_DUA { get; set; }

        [StringLength(25)]
        [Column]
        public string NU_DUA { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_DOCUMENTO_INGRESO_DUA { get; set; }

        [StringLength(62)]
        public string NU_DOCUMENTO_INGRESO_DUA_FORM { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_DOCUMENTO_INGRESO_DUA { get; set; }

        [StringLength(6)]
        [Column]
        public string ID_ESTADO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ESTADO { get; set; }

        public decimal? VL_ARBITRAJE { get; set; }

        [StringLength(15)]
        [Column]
        public string CD_MONEDA { get; set; }

        public int? NU_AGENDA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_MANUAL { get; set; }

        [StringLength(20)]
        public string NU_AGRUPADOR { get; set; }

        [StringLength(3)]
        public string TP_AGRUPADOR { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(20)]
        [Column]
        public string NCM { get; set; }

        [Key]
        [Column(Order = 1)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        [StringLength(62)]
        public string NU_DOCUMENTO_FORMAT { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        public decimal? QT_INGRESADA { get; set; }

        public decimal? QT_RESERVADA { get; set; }

        public decimal? QT_DESAFECTADA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_DISPONIBLE { get; set; }

        public DateTime? DT_DISPONIBLE { get; set; }

        public decimal? VL_MERCADERIA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public decimal? QT_MERCADERIA { get; set; }

        public decimal? QT_DISPONIBLE { get; set; }

        public decimal? VL_EXISTENCIA_USD { get; set; }
    }
}
