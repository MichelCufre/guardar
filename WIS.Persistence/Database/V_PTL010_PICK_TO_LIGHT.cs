using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_PTL010_PICK_TO_LIGHT")]
    public partial class V_PTL010_PICK_TO_LIGHT
    {

        [Key]
        [Column(Order = 0)]
        public int NU_PREPARACION { get; set; }

        public int NU_AUTOMATISMO { get; set; }

        [StringLength(400)]
        public string DS_AUTOMATISMO { get; set; }

        [StringLength(20)]
        public string CD_ZONA_UBICACION { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(40)]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        public string DS_CLIENTE { get; set; }

        [Key]
        [Column(Order = 6)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        public decimal? QT_PEDIDOS { get; set; }

        public decimal? QT_POSICIONES { get; set; }

        public decimal? QT_VL_COMPARTE_CONT_PICK { get; set; }

        public decimal? QT_SUB_CLASES_PRODUCTOS { get; set; }

        public decimal? QT_VOLUMEN_TOTAL { get; set; }

        public decimal? PJ_DISP_CUMPLIR { get; set; }

        public string FL_LIB_OUT_PTL { get; set; }

    }
}
