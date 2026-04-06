namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_ZONA_UBICACION")]
    public partial class T_ZONA_UBICACION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_ZONA_UBICACION()
        {
            T_ENDERECO_ESTOQUE = new HashSet<T_ENDERECO_ESTOQUE>();
        }

        [Key]
        [StringLength(20)]
        [Column]
        public string CD_ZONA_UBICACION { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ZONA_UBICACION { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_ZONA_UBICACION { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_ZONA_UBICACION_PICKING { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_ESTACION { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_ESTACION_ALMACENAJE { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_HABILITADA { get; set; }

        [Column]
        public int ID_ZONA_UBICACION { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ENDERECO_ESTOQUE> T_ENDERECO_ESTOQUE { get; set; }
    }
}
