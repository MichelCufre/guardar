namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("USERPERMISSIONS")]
    public partial class USERPERMISSIONS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int USERPERMISSIONID { get; set; }

        public int USERID { get; set; }

        public int? PROFILEID { get; set; }

        public int? RESOURCEID { get; set; }

        public virtual PROFILES PROFILES { get; set; }

        public virtual RESOURCES RESOURCES { get; set; }

        public virtual USERS USERS { get; set; }
    }
}
