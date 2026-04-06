namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_TIPO_CODIGO_BARRAS")]
    public partial class T_TIPO_CODIGO_BARRAS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_TIPO_CODIGO_BARRAS()
        {
            T_CODIGO_BARRAS = new HashSet<T_CODIGO_BARRAS>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TP_CODIGO_BARRAS { get; set; }

        [StringLength(40)]
        [Column]
        public string DS_CODIGO_BARRAS { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_CODIGO_BARRAS> T_CODIGO_BARRAS { get; set; }
    }
}
