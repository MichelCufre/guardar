namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_INT108_ESTAN_CONVERTEDOR")]
    public partial class V_INT108_ESTAN_CONVERTEDOR
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_REGISTRO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_EXTERNO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        public DateTime? DT_ADDROW_INTERFAZ { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PROCESADO { get; set; }

        public int NU_REGISTRO_PADRE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_CARGA { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string TP_OPERACION { get; set; }
    }
}
