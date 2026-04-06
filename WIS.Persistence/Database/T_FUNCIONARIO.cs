namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FUNCIONARIO")]
    public partial class T_FUNCIONARIO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_FUNCIONARIO { get; set; }

        public short? CD_ATIVIDADE { get; set; }

        [Required]
        [StringLength(30)]
        [Column]
        public string NM_FUNCIONARIO { get; set; }

        [Required]
        [StringLength(15)]
        [Column]
        public string NM_ABREV_FUNC { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string DS_FUNCAO { get; set; }

        public short QT_CARGA_HORARIA { get; set; }

        public short CD_SITUACAO { get; set; }

        public DateTime DT_UPDROW { get; set; }

        public DateTime DT_ADDROW { get; set; }

        [StringLength(15)]
        [Column]
        public string CD_OPID { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_USER_UNIX { get; set; }

        [Required]
        [StringLength(5)]
        [Column]
        public string CD_IDIOMA { get; set; }

        public int? NU_ORDEN_TRABAJO { get; set; }

        [StringLength(20)]
        [Column]
        public string NU_IP_COLECTOR { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DIR_ARCHIVOS_EXCEL { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_PTS { get; set; }

        [StringLength(200)]
        [Column]
        public string CD_USER_ORCLE { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_EMAIL { get; set; }

        public int? CD_EQUIPO { get; set; }
    }
}
