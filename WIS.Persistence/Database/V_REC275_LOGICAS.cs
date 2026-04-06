
namespace WIS.Persistence.Database
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_REC275_LOGICAS")]
    public partial class V_REC275_LOGICAS
    {
        [Key]
        public short NU_ALM_LOGICA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ALM_LOGICA { get; set; }
    }
}
