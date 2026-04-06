namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_CONTACTO")]
    public partial class T_CONTACTO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_CONTACTO()
        {
            T_CONTACTO_GRUPO_REL = new HashSet<T_CONTACTO_GRUPO_REL>();
            T_EVENTO_GRUPO_INSTANCIA_REL = new HashSet<T_EVENTO_GRUPO_INSTANCIA_REL>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_CONTACTO { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_CONTACTO { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_CONTACTO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_EMAIL { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_TELEFONO { get; set; }

        public int? USERID { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_CONTACTO_GRUPO_REL> T_CONTACTO_GRUPO_REL { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EVENTO_GRUPO_INSTANCIA_REL> T_EVENTO_GRUPO_INSTANCIA_REL { get; set; }
    }
}
