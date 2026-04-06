using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_STO721_HISTORIAL_CABEZAL")]
    public partial class V_STO721_HISTORIAL_CABEZAL
    {
        [Key]
        public long NU_LOG_SECUENCIA { get; set; }

        public DateTime? DT_LOG_ADD_ROW { get; set; }

        public int? CD_LOG_USERID { get; set; }

        [StringLength(100)]
        public string FULLNAME { get; set; }

        [StringLength(1)]
        public string ID_LOG_TRIGGER { get; set; }

        public long NU_LPN { get; set; }

        [StringLength(50)]
        public string ID_LPN_EXTERNO { get; set; }

        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

        [StringLength(30)]
        public string NM_LPN_TIPO { get; set; }

        [StringLength(6)]
        public string ID_ESTADO { get; set; }

        [StringLength(100)]
        public string DS_ESTADO { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_ACTIVACION { get; set; }

        public DateTime? DT_FIN { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        public int? NU_AGENDA { get; set; }

        [StringLength(30)]
        public string CD_LOG_APLICACION { get; set; }

        [StringLength(100)]
        public string DS_APLICACAO { get; set; }

        [StringLength(500)]
        public string DS_TRANSACCION { get; set; }
    }
}