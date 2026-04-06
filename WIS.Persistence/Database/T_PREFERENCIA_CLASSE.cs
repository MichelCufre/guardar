namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PREFERENCIA_CLASSE")]
    public partial class T_PREFERENCIA_CLASSE
    {
        [Key]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        [StringLength(2)]
        public string CD_CLASSE { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
