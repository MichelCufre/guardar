namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("T_COLA_TRABAJO_PONDERADOR_DET")]
    public partial class T_COLA_TRABAJO_PONDERADOR_DET
    {
        [Key]
        public int NU_COLA_TRABAJO { get; set; }

        [Key]
        [StringLength(100)]
        public string CD_PONDERADOR { get; set; }

        [Key]
        [StringLength(100)]
        public string CD_INST_PONDERADOR { get; set; }

        public int? NU_PONDERACION { get; set; }

        [StringLength(5)]
        public string VL_OPERACION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public virtual T_COLA_TRABAJO T_COLA_TRABAJO { get; set; }

        public virtual T_COLA_TRABAJO_PONDERADOR T_COLA_TRABAJO_PONDERADOR { get; set; }
    }
}