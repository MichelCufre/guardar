namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_CLASIFICACION_STOCK_WSTO640")]
    public partial class V_CLASIFICACION_STOCK_WSTO640
    {

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_LOG_CLASIF_STOCK { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }


        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        public decimal CD_FAIXA { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AVERIA_DESP { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_MOTIVO_AVERIA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOMINIO_VALOR { get; set; }
    }
}
