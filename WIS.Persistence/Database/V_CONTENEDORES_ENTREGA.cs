using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    public class V_CONTENEDORES_ENTREGA
    {
        [Key]
        [Column(Order = 0)]
        public int NU_CONTENEDOR { get; set; }
        [Key]
        [Column(Order = 1)]
        public int CD_EMPRESA { get; set; }
        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }
        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_CONTENEDOR { get; set; }
        public short? CD_SITUACAO { get; set; }
        public int CD_CAMION { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_OBJETO_TRACKING { get; set; }

        [StringLength(1)]
        [Column]
        public string PEDIDO_SINCRONIZADO { get; set; }

        [StringLength(50)]
        [Column]
        public string ID_EXTERNO_CONTENEDOR { get; set; }

        public decimal? QT_PRODUTO { get; set; }
        public decimal? VL_CUBAGEM { get; set; }
        public decimal? PS_BRUTO_TOTAL { get; set; }
        public decimal? VL_ALTURA { get; set; }
        public decimal? VL_LARGURA { get; set; }
        public decimal? VL_PROFUNDIDADE { get; set; }
        public long? ID_EXTERNO_TRACKING { get; set; }

        [StringLength(55)]
        [Column]
        public string DS_CONTENEDOR { get; set; }

        [StringLength(100)]
        [Column]
        public string CD_BARRAS { get; set; }
    }
}
