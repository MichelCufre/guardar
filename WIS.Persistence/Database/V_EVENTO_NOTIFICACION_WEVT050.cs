namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_EVENTO_NOTIFICACION_WEVT050")]
    public partial class V_EVENTO_NOTIFICACION_WEVT050
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_EVENTO_NOTIFICACION { get; set; }

        public DateTime? DT_ENVIO { get; set; }

        public DateTime? DT_RENVIO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_EMAIL_TO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_EMAIL_FROM { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_SUBJECT { get; set; }

        [StringLength(100)]
        [Column]
        public string ND_ESTADO { get; set; }

        public long? ARCHIVOS { get; set; }

        [StringLength(100)]
        [Column]
        public string TP_NOTIFICACION { get; set; }
    }
}
