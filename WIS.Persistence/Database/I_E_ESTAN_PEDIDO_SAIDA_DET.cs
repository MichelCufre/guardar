namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("I_E_ESTAN_PEDIDO_SAIDA_DET")]
    public partial class I_E_ESTAN_PEDIDO_SAIDA_DET
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

        [StringLength(40)]
        [Column]
        public string NU_PEDIDO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AGRUPACION { get; set; }

        [StringLength(20)]
        [Column]
        public string QT_PEDIDO { get; set; }

        [StringLength(20)]
        [Column]
        public string DT_ADDROW { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_MEMO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_EMPRESA { get; set; }

        public DateTime? DT_ADDROW_INTERFAZ { get; set; }

        [StringLength(20)]
        [Column]
        public string VL_PORCENTAJE_TOLERANCIA { get; set; }

        [StringLength(20)]
        [Column]
        public string DT_GENERICO_1 { get; set; }

        [StringLength(20)]
        [Column]
        public string NU_GENERICO_1 { get; set; }

        [StringLength(400)]
        [Column]
        public string VL_GENERICO_1 { get; set; }
    }
}
