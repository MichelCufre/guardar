using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class T_ANULACIONES_PENDIENTES
    {
        [Key]
        public int NU_ANULACIONES_PENDIENTES { get; set; }
        [StringLength(40)]
        [Column]
        public string NU_PEDIDO { get; set; }
        public int CD_EMPRESA { get; set; }
        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }
        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }
        public decimal QT_ANULAR { get; set; }
        public decimal QT_PENDIENTE { get; set; }
        [StringLength(100)]
        [Column]
        public string DS_ANEXO1 { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public int NU_PREPARACION { get; set; }
        public DateTime? DT_UPDROW { get; set; }
    }
}
