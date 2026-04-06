using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_STO700_LT_LPN")]
    public partial class V_STO700_LT_LPN
    {
        [Key]
        public long NU_LOG_SECUENCIA { get; set; }

        public DateTime? DT_LOG_ADD_ROW { get; set; }

        public int? CD_LOG_USERID { get; set; }

        [StringLength(1)]
        public string ID_LOG_TRIGGER { get; set; }

        [Column(Order = 5)]
        public long NU_LPN { get; set; }

        [Required]
        [StringLength(50)]
        public string ID_LPN_EXTERNO { get; set; }

        [Required]
        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

        [Required]
        [StringLength(6)]
        public string ID_ESTADO { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_ACTIVACION { get; set; }

        public DateTime? DT_FIN { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public int CD_EMPRESA { get; set; }

        public int? NU_AGENDA { get; set; }

    }
}