using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_AUTOMATISMO_POSICION")]
    public partial class T_AUTOMATISMO_POSICION
    {

        [Key]
        [Column(Order = 0)]
        public int NU_AUTOMATISMO_POSICION { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        public int NU_AUTOMATISMO { get; set; }

        [Required]
        [StringLength(40)]
        public string ND_TIPO_ENDERECO { get; set; }

        [Required]
        [StringLength(40)]
        public string VL_POSICION_EXTERNA { get; set; }

        public int? TP_AGRUPACION_UBIC { get; set; }

        [StringLength(40)]
        public string VL_COMPARTE_AGRUPACION { get; set; }

        public short NU_ORDEN { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UDPROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public virtual T_AUTOMATISMO T_AUTOMATISMO { get; set; }
    }
}
