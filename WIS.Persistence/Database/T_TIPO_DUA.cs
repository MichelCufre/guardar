namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_TIPO_DUA")]
    public partial class T_TIPO_DUA
    {
        [Key]
        [StringLength(2)]
        [Column]
        public string TP_DUA { get; set; }

        [Required]
        [StringLength(35)]
        [Column]
        public string DS_DUA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_TIPO_DUA_DOCUMENTO> T_TIPO_DUA_DOCUMENTO { get; set; }
    }
}
