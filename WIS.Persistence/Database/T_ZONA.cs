using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("T_ZONA")]
    public partial class T_ZONA
    {
        [Key]
        [Column(Order = 0)]
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

    }
}