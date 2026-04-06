namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PREFERENCIA_ZONA")]
    public partial class T_PREFERENCIA_ZONA
    {
        [Key]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        [StringLength(20)]
        public string CD_ZONA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
