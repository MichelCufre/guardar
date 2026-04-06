namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_RESUMEN_AGENDA_WREC250")]
    public partial class V_RESUMEN_AGENDA_WREC250
    {
        public int? CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        [StringLength(21)]
        [Column]
        public string CD_SITUACAO { get; set; }

        [StringLength(27)]
        [Column]
        public string DT_LIBERACIO { get; set; }

        [StringLength(27)]
        [Column]
        public string DT_INI_RECEPCION { get; set; }

        [StringLength(27)]
        [Column]
        public string DT_FIN_RECEPCION { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public int? CD_FUN_RESP { get; set; }

        public DateTime? DT_FUN_RESP { get; set; }

        public int? NU_COLOR { get; set; }

        public decimal? QT_ACEPTADA { get; set; }

        public decimal? QT_RECIBIDA { get; set; }

        public decimal? QT_CROSS_DOCKING { get; set; }

        [StringLength(100)]
        [Column]
        public string FULLNAME { get; set; }

        public int? CD_FUNCIONARIO_ASIGNADO { get; set; }

        [StringLength(100)]
        public string NM_FUNCIONARIO_ASIGNADO { get; set; }

    }
}
