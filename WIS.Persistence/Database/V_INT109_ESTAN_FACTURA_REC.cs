namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_INT109_ESTAN_FACTURA_REC")]
    public partial class V_INT109_ESTAN_FACTURA_REC
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string NU_REGISTRO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TIPO_AGENTE { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_AGENTE { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_GLN { get; set; }

        [StringLength(6)]
        [Column]
        public string CD_MONEDA { get; set; }

        public DateTime? DT_ADDROW_INTERFAZ { get; set; }

        [StringLength(20)]
        [Column]
        public string DT_EMISION { get; set; }

        [StringLength(20)]
        [Column]
        public string DT_VENCIMIENTO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ORIGEN { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PROCESADO { get; set; }

        [StringLength(16)]
        [Column]
        public string IM_TOTAL_DIGITADO { get; set; }

        [StringLength(12)]
        [Column]
        public string NU_FACTURA { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(12)]
        [Column]
        public string TP_FACTURA { get; set; }

    }
}
