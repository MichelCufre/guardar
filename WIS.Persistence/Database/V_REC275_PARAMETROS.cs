
namespace WIS.Persistence.Database
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_REC275_PARAMETROS")]
    public partial class V_REC275_PARAMETROS
    {
        [Key]
        public short NU_ALM_PARAMETRO { get; set; }
        public short? NU_ALM_LOGICA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ALM_PARAMETRO { get; set; }
        
        [StringLength(30)]
        [Column]
        public string NM_ALM_PARAMETRO { get; set; }
        
        [StringLength(100)]
        [Column]
        public string VL_ALM_PARAMETRO_DEFAULT { get; set; }
    }
}
