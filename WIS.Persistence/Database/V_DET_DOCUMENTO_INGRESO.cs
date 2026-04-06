namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_DET_DOCUMENTO_INGRESO")]
    public partial class V_DET_DOCUMENTO_INGRESO
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Column(Order = 7)]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_NAM { get; set; }

        [Key]
        [Column(Order = 1)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO_INGRESO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_DISPONIBLE { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_DISPONIBLE { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_DESAFECTADA { get; set; }

        public decimal? QT_DESCARGADA { get; set; }

        public decimal? QT_INGRESADA { get; set; }

        public decimal? QT_RESERVADA { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        public decimal? VL_CIF { get; set; }

        [StringLength(64)]
        [Column]
        public string VL_DATO_AUDITORIA { get; set; }

        public decimal? VL_MERCADERIA { get; set; }

        public decimal? QT_DISPONIBLE { get; set; }
    }
}
