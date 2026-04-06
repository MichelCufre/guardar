using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{
    [Table("V_EVT060_TEMPLATES_NOTIFICACION")]
    public partial class V_EVT060_TEMPLATES_NOTIFICACION
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(15)]
        public string CD_LABEL_ESTILO { get; set; }

        [StringLength(200)]
        public string DS_LABEL_ESTILO { get; set; }

        [Key]
        [Column(Order = 2)]
        public int NU_EVENTO { get; set; }

        [StringLength(30)]
        public string NM_EVENTO { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(60)]
        public string TP_NOTIFICACION { get; set; }

        [StringLength(70)]
        public string VL_ASUNTO { get; set; }

        [StringLength(1)]
        public string FL_HTML { get; set; }

        public DateTime DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }

    }
}
