namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_RECEPCION_EMP_TIPO_REPORTE")]
    public partial class T_RECEPCION_EMP_TIPO_REPORTE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_REC_EMP_TIPO_REP { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_RECEPCION_EXTERNO { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_REPORTE { get; set; }
    }
}
