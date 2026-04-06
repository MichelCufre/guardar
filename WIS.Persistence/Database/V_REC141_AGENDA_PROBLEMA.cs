namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REC141_AGENDA_PROBLEMA")]
    public partial class V_REC141_AGENDA_PROBLEMA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_RECEPCION_AGENDA_PROBLEMA { get; set; }

        public int NU_AGENDA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string ND_TIPO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ND_TIPO { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string ND_PROBLEMA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ND_PROBLEMA { get; set; }

        public decimal? VL_DIFERENCIA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ACEPTADO { get; set; }

        public int? CD_FUNCIONARIO_CREA_PROBLEMA { get; set; }

        [StringLength(30)]
        [Column]
        public string NM_FUNCIONARIO_CREA_PROBLEMA { get; set; }

        public int? CD_FUNCIONARIO_ACEPTA_PROBLEMA { get; set; }

        [StringLength(30)]
        [Column]
        public string NM_FUNCIONARIO_ACEPTA_PROBLEMA { get; set; }

        public DateTime? DT_ACEPTADO { get; set; }

        public DateTime? DT_ADDROR { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public decimal? CD_FAIXA { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
