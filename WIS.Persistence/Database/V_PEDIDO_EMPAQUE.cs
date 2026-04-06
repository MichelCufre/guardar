using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_PEDIDO_EMPAQUE")]
    public partial class V_PEDIDO_EMPAQUE
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }
        [Key]
        [Column(Order = 2)]
        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        public string DS_CLIENTE { get; set; }

        [StringLength(6)]
        public string TP_EXPEDICION { get; set; }

        [StringLength(60)]
        public string NM_EXPEDICION { get; set; }

        [StringLength(6)]
        public string TP_PEDIDO { get; set; }

        [StringLength(60)]
        public string DS_TIPO_PEDIDO { get; set; }

        [StringLength(1)]
        public string FL_FACTURAR_EN_EMPAQUETADO { get; set; }

        [StringLength(1)]
        public string TP_ARMADO_EGRESO { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        public decimal? QT_CONTENEDOR_EMPACADO { get; set; }

        public decimal? QT_CONTENEDOR_SIN_EMPACAR { get; set; }

        public int? CD_CAMION { get; set; }

        public DateTime? DT_FACTURACION { get; set; }

    }
}
