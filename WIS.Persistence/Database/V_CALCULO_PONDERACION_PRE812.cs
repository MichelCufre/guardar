using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_CALCULO_PONDERACION_PRE812")]
    public partial class V_CALCULO_PONDERACION_PRE812
    {

        [Key]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        public string DS_CLIENTE { get; set; }

        [Key]
        [StringLength(20)]
        public string CD_PONDEREDOR { get; set; }
        public decimal? VL_PUNTUACION { get; set; }

    }
}