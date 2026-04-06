using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("T_RECORRIDO_DET")]
    public partial class T_RECORRIDO_DET
    {

        [Key]
        [Column(Order = 0)]
        public long NU_RECORRIDO_DET { get; set; }

        public int NU_RECORRIDO { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        public long NU_ORDEN { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        [StringLength(40)]
        public string VL_ORDEN { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
