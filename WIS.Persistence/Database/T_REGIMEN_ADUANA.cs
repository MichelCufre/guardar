using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_REGIMEN_ADUANA")]
    public partial class T_REGIMEN_ADUANA
    {
        [Key]
        public int CD_REGIMEN_ADUANA { get; set; }

        [StringLength(50)]
        public string DS_REGIMEN_ADUANA { get; set; }
    }

}
