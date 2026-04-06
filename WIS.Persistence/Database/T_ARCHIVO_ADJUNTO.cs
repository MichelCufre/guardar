namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_ARCHIVO_ADJUNTO")]
    public partial class T_ARCHIVO_ADJUNTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_ARCHIVO_ADJUNTO()
        {
            T_ARCHIVO_ADJUNTO_VERSION = new HashSet<T_ARCHIVO_ADJUNTO_VERSION>();
        }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_ARCHIVO_ADJUNTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(5)]
        public string CD_MANEJO { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(200)]
        public string DS_REFERENCIA { get; set; }

        [Required]
        [StringLength(50)]
        [Column]
        public string TP_DOCUMENTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_OBSERVACION { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        public short? CD_SITUACAO { get; set; }

        public long NU_VERSION_ACTIVA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

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

        [StringLength(300)]
        [Column]
        public string DS_REFERENCIA2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ARCHIVO_ADJUNTO_VERSION> T_ARCHIVO_ADJUNTO_VERSION { get; set; }

        public virtual T_ARCHIVO_MANEJO T_ARCHIVO_MANEJO { get; set; }
    }
}
