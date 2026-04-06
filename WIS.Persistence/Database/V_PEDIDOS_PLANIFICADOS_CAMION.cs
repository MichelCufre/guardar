using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    public class V_PEDIDOS_PLANIFICADOS_CAMION
    {
        [Key]
        [Column(Order = 0)]
        public int CD_CAMION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [Key]
        [Column(Order = 2)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_PUNTO_ENTREGA { get; set; }

        public int? NU_ORDEN_ENTREGA { get; set; }


        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(1)]
        [Column]
        public string PEDIDO_SINCRONIZADO { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_EXPEDICION { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_EXP_MANEJA_TRACKING { get; set; }

        public short? NU_PRIOR_CARGA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }
    }
}
