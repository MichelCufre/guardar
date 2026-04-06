namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PUERTA_EMBARQUE_WREG080")]
    public partial class V_PUERTA_EMBARQUE_WREG080
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_PORTA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_PORTA { get; set; }

        public short? CD_SITUACAO { get; set; }

        [Required]
        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_PUERTA { get; set; }
    }
}
