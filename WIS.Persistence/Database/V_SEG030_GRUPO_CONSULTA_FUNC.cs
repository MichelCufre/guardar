namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_SEG030_GRUPO_CONSULTA_FUNC")]
    public partial class V_SEG030_GRUPO_CONSULTA_FUNC
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CD_GRUPO_CONSULTA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_GRUPO_CONSULTA { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int USERID { get; set; }

        [Required]
        [StringLength(50)]
        [Column]
        public string LOGINNAME { get; set; }

        [StringLength(50)]
        [Column]
        public string DOMAINNAME { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string FULLNAME { get; set; }

        [StringLength(100)]
        [Column]
        public string EMAIL { get; set; }

        public short ISENABLED { get; set; }

        public int? USERTYPEID { get; set; }

        [StringLength(50)]
        [Column]
        public string USERTYPE { get; set; }

        [StringLength(3)]
        [Column]
        public string LANGUAGE { get; set; }
    }
}
