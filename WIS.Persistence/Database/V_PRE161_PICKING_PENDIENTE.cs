using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_PRE161_PICKING_PENDIENTE")]
    public partial class V_PRE161_PICKING_PENDIENTE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION { get; set; }

        public long? QT_PEDIDOS { get; set; }

        public double? AUXPEDI_COMPLETOS { get; set; }

        public double? AUXPORC_PEDIDOS { get; set; }

        public long? QT_CLIENTES { get; set; }

        public double? AUXCLIENTES_COMPLETOS { get; set; }

        public double? AUXPORC_CLIENTES { get; set; }

        public decimal? QT_PRODUCTOS { get; set; }

        public decimal? AUXUNIDADES_PREPARADAS { get; set; }

        public decimal? AUXPORC_UNIDADES { get; set; }

        public long? QT_PICKEOS { get; set; }

        public double? AUXPICKEOS_PREPARADOS { get; set; }

        public double? AUXPORC_PICKEOS { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [StringLength(60)]
        public string DS_PREPARACION { get; set; }

        public DateTime? DT_INICIO { get; set; }
    }
}
