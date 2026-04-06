namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_HIST_PRDC_LINEA_PRODUCIDO")]
    public partial class T_HIST_PRDC_LINEA_PRODUCIDO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_HISTORICO { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string CD_PRDC_DEFINICION { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string NU_PRDC_INGRESO { get; set; }

        public int NU_FORMULA { get; set; }

        public int NU_PASADA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_PRODUCIDO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_VENCIMIENTO { get; set; }

        public DateTime? DT_ADDHIST { get; set; }
    }
}
