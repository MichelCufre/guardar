using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;

namespace WIS.Persistence.Database
{
    [Table("T_USUARIO_CONFIGURACION")]
    public partial class T_USUARIO_CONFIGURACION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int USERID { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ASIG_AUTO_NUEVA_EMPRESA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public virtual USERS USERS { get; set; }
    }
}
