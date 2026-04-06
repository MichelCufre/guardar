namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_REC170_VALIDAR_FACTURA")]
    public partial class V_REC170_VALIDAR_FACTURA
    {
        [Key]
        [Column(Order = 0)]
        public int? NU_AGENDA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        public decimal? QT_AGENDADO { get; set; }
        public decimal? QT_RECIBIDA { get; set; }
        public decimal? QT_CROSS_DOCKING { get; set; }
        public decimal? QT_FACTURADA { get; set; }
        public decimal? QT_VALIDADA { get; set; }

        [StringLength(16)]
        public string DIFERENCIAS { get; set; }


    }
}
