using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_PEDIDOS_CAMION_EXP050")]
    public partial class V_PEDIDOS_CAMION_EXP050
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_CAMION { get; set; }

        [Key]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        public string DS_CLIENTE { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        public decimal? QT_NO_LIBERADO { get; set; }

        public decimal? QT_NO_PREPARADO { get; set; }

        public decimal? QT_PREPARADO { get; set; }

        [StringLength(40)]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        public string TP_AGENTE { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        public decimal? QT_PEDIDO { get; set; }

        public decimal? QT_EXPEDIDA { get; set; }

        [StringLength(1)]
        public string FL_PROBLEMA { get; set; }
    }
}
