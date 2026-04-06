namespace WIS.Persistence.Database
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_INVENTARIO_ENDERECO")]
    public partial class T_INVENTARIO_ENDERECO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_INVENTARIO_ENDERECO()
        {
            T_INVENTARIO_ENDERECO_DET = new HashSet<T_INVENTARIO_ENDERECO_DET>();
        }

        [Key]
        public decimal NU_INVENTARIO_ENDERECO { get; set; }

        public decimal NU_INVENTARIO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_ESTADO_INVENTARIO_ENDERECO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public virtual T_INVENTARIO T_INVENTARIO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_INVENTARIO_ENDERECO_DET> T_INVENTARIO_ENDERECO_DET { get; set; }
    }
}
