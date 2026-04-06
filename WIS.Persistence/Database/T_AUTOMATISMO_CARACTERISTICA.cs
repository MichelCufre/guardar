using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_AUTOMATISMO_CARACTERISTICA")]
    public partial class T_AUTOMATISMO_CARACTERISTICA
    {

        [Key]
        [Column(Order = 0)]
        public decimal NU_AUTOMATISMO_CARACTERISTICA { get; set; }

        public int NU_AUTOMATISMO { get; set; }

        [Required]
        [StringLength(100)]
        public string CD_AUTOMATISMO_CARACTERISTICA { get; set; }

        [Required]
        [StringLength(400)]
        public string VL_AUTOMATISMO_CARACTERISTICA { get; set; }

        [StringLength(400)]
        public string DS_AUTOMATISMO_CARACTERISTICA { get; set; }

        [StringLength(100)]
        public string VL_AUX1 { get; set; }

        public long? NU_AUX1 { get; set; }

        public decimal? QT_AUX1 { get; set; }

        [StringLength(1)]
        public string FL_AUX1 { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UDPROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public virtual T_AUTOMATISMO T_AUTOMATISMO { get; set; }

    }
}
