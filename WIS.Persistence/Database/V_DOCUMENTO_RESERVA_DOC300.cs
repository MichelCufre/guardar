namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_DOCUMENTO_RESERVA_DOC300")]
    public partial class V_DOCUMENTO_RESERVA_DOC300
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
        public int NU_PREPARACION { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR_PICKING_DET { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Column]
        [StringLength(1)]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        public decimal? QT_PRODUTO { get; set; }
        public decimal? QT_PREPARADO { get; set; }

        public decimal? QT_ANULAR { get; set; }

        [StringLength(64)]
        [Column]
        public string VL_DATO_AUDITORIA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
    }
}
