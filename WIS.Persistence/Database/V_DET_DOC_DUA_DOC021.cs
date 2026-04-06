namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_DET_DOC_DUA_DOC021")]
    public partial class V_DET_DOC_DUA_DOC021
    {
        [StringLength(6)]
        [Column]
        public string TP_DUA { get; set; }

        [StringLength(25)]
        [Column]
        public string NU_DUA { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string NU_DOCUMENTO_INGRESO_DUA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_DOCUMENTO_INGRESO_DUA { get; set; }

        [StringLength(6)]
        [Column]
        public string ID_ESTADO { get; set; }

        public decimal? VL_ARBITRAJE { get; set; }

        public int? NU_AGENDA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_MANUAL { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 4)]
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

        public decimal? QT_EXISTENCIA { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(20)]
        public string CD_NAM { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_NAM { get; set; }
    }
}
