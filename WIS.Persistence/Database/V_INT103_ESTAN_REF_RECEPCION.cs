namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_INT103_ESTAN_REF_RECEPCION")]
    public partial class V_INT103_ESTAN_REF_RECEPCION
    {
        [StringLength(30)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(30)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_MEMO { get; set; }

        public DateTime? DT_ADDROW_INTERFAZ { get; set; }

        [StringLength(30)]
        [Column]
        public string DT_EMITIDA { get; set; }

        [StringLength(30)]
        [Column]
        public string DT_ENTREGA { get; set; }

        [StringLength(30)]
        [Column]
        public string FL_AUTO_AGENDABLE { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(30)]
        [Column]
        public string NU_REFERENCIA { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_REGISTRO { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_RECEPCION_EXTERNO { get; set; }

        [StringLength(30)]
        [Column]
        public string TP_REFERENCIA { get; set; }

        [StringLength(1000)]
        [Column]
        public string VL_SERIALIZADO { get; set; }
    }
}
