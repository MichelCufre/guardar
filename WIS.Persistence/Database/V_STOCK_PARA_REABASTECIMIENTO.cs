using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_STOCK_PARA_REABASTECIMIENTO")]
    public partial class V_STOCK_PARA_REABASTECIMIENTO
    {

        [Key]
        [Column(Order = 0)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(4000)]
        public string CD_ENDERECO_PI { get; set; }

        public int? QT_MINIMO_PI { get; set; }

        public int? QT_MAXIMO_PI { get; set; }

        [Key]
        [Column(Order = 8)]
        public decimal QT_PADRAO_PI { get; set; }

        public int? QT_DESBORDE_PI { get; set; }

        public decimal? QT_STOCK_PI { get; set; }

        public decimal? QT_TRANSITO_ENTRADA_PI { get; set; }

        public decimal? QT_RESERVA_SAIDA_PI { get; set; }

        [Key]
        [Column(Order = 13)]
        [StringLength(40)]
        public string CD_ENDERECO_ST { get; set; }

        [Key]
        [Column(Order = 14)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR_ST { get; set; }

        [StringLength(160)]
        public string NU_IDENTIFICADOR_PI { get; set; }

        [StringLength(1)]
        public string ID_MENOS_MINIMO_PI { get; set; }

        [StringLength(1)]
        public string ID_MENOS_PEDIDO_PI { get; set; }

        [StringLength(1)]
        public string ID_SIRVE_PARA_PI { get; set; }

        public decimal? QT_FISICO_ST { get; set; }

        public decimal? QT_SALIDA_ST { get; set; }

        public decimal? QT_ENTRADA_ST { get; set; }

        public decimal? QT_LIBRE_ST { get; set; }

        public DateTime? DT_FABRICACAO_ST { get; set; }

        [StringLength(1)]
        public string ID_AVERIA_ST { get; set; }

        [StringLength(1)]
        public string ID_INVENTARIO_ST { get; set; }

        [StringLength(1)]
        public string ID_CTRL_CALIDAD_ST { get; set; }

        [Key]
        [Column(Order = 27)]
        [StringLength(1)]
        public string ID_ENDERECO_BAIXO_ST { get; set; }

        [Required]
        [StringLength(10)]
        public string NU_PREDIO_PI { get; set; }

        [Required]
        [StringLength(10)]
        public string NU_PREDIO_ST { get; set; }

        [StringLength(20)]
        public string CD_ZONA_UBICACION_PI { get; set; }

        [StringLength(20)]
        public string CD_ZONA_UBICACION_ST { get; set; }

    }
}