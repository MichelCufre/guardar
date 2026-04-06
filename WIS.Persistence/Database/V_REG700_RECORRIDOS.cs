using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WIS.Persistence.Database
{
    [Table("V_REG700_RECORRIDOS")]
    public partial class V_REG700_RECORRIDOS
    {

        [Key]
        [Column(Order = 0)]
        public int NU_RECORRIDO { get; set; }

        [Required]
        [StringLength(50)]
        public string NM_RECORRIDO { get; set; }

        [Required]
        [StringLength(200)]
        public string DS_RECORRIDO { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_DEFAULT { get; set; }

        [Required]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [StringLength(1)]
        [Required]
        public string FL_HABILITADO { get; set; }
        [StringLength(1)]
        public string FL_UBIC_FALTANTE { get; set; }
    }
}
