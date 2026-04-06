using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public partial class V_RECURSOS_USUARIO
    {
        [Key]
        [Column(Order = 1)]
        public int RESOURCEID { get; set; }
        [Column]
        public string UNIQUENAME { get; set; }
        [Key]
        [Column(Order = 0)]
        public int USERID { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ACTIVO { get; set; }
    }
}
