namespace WIS.Persistence.Database
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DOCUMENTO_TIPO")]
    public partial class T_DOCUMENTO_TIPO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_DOCUMENTO_TIPO()
        {
            T_DOCUMENTO = new HashSet<T_DOCUMENTO>();
            T_DOCUMENTO_TIPO_EXTERNO = new HashSet<T_DOCUMENTO_TIPO_EXTERNO>();
            T_TIPO_DUA_DOCUMENTO = new HashSet<T_TIPO_DUA_DOCUMENTO>();
            T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO = new HashSet<T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO>();
            T_DOCUMENTO_ESTADO_ORDEN = new HashSet<T_DOCUMENTO_ESTADO_ORDEN>();
            T_DOCUMENTO_TIPO_ESTADO = new HashSet<T_DOCUMENTO_TIPO_ESTADO>();
        }

        [Key]
        [Column(Order = 0)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        [Column(Order = 1)]
        [StringLength(2)]
        public string TP_OPERACION { get; set; }

        [StringLength(50)]
        public string DS_TP_DOCUMENTO { get; set; }

        [StringLength(1)]
        public string FL_NUMERO_AUTOGENERADO { get; set; }

        [StringLength(1)]
        public string FL_INGRESO_MANUAL { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO { get; set; }

        [StringLength(1)]
        public string FL_MANEJA_AGRUPADOR { get; set; }

        [StringLength(1)]
        public string FL_MANEJA_CAMBIO_ESTADO { get; set; }

        [StringLength(1)]
        public string FL_REQUIERE_DUA { get; set; }

        [StringLength(1)]
        public string FL_REQUIERE_DTI { get; set; }

        [StringLength(1)]
        public string FL_REQUIERE_REFERENCIA_EXTERNA { get; set; }

        [StringLength(1)]
        public string FL_PERMITE_EDICION { get; set; }

        [StringLength(1)]
        public string FL_REQUIERE_FACTURA { get; set; }

        [StringLength(1)]
        public string FL_AUTOAGENDABLE { get; set; }        
        
        [StringLength(1)]
        public string FL_MANEJA_AGENDA { get; set; }

        [StringLength(1)]
        public string FL_MANEJA_CAMION { get; set; }

        [StringLength(50)]
        public string NM_SECUENCIA { get; set; }

        [StringLength(1)]
        public string FL_PERMITE_AGREGAR_DETALLE { get; set; }

        [StringLength(1)]
        public string FL_PERMITE_REMOVER_DETALLE { get; set; }

        [StringLength(20)]
        public string VL_MASK { get; set; }

        [StringLength(5)]
        public string VL_MASK_CHARS { get; set; }

        [StringLength(1)]
        public string FL_IE_HABILITADA { get; set; }

        public short VL_LARGO_MAX_NU_DOCUMENTO { get; set; }

        public short VL_LARGO_PREFIJO { get; set; }

        public decimal QT_MIN_INGRESADA { get; set; }

        [StringLength(1)]
        public string FL_PERMITE_EDITAR_CAMION { get; set; }

        [StringLength(1)]
        public string FL_DISPONIBILIZA_STOCK { get; set; }

        [StringLength(1)]
        public string FL_PERMITE_TRASPASO { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO> T_DOCUMENTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO_TIPO_EXTERNO> T_DOCUMENTO_TIPO_EXTERNO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_TIPO_DUA_DOCUMENTO> T_TIPO_DUA_DOCUMENTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO> T_TIPO_REFERENCIA_EXTERNA_DOCUMENTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO_ESTADO_ORDEN> T_DOCUMENTO_ESTADO_ORDEN { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO_TIPO_ESTADO> T_DOCUMENTO_TIPO_ESTADO { get; set; }
    }
}
