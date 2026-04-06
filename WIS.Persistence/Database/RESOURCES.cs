namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("RESOURCES")]
    public partial class RESOURCES
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RESOURCES()
        {
            PROFILERESOURCES = new HashSet<PROFILERESOURCES>();
            USERPERMISSIONS = new HashSet<USERPERMISSIONS>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RESOURCEID { get; set; }

        [Required]
        [StringLength(200)]
        [Column]
        public string DESCRIPTION { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string NAME { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string UNIQUENAME { get; set; }

        public int? USERTYPEID { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_ACTIVO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PROFILERESOURCES> PROFILERESOURCES { get; set; }

        public virtual USERTYPES USERTYPES { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USERPERMISSIONS> USERPERMISSIONS { get; set; }
    }
}
