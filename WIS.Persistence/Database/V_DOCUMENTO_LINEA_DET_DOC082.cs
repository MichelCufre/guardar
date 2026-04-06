namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_DOCUMENTO_LINEA_DET_DOC082")]
    public partial class V_DOCUMENTO_LINEA_DET_DOC082
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_DOCUMENTO_ASOCIADO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_DOCUMENTO_ASOCIADO { get; set; }

        public decimal? QT_ASOCIADA { get; set; }

        [StringLength(6)]
        [Column]
        public string ID_ESTADO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ESTADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public decimal? VL_MERCADERIA { get; set; }

        public decimal? VL_CIF { get; set; }
    }
}
