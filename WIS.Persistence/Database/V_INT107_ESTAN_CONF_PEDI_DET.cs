namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_INT107_ESTAN_CONF_PEDI_DET")]
    public partial class V_INT107_ESTAN_CONF_PEDI_DET
    {
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_MEMO { get; set; }

        public DateTime? DT_ADDROW_INTERFAZ { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PROCESADO { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string NU_REGISTRO { get; set; }

        [StringLength(20)]
        [Column]
        public string QT_PRODUTO { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_SERIALIZADO_1 { get; set; }
    }
}
