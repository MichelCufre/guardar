namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRE351_STOCK_PICKING_REABAST")]
    public partial class V_PRE351_STOCK_PICKING_REABAST
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string CD_ENDERECO_PICKING { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_SIRVE_PARA_PI { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [StringLength(120)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_FISICO { get; set; }

        public decimal? QT_SALIDA { get; set; }

        public decimal? QT_ENTRADA { get; set; }

        public decimal? QT_LIBRE { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AVERIA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_INVENTARIO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_CTRL_CALIDAD { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string ID_ENDERECO_BAIXO { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
