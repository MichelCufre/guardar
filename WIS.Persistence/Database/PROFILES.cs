namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PROFILES")]
    public partial class PROFILES
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PROFILES()
        {
            PROFILERESOURCES = new HashSet<PROFILERESOURCES>();
            USERPERMISSIONS = new HashSet<USERPERMISSIONS>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PROFILEID { get; set; }

        [Required]
        [StringLength(200)]
        [Column]
        public string DESCRIPTION { get; set; }

        public int? USERTYPEID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PROFILERESOURCES> PROFILERESOURCES { get; set; }

        public virtual USERTYPES USERTYPES { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USERPERMISSIONS> USERPERMISSIONS { get; set; }
    }
}
