namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_EVENTO_INSTANCIA_WEVT040")]
    public partial class V_EVENTO_INSTANCIA_WEVT040
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_EVENTO_INSTANCIA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_INSTANCIA { get; set; }

        public int NU_EVENTO { get; set; }

        [StringLength(30)]
        [Column]
        public string NM_EVENTO { get; set; }

        [StringLength(60)]
        [Column]
        public string TP_NOTIFICACION { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_NOTIFICACION { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string FL_HABILITADO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
