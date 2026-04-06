namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("T_PORTERIA_EMPRESA")]
    public partial class T_PORTERIA_EMPRESA
    {
        [Key]
        [StringLength(100)]
        [Column]
        public string CD_PORTERIA_EMPRESA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_OBSERVACIONES { get; set; }

        public DateTime DT_ADDROW { get; set; }
    }
}
