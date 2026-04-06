namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PRDC_ACCION")]
    public partial class T_PRDC_ACCION
    {
        [Key]
        [StringLength(10)]
        [Column]
        public string CD_ACCION { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ACCION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string TP_ACCION { get; set; }
    }
}
