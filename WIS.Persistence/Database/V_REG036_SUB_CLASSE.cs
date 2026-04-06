namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REG036_SUB_CLASSE")]
    public partial class V_REG036_SUB_CLASSE
    {
        [Key]
        [StringLength(2)]
        [Column]
        public string CD_SUB_CLASSE { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_SUB_CLASSE { get; set; }

        public DateTime DT_UPDROW { get; set; }

        public DateTime DT_ADDROW { get; set; }
    }
}
