using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_AUTOMATISMO_PUESTO")]
    public partial class T_AUTOMATISMO_PUESTO
    {

        [Key]
        [Column(Order = 0)]
        public int NU_AUTOMATISMO_PUESTO { get; set; }

        public int NU_AUTOMATISMO { get; set; }

        [Required]
        [StringLength(200)]
        public string ID_PUESTO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UDPROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        [StringLength(50)]
        public string CD_IMPRESORA { get; set; }
        public virtual T_AUTOMATISMO T_AUTOMATISMO { get; set; }
    }
}
