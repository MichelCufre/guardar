namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_EVENTO_WEVT010")]
    public partial class V_EVENTO_WEVT010
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_EVENTO { get; set; }

        [StringLength(30)]
        [Column]
        public string NM_EVENTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_EVENTO { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_PROGRAMADO { get; set; }
    }
}
