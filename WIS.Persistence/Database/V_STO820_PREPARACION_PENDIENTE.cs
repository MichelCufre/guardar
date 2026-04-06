using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_STO820_PREPARACION_PENDIENTE")]
    public partial class V_STO820_PREPARACION_PENDIENTE
    {
        public int NU_PREPARACION { get; set; }

        [StringLength(60)]
        public string DS_PREPARACION { get; set; }

        public int? CD_EMPRESA { get; set; }
        public int? QT_LOTE_AUTO { get; set; }
        public decimal? QT_LPN_ATR { get; set; }
        public decimal? QT_LPN_DET { get; set; }
        public decimal? QT_LPN_DET_PICKING { get; set; }
        public int? QT_PENDIENTE { get; set; }
        public int? QT_PICKING_MAYOR_SUELTO { get; set; }
        public int? QT_PREPARADO { get; set; }
        public decimal? QT_SALDO_SIN_TRABAJAR { get; set; }
        public int? QT_TIPO_EXP_NO_TRASPASO { get; set; }
        public int? QT_REAB_UBIC_BAJAS { get; set; }
    }
}
