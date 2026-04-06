namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_EVENTO_NOTIFICACION_EMAIL")]
    public partial class T_EVENTO_NOTIFICACION_EMAIL
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_EVENTO_NOTIFICACION { get; set; }

        public DateTime? DT_ENVIO { get; set; }

        public byte[] DS_CUERPO { get; set; }

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

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_HTML { get; set; }

        public virtual T_EVENTO_NOTIFICACION T_EVENTO_NOTIFICACION { get; set; }
    }
}
