namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DET_ETIQUETA_LOTE")]
    public partial class T_DET_ETIQUETA_LOTE
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_ETIQUETA_LOTE { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_PRODUTO_RECIBIDO { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        public decimal? QT_AJUSTE_RECIBIDO { get; set; }

        public decimal? QT_ETIQUETA_GENERADA { get; set; }

        public decimal? QT_ALMACENADO { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public decimal? QT_RASTREO_PALLET { get; set; }

        public decimal? QT_MOVILIZADO { get; set; }

        public DateTime? DT_ENTRADA { get; set; }

        public decimal? PS_PRODUTO_RECIBIDO { get; set; }

        public decimal? PS_PRODUTO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_MOTIVO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public virtual T_ETIQUETA_LOTE T_ETIQUETA_LOTE { get; set; }

        public virtual T_PRODUTO T_PRODUTO { get; set; }
    }
}
