using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_SEG030_USUARIOS")]
    public partial class V_SEG030_USUARIOS
    {
        [Key]
        public int USERID { get; set; }

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

        public int? USERTYPEID { get; set; }

        [StringLength(50)]
        [Column]
        public string USERTYPE { get; set; }

        [StringLength(3)]
        [Column]
        public string LANGUAGE { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ASIG_AUTO_NUEVA_EMPRESA { get; set; }

        [StringLength(1)]
        [Column]
        public string ISENABLED { get; set; }
    }
}
