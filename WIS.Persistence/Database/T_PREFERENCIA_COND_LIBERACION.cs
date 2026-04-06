namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PREFERENCIA_COND_LIBERACION")]
    public partial class T_PREFERENCIA_COND_LIBERACION
    {
        [Key]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        [StringLength(6)]
        public string CD_CONDICION_LIBERACION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
