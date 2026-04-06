namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PREFERENCIA_REL")]
    public partial class T_PREFERENCIA_REL
    {
        [Key]
        public int NU_PREFERENCIA_REL { get; set; }

        public int NU_PREFERENCIA { get; set; }

        public int? USERID { get; set; }

        public int? CD_EQUIPO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}