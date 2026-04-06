using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    public class V_PEDIDOS_NO_PLANIFICADOS
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 2)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        public DateTime? DT_EMITIDO { get; set; }

        public DateTime? DT_ENTREGA { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_PUNTO_ENTREGA { get; set; }

        [StringLength(200)]
        [Column]
        public string VL_COMPARTE_CONTENEDOR_PICKING { get; set; }

        [StringLength(200)]
        [Column]
        public string VL_COMPARTE_CONTENEDOR_ENTREGA { get; set; }

    }
}
