namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_EVENTO_MAIL_SALIDA")]
    public partial class T_EVENTO_MAIL_SALIDA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_SECUENCIA_ENVIO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public int? NU_INSTANCIA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_EMAIL_FROM { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_EMAIL_TO { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_SUBJECT { get; set; }

        [StringLength(1000)]
        [Column]
        public string DS_CUERPO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ARCHIVO_1 { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ARCHIVO_2 { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ARCHIVO_3 { get; set; }

        public int? CD_SITUACAO { get; set; }

        public DateTime? DT_SITUACAO { get; set; }

        public DateTime? DT_ENVIO { get; set; }

        public DateTime? DT_REENVIO { get; set; }
    }
}
