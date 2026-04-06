using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_LOG_PEDIDO_ANULADO_LPN")]
    public partial class T_LOG_PEDIDO_ANULADO_LPN
    {
        [Key]
        public long NU_LOG_PEDIDO_ANULADO_LPN { get; set; }

        public long NU_LOG_PEDIDO_ANULADO { get; set; }
        
        [StringLength(50)]
        [Column]
        public string TP_OPERACION { get; set; }

        [StringLength(50)]
        [Column]
        public string ID_LPN_EXTERNO { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_LPN_TIPO { get; set; }

        public long? NU_DET_PED_SAI_ATRIB { get; set; }

        public decimal QT_ANULADO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
        
    }
}
