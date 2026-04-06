namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_AJUSTE_STOCK")]
    public partial class T_AJUSTE_STOCK
    {
        [Required]
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        public int CD_EMPRESA { get; set; }

        public DateTime DT_REALIZADO { get; set; }

        [StringLength(2)]
        [Column]
        public string TP_AJUSTE { get; set; }

        public decimal? QT_MOVIMIENTO { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_MOTIVO { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_DOCUMENTO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_DOCUMENTO { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PROCESAR { get; set; }

        [Required]
        [StringLength(3)]
        [Column]
        public string CD_MOTIVO_AJUSTE { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AJUSTE_STOCK { get; set; }

        public int? NU_LOG_INVENTARIO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public int? CD_FUNC_MOTIVO { get; set; }

        public DateTime? DT_MOTIVO { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_APLICACAO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AREA_AVERIA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        public decimal? NU_INVENTARIO_ENDERECO_DET { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PROCESADO { get; set; }

        public long? NU_INTERFAZ_EJECUCION { get; set; }

        [StringLength(200)]
        public string VL_METADATA { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_SERIALIZADO { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_ATRIBUTOS_LPN { get; set; }
    }
}
