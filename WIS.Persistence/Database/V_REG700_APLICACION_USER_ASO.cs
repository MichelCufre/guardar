using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_REG700_APLICACION_USER_ASO")]
    public partial class V_REG700_APLICACION_USER_ASO
    {

        [Key]
        public int NU_RECORRIDO { get; set; }

        [Key]
        [StringLength(30)]
        public string CD_APLICACION { get; set; }

        [StringLength(100)]
        public string DS_APLICACAO { get; set; }

        [Key]
        public int USERID { get; set; }

        [Required]
        [StringLength(50)]
        public string LOGINNAME { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_PREDETERMINADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

    }
}
