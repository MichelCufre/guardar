using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_REG700_RECORRIDO_DET")]
    public partial class V_REG700_RECORRIDO_DET
    {

        [Key]
        [Column(Order = 0)]
        public long NU_RECORRIDO_DET { get; set; }

        public int NU_RECORRIDO { get; set; }

        [Required]
        [StringLength(50)]
        public string NM_RECORRIDO { get; set; }


        [Required]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [Required]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        public long? NU_ORDEN { get; set; }

        [StringLength(40)]
        public string VL_ORDEN { get; set; }
    }
}
