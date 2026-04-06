namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_EVENTO_SALDO_FAC")]
    public partial class V_EVENTO_SALDO_FAC
    {
        [StringLength(1)]
        [Column]
        public string NU_SERIE { get; set; }

        [StringLength(12)]
        [Column]
        public string NU_FACTURA { get; set; }

        [StringLength(12)]
        [Column]
        public string TP_FACTURA { get; set; }

        public int? CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PROVEEDOR { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_PROVEEDOR { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_EXTERNO { get; set; }

        public decimal? QT_AGENDADA { get; set; }

        public decimal? QT_RECIBIDA { get; set; }

        public decimal? QT_RESTANTE { get; set; }
    }
}
