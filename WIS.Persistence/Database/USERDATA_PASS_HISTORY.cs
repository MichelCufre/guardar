namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("USERDATA_PASS_HISTORY")]
    public partial class USERDATA_PASS_HISTORY
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_PASS_HISTORY { get; set; }

        public int USERID { get; set; }

        public int NU_PASS_USERID { get; set; }

        [Required]
        [StringLength(4000)]
        [Column]
        public string PASSWORD { get; set; }

        [Required]
        [StringLength(4000)]
        [Column]
        public string PASSWORDSALT { get; set; }

        public decimal? PASSWORDFORMAT { get; set; }

        public DateTime DT_ADDROW { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_ANEXO { get; set; }
    }
}
