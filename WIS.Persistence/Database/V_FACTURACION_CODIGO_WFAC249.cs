namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_FACTURACION_CODIGO_WFAC249")]
    public partial class V_FACTURACION_CODIGO_WFAC249
    {
        [Key]
        public string CD_FACTURACION { get; set; }
        [StringLength(100)]
        public string DS_FACTURACION { get; set; }
        [StringLength(1)]
        public string TP_CALCULO { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
    }
}
