using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_AUTOMATISMO_DATA")]
    public partial class T_AUTOMATISMO_DATA
    {

        [Key]
        [Column(Order = 0)]
        public int NU_AUTOMATISMO_DATA { get; set; }

        public int NU_AUTOMATISMO_EJECUCION { get; set; }

        public byte[] VL_DATA_REQUEST { get; set; }

        public byte[] VL_DATA_RESPONSE { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UDPROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public virtual T_AUTOMATISMO_EJECUCION T_AUTOMATISMO_EJECUCION { get; set; }


    }
}
