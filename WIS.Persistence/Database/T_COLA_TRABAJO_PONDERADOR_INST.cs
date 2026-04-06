namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_COLA_TRABAJO_PONDERADOR_INST")]
    public partial class T_COLA_TRABAJO_PONDERADOR_INST
    {
        [Key]
        [StringLength(100)]
        public string CD_PONDERADOR { get; set; }

        [StringLength(200)]
        public string DS_PONDERADOR { get; set; }

        public int? NU_INCREMENTO_DEFAULT { get; set; }

        public int? NU_PONDERACION_DEFAULT { get; set; }

        [StringLength(5)]
        public string TP_DATO { get; set; }

        [StringLength(1)]
        public string FL_HABILITADO_DEFAULT { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}

