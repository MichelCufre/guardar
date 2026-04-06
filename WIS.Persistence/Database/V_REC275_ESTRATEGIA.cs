namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_REC275_ESTRATEGIA")]
    public partial class V_REC275_ESTRATEGIA
    {
        [Key]
        public int NU_ALM_ESTRATEGIA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ALM_ESTRATEGIA { get; set; }
        public DateTime DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        public long? QT_ASOCIACIONES { get; set; }
    }
}
