namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("I_E_ESTAN_FACTURA_REC")]
    public partial class I_E_ESTAN_FACTURA_REC
    {
        [StringLength(1)]
        [Column]
        public string ID_PROCESADO { get; set; }
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }
        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string NU_REGISTRO { get; set; }

        [StringLength(12)]
        [Column]
        public string NU_FACTURA { get; set; }

        [StringLength(12)]
        [Column]
        public string TP_FACTURA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_EMPRESA { get; set; }

        [StringLength(20)]
        [Column]
        public string DT_EMISION { get; set; }

        [StringLength(16)]
        [Column]
        public string IM_TOTAL_DIGITADO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ORIGEN { get; set; }

        public DateTime? DT_ADDROW_INTERFAZ { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(6)]
        [Column]
        public string CD_MONEDA { get; set; }

        [StringLength(20)]
        [Column]
        public string DT_VENCIMIENTO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_GLN { get; set; }
    }
}
