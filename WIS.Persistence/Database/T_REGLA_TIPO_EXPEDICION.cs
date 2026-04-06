namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_REGLA_TIPO_EXPEDICION")]
    public partial class T_REGLA_TIPO_EXPEDICION
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_REGLA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_EXPEDICION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public virtual T_REGLA_LIBERACION T_REGLA_LIBERACION { get; set; }
    }
}
