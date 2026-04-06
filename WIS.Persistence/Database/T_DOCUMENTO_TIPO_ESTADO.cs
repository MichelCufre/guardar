namespace WIS.Persistence.Database
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DOCUMENTO_TIPO_ESTADO")]
    public partial class T_DOCUMENTO_TIPO_ESTADO
    {
        [Key]
        [StringLength(6)]
        [Column]
        public string TP_DOCUMENTO { get; set; }

        [Key]
        [StringLength(6)]
        [Column]
        public string ID_ESTADO { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_PERMITE_EDICION { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_PERMITE_SIMULAR_CC { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_INICIAL { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_PERMITE_BALANCEO { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_REQUIERE_VALIDACION { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_REQUIERE_AGENDA { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_REQUIERE_LINEAS { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_DISPONIBILIZA_STOCK { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_REQUIERE_DUA { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_REQUIERE_DTI { get; set; }

        [StringLength(1)]
        public string FL_REQUIERE_REFERENCIA_EXTERNA { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_REQUIERE_FACTURA { get; set; }

        [Required]
        [StringLength(1)]
        public string FL_PERMITE_EDITAR_CAMION { get; set; }

        public virtual T_DOCUMENTO_ESTADO T_DOCUMENTO_ESTADO { get; set; }

        public virtual T_DOCUMENTO_TIPO T_DOCUMENTO_TIPO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO> T_DOCUMENTO { get; set; }
    }
}
