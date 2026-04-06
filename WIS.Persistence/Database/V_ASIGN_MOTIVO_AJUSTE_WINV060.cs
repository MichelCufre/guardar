namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_ASIGN_MOTIVO_AJUSTE_WINV060")]
    public partial class V_ASIGN_MOTIVO_AJUSTE_WINV060
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AJUSTE_STOCK { get; set; }

        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [Required]
        [StringLength(3)]
        [Column]
        public string CD_MOTIVO_AJUSTE { get; set; }

        [Required]
        [StringLength(60)]
        [Column]
        public string DS_MOTIVO_AJUSTE { get; set; }

        public decimal CD_FAIXA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

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

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
