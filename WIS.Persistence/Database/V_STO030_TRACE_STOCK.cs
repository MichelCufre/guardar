namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_STO030_TRACE_STOCK")]
    public partial class V_STO030_TRACE_STOCK
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_TRACE_STOCK { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_APLICACAO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_APLICACAO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string NM_FUNCIONARIO { get; set; }

        [Required]
        [StringLength(50)]
        [Column]
        public string LOGINNAME { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        public decimal? CD_FAIXA { get; set; }

        public decimal? QT_MOVIMIENTO { get; set; }

        public decimal? QT_ESTOQUE { get; set; }

        public decimal? QT_RESERVA_SAIDA { get; set; }

        public decimal? QT_TRANSITO_ENTRADA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_INVENTARIO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_CTRL_CALIDAD { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AVERIA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PROCESAR { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
