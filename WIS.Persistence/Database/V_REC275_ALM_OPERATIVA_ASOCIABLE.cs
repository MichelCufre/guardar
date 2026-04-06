namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_REC275_ALM_OPERATIVA_ASOCIABLE")]
    public partial class V_REC275_ALM_OPERATIVA_ASOCIABLE
    {
        [Key]
        [StringLength(6)]
        [Column]
        public string TP_RECEPCION { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_TIPO_RECEPCION { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ALM_OPERATIVA_ASOCIABLE { get; set; }

        [Key]
        [StringLength(10)]
        [Column]

        public string TP_ALM_OPERATIVA_ASOCIABLE { get; set; }
    }
}
