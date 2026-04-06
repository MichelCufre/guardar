namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_EVENTO_SALDO_REF")]
    public partial class V_EVENTO_SALDO_REF
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string NU_REFERENCIA { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_PROVEEDOR { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_PROVEEDOR { get; set; }

        public int? CD_EMPRESA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_EXTERNO { get; set; }

        public decimal? QT_REFERENCIA { get; set; }

        public decimal? QT_RECIBIDA { get; set; }

        public decimal? QT_RESTANTE { get; set; }
    }
}
