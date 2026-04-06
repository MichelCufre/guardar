namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_LOG_ETIQUETA")]
    public partial class T_LOG_ETIQUETA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_LOG_ETIQUETA { get; set; }
        public int? NU_AGENDA { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_APLICACAO { get; set; }

        public int? NU_ETIQUETA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        public decimal? CD_FAIXA { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_MOVIMIENTO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        public DateTime? DT_OPERACION { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public long? NU_TRANSACCION { get; set; }
        public DateTime? DT_FABRICACAO { get; set; }
        public long? NU_INTERFAZ_EJECUCION { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_MOVIMIENTO { get; set; }
    }
}
