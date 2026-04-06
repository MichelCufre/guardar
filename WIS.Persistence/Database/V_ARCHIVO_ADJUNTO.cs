namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_ARCHIVO_ADJUNTO")]
    public partial class V_ARCHIVO_ADJUNTO
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_ARCHIVO_ADJUNTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string CD_MANEJO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_MANEJO { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(200)]
        public string DS_REFERENCIA { get; set; }

        [StringLength(300)]
        [Column]
        public string DS_REFERENCIA2 { get; set; }

        [Required]
        [StringLength(50)]
        [Column]
        public string TP_DOCUMENTO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TP_DOCUMENTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_OBSERVACION { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }
        [StringLength(200)]
        [Column]
        public string DS_ANEXO2 { get; set; }
        [StringLength(200)]
        [Column]
        public string DS_ANEXO3 { get; set; }
        [StringLength(200)]
        [Column]
        public string DS_ANEXO4 { get; set; }
        [StringLength(200)]
        [Column]
        public string DS_ANEXO5 { get; set; }
        [StringLength(200)]
        [Column]
        public string DS_ANEXO6 { get; set; }
        public short? CD_SITUACAO { get; set; }

        public long NU_VERSION_ACTIVA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(500)]
        [Column]
        public string LK_RUTA { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_ARCHIVO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TP_ARCHIVO { get; set; }

        public int CD_FUNCIONARIO { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_FUNCIONARIO { get; set; }
    }
}
