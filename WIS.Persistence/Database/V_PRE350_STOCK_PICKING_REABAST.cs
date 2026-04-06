namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_PRE350_STOCK_PICKING_REABAST")]
    public partial class V_PRE350_STOCK_PICKING_REABAST
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal CD_FAIXA { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string NU_PREDIO_PI { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string CD_ENDERECO_PI { get; set; }

        public int? QT_MINIMO_PI { get; set; }

        public int? QT_MAXIMO_PI { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal QT_PADRAO_PI { get; set; }

        public int? QT_DESBORDE_PI { get; set; }

        [StringLength(120)]
        [Column]
        public string NU_IDENTIFICADOR_PI { get; set; }

        public decimal? QT_FISICO_PI { get; set; }

        public decimal? QT_ENTRADA_PI { get; set; }

        public decimal? QT_SALIDA_PI { get; set; }

        public decimal? QT_DISPONIBLE_PI { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_NECESITA_URGENTE { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_NECESITA_MINIMO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_NECESITA_FORZADO { get; set; }

        public decimal? QT_DISPONIBLE_STOCK { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_PORCENTAJE_FORZADO { get; set; }

        [Required]
        [StringLength(20)]
        public string CD_ZONA_UBICACION { get; set; }

        [StringLength(100)]
        public string DS_ZONA_UBICACION { get; set; }

    }
}
