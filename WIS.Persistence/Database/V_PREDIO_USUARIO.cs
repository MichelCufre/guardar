namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PREDIO_USUARIO")]
    public partial class V_PREDIO_USUARIO
    {
        [Key]
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_PREDIO { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int USERID { get; set; }

        [Required]
        [StringLength(50)]
        [Column]
        public string LOGINNAME { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string FULLNAME { get; set; }
    }
}
