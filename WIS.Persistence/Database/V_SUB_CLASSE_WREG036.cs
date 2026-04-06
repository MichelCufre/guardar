namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_SUB_CLASSE_WREG036")]
    public partial class V_SUB_CLASSE_WREG036
    {
        [Key]
        [StringLength(2)]
        [Column]
        public string CD_SUB_CLASSE { get; set; }

        [Required]
        [StringLength(35)]
        [Column]
        public string DS_SUB_CLASSE { get; set; }

        public DateTime DT_UPDROW { get; set; }

        public DateTime DT_ADDROW { get; set; }
    }
}
