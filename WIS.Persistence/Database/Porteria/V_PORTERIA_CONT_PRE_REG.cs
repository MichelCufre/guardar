namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PORTERIA_CONT_PRE_REG")]
    public partial class V_PORTERIA_CONT_PRE_REG
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short NU_SEQ_CONTAINER { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string NU_CONTAINER { get; set; }
    }
}
