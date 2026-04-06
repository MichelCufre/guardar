using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    public class V_PLANIFICACION_CAMION
    {
        [Key]
        [Column(Order = 0)]
        public int CD_CAMION { get; set; }

        [Key]
        [StringLength(20)]
        [Column(Order = 1)]
        public string CD_PUNTO_ENTREGA { get; set; }

        [Key]
        [StringLength(200)]
        [Column(Order = 2)]
        public string VL_COMPARTE_CONTENEDOR_ENTREGA { get; set; }

        [Key]
        [Column(Order = 3)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [Key]
        [Column(Order = 5)]
        public long NU_CONTENEDOR { get; set; }


        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(10)]
        [Column]
        public string DS_CONTENEDOR { get; set; }

        [StringLength(7)]
        [Column]
        public string TP_BULTO { get; set; }
        public decimal? QT_PRODUTO { get; set; }
        public decimal? VL_CUBAGEM_TOTAL { get; set; }
        public decimal? VL_PESO_TOTAL { get; set; }
        public decimal? VL_ALTURA { get; set; }
        public decimal? VL_LARGURA { get; set; }
        public decimal? VL_PROFUNDIDADE { get; set; }

        public short NU_PRIOR_CARGA { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_CONTENEDOR { get; set; }

        [StringLength(50)]
        [Column]
        public string ID_EXTERNO_CONTENEDOR { get; set; }
        public long? ID_EXTERNO_TRACKING { get; set; }

        [StringLength(100)]
        [Column]
        public string CD_BARRAS { get; set; }

    }
}
