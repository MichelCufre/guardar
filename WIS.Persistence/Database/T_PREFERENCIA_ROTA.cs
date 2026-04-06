namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PREFERENCIA_ROTA")]
    public partial class T_PREFERENCIA_ROTA
    {
        [Key]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        public int CD_ROTA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
