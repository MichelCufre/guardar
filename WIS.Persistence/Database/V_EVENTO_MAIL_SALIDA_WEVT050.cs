namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_EVENTO_MAIL_SALIDA_WEVT050")]
    public partial class V_EVENTO_MAIL_SALIDA_WEVT050
    {
        public DateTime DT_ADDROW { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_SUBJECT { get; set; }

        public int? CD_SITUACAO { get; set; }

        [Required]
        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        public DateTime? DT_SITUACAO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_EMAIL_FROM { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_EMAIL_TO { get; set; }

        public DateTime? DT_ENVIO { get; set; }

        public DateTime? DT_REENVIO { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_SECUENCIA_ENVIO { get; set; }

        public int? NU_INSTANCIA { get; set; }

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
    }
}
