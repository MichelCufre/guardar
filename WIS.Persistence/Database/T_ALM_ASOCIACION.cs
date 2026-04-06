namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("T_ALM_ASOCIACION")]
    public partial class T_ALM_ASOCIACION
    {
        
        [StringLength(24)]
        [Column]
        public string CD_ALM_OPERATIVA_ASOCIABLE { get; set; }

        [StringLength(2)]
        [Column]
        public string CD_CLASSE { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_GRUPO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }
        public DateTime DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

        [Key]
        public int NU_ALM_ASOCIACION { get; set; }

        public int NU_ALM_ESTRATEGIA { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_ALM_ASOCIACION { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_ALM_OPERATIVA_ASOCIABLE { get; set; }

        [StringLength(64)]
        [Column]
        public string VL_AUDITORIA { get; set; }

    }
}
