namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PROFILERESOURCES")]
    public partial class PROFILERESOURCES
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PROFILERESOURCEID { get; set; }
        public int? PROFILEID { get; set; }
        public int? RESOURCEID { get; set; }
        public virtual PROFILES PROFILES { get; set; }
        public virtual RESOURCES RESOURCES { get; set; }
    }
}
