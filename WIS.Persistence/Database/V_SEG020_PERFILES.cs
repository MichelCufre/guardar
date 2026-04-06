namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_SEG020_PERFILES")]
    public partial class V_SEG020_PERFILES
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PROFILEID { get; set; }

        [Required]
        [StringLength(200)]
        [Column]
        public string DESCRIPTION { get; set; }

        public int? USERTYPEID { get; set; }

        [StringLength(50)]
        [Column]
        public string USERTYPE { get; set; }
    }
}
