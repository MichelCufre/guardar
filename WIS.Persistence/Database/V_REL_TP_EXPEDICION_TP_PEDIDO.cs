using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_REL_TP_EXPEDICION_TP_PEDIDO
    {
        [Column]
        public string DS_MEMO { get; set; }
        [Column]
        public string DS_TIPO_PEDIDO { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UDPROW { get; set; }
        [Column]
        public string NM_EXPEDICION { get; set; }
        [Key]
        public int NU_REL_TP_EXP_PED { get; set; }
        [Column]
        public string TP_EXPEDICION { get; set; }
        [Column]
        public string TP_PEDIDO { get; set; }
    }
}
