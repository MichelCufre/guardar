namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_INT105_ESTAN_AJUSTE_STOCK")]
    public partial class V_INT105_ESTAN_AJUSTE_STOCK
    {
        [StringLength(30)]
        [Column]
        public string CD_APLICACAO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_FAIXA { get; set; }

        [StringLength(7)]
        [Column]
        public string CD_FUNCIONARIO { get; set; }

        [StringLength(7)]
        [Column]
        public string CD_FUNC_MOTIVO { get; set; }

        [StringLength(3)]
        [Column]
        public string CD_MOTIVO_AJUSTE { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_MOTIVO { get; set; }

        public DateTime? DT_ADDROW_INTERFAZ { get; set; }

        [StringLength(10)]
        [Column]
        public string DT_MOTIVO { get; set; }

        [StringLength(10)]
        [Column]
        public string DT_REALIZADO { get; set; }

        [StringLength(10)]
        [Column]
        public string DT_UPDROW { get; set; }

        [StringLength(12)]
        [Column]
        public string DT_VENCIMIENTO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AREA_AVERIA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PROCESADO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PROCESAR { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_AJUSTE_STOCK { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_DOCUMENTO { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [StringLength(12)]
        [Column]
        public string NU_INVENTARIO_ENDERECO_DET { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_LOG_INVENTARIO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_REGISTRO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_TRANSACCION { get; set; }

        [StringLength(13)]
        [Column]
        public string QT_MOVIMIENTO { get; set; }

        [StringLength(2)]
        [Column]
        public string TP_AJUSTE { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_DOCUMENTO { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_SERIALIZADO { get; set; }
    }
}
