using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_USERS
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int USERID { get; set; }

        [Column]
        public string EMAIL { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        [Column]
        public string TP_DOCUMENTO { get; set; }
        [Column]
        public string NU_DOCUMENTO { get; set; }
        public DateTime? FECHA_NACIMIENTO { get; set; }
        [Column]
        public string NU_CELULAR { get; set; }
        [Column]
        public string NOMBRE { get; set; }
        [Column]
        public string APELLIDO { get; set; }
        public int USERTYPEID { get; set; }
        public short? ISENABLED { get; set; }
        [Column]
        public string USERTYPE { get; set; }
    }
}
