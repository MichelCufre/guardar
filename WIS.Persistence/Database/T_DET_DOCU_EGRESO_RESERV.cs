using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_DET_DOCU_EGRESO_RESERV")]
    public partial class T_DET_DOCU_EGRESO_RESERV
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
        public int NU_SECUENCIA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(10)]
        public string NU_DOCUMENTO_INGRESO { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(6)]
        public string TP_DOCUMENTO_INGRESO { get; set; }

        [Key]
        [Column(Order = 5)]
        public int NU_PREPARACION { get; set; }

        [Key]
        [Column(Order = 6)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 8)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 9)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR_PICKING_DET { get; set; }

        [Key]
        [Column(Order = 10)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_DESAFECTADA { get; set; }

        public T_DET_DOCUMENTO_EGRESO T_DET_DOCUMENTO_EGRESO { get; set; }
    }
}
