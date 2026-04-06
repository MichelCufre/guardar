using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_EVENTO_ARCHIVO_WEVT050")]
    public class V_EVENTO_ARCHIVO_WEVT050
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_EVENTO_NOTIFICACION_ARCHIVO { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_EVENTO_NOTIFICACION { get; set; }

        public int NU_EVENTO_INSTANCIA { get; set; }

        public int NU_EVENTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_INSTANCIA { get; set; }

        [StringLength(30)]
        [Column]
        public string NM_EVENTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ARCHIVO { get; set; }

        [StringLength(100)]
        [Column]
        public string ID_REFERENCIA { get; set; }

        [StringLength(30)]
        [Column]
        public string TP_REFERENCIA { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
