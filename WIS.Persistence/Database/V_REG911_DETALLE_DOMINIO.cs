
namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_REG911_DETALLE_DOMINIO")]
    public partial class V_REG911_DETALLE_DOMINIO
    {
        [StringLength(10)]
        [Column]
        public string CD_DOMINIO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_DOMINIO_VALOR { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOMINIO_VALOR { get; set; }

        [Key]
        [StringLength(50)]
        [Column]

        public string NU_DOMINIO { get; set; }
    }
}
