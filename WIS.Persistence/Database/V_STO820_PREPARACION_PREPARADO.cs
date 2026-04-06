using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_STO820_PREPARACION_PREPARADO")]
    public partial class V_STO820_PREPARACION_PREPARADO
    {
        public int NU_PREPARACION { get; set; }

        [StringLength(60)]
        public string DS_PREPARACION { get; set; }

        public int? CD_EMPRESA { get; set; }

        public int? QT_PREPARADO { get; set; }
        public int? QT_TIPO_EXP_NO_TRASPASO { get; set; }
        public int? QT_PENDIENTE { get; set; }
        public decimal? QT_SALDO_SIN_TRABAJAR { get; set; }
    }
}