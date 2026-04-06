using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_REG104_ZONA")]
    public partial class V_REG104_ZONA
    {
        [Key]
        [StringLength(20)]
        public string CD_ZONA { get; set; }

        [Required]
        [StringLength(100)]
        public string NM_ZONA { get; set; }

        [StringLength(200)]
        public string DS_ZONA { get; set; }

        [StringLength(40)]
        public string CD_DEPARTAMENTO { get; set; }

        [StringLength(40)]
        public string CD_LOCALIDAD { get; set; }

        [StringLength(100)]
        public string DS_LOCALIDAD { get; set; }
    }
}

