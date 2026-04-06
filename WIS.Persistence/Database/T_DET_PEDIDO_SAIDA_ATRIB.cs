namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DET_PEDIDO_SAIDA_ATRIB")]
    public partial class T_DET_PEDIDO_SAIDA_ATRIB
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

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

        [Key]
        [Column(Order = 6)]
        [StringLength(1)]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 7)]
        public long NU_DET_PED_SAI_ATRIB { get; set; }

        public decimal QT_PEDIDO { get; set; }

        public decimal? QT_LIBERADO { get; set; }

        public decimal? QT_ANULADO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
