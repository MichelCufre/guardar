namespace WIS.Persistence.Database
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_PAR050_TIPO_ESTRUTURA")]
    public partial class V_PAR050_TIPO_ESTRUTURA
    {
        [Key]
        public short CD_TP_ESTR { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_TP_ESTR { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_BLOCADO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_GAIOLA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PORTA_PALETE { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PRATELEIRA { get; set; }

    }
}
