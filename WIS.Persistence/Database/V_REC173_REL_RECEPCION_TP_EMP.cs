namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REC173_REL_RECEPCION_TP_EMP")]
    public partial class V_REC173_REL_RECEPCION_TP_EMP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_RECEPCION_REL_EMPRESA_TIPO { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_RECEPCION_EXTERNO { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_RECEPCION_EXTERNO { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_RECEPCION { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_TIPO_RECEPCION { get; set; }

        public int? CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_MANEJO_INTERFAZ { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_HABILITADO { get; set; }

        public int? CD_INTERFAZ_EXTERNA { get; set; }
    }
}
