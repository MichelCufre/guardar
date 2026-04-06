namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PREFERENCIA_EMPRESA")]
    public partial class T_PREFERENCIA_EMPRESA
    {
        [Key]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        public int CD_EMPRESA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
