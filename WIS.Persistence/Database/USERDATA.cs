namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("USERDATA")]
    public partial class USERDATA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int USERID { get; set; }

        public DateTime? LASTLOGINDATETIME { get; set; }

        public DateTime? LASTLOCKOUTDATETIME { get; set; }

        public short? ISLOCKED { get; set; }

        public decimal? LOCKCAUSE { get; set; }

        public decimal? FAILEDPASSWORDATTEMPTCOUNT { get; set; }

        [StringLength(4000)]
        [Column]
        public string PASSWORD { get; set; }

        [StringLength(4000)]
        [Column]
        public string PASSWORDSALT { get; set; }

        public decimal? PASSWORDFORMAT { get; set; }

        public DateTime? LASTPASSWORDCHANGEDATETIME { get; set; }

        public DateTime? FAILEDPASSWORDATTEMPTWINSTART { get; set; }

        [StringLength(40)]
        [Column]
        public string ID_TENANT { get; set; }

        public virtual USERS USERS { get; set; }
    }
}
