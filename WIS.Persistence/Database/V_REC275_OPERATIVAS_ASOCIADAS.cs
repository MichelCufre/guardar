
namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_REC275_OPERATIVAS_ASOCIADAS")]
    public partial class V_REC275_OPERATIVAS_ASOCIADAS
    {
        [Key]
        [StringLength(6)]
        [Column]
        public string TP_RECEPCION { get; set; }

        [Key]
        [Column]
        public string CD_ENTIDAD { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ALM_ESTRATEGIA { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_TIPO_RECEPCION { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ENTIDAD { get; set; }

        public int? NU_ALM_ESTRATEGIA { get; set; }

        [Key]
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [Key]
        [StringLength(10)]
        [Column]
        public string TP_ALM_OPERATIVA_ASOCIABLE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ALM_OPERATIVA_ASOCIABLE { get; set; }

        [Key]
        [Column]
        public string TP_ENTIDAD { get; set; }

        [Key]
        [Column]
        public string CD_EMPRESA { get; set; }

        [Column]
        public string NM_EMPRESA { get; set; }
    }
}
