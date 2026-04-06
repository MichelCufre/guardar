using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_PREP_DISP_ASOCIAR
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string TP_OPERATIVA { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_PREPARACION { get; set; }

        public int? CD_EMPRESA { get; set; }
    }
}
