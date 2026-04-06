using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_PTL010_AGRU_SUBCLASE_PROD")]
    public partial class V_PTL010_AGRU_SUBCLASE_PROD
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

        [StringLength(2)]
        public string CD_SUB_CLASSE { get; set; }
        [StringLength(200)]
        public string VL_COMPARTE_CONTENEDOR_PICKING { get; set; }

    }
}
