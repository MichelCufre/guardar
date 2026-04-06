namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PREFERENCIA_TIPO_EXPEDICION")]
    public partial class T_PREFERENCIA_TIPO_EXPEDICION
    {
        [Key]
        public int NU_PREFERENCIA { get; set; }

        [Key]
        [StringLength(6)]
        public string TP_EXPEDICION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
