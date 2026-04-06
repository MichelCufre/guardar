using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_REC170_RECEPCION_LPNS")]
    public partial class V_REC170_RECEPCION_LPNS
    {
        [Key]
        [Column(Order = 0)]
        public int NU_AGENDA { get; set; }

        [Key]
        [Column(Order = 1)]
        public long NU_LPN { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(1)]
        public string FL_PLANIFICADO { get; set; }

        [StringLength(1)]
        public string FL_RECIBIDO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public int? CD_FUNCIONARIO_RECEPCION { get; set; }

        public DateTime? DT_RECEPCION { get; set; }

        [Required]
        [StringLength(50)]
        public string ID_LPN_EXTERNO { get; set; }

        [Required]
        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

        [StringLength(100)]
        public string NM_FUNCIONARIO { get; set; }

        [StringLength(100)]
        public string NM_FUNCIONARIO_RECEPCION { get; set; }
    }
}