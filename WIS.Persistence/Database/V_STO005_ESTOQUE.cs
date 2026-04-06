namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_STO005_ESTOQUE")]
    public partial class V_STO005_ESTOQUE
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [Required]
        [StringLength(2)]
        [Column]
        public string CD_CLASSE { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_CLASSE { get; set; }

        public int CD_FAMILIA_PRODUTO { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_FAMILIA_PRODUTO { get; set; }

        public short CD_SITUACAO { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string CD_MERCADOLOGICO { get; set; }

        public short? CD_ROTATIVIDADE { get; set; }

        [StringLength(20)]
        [Column]
        public string DS_REDUZIDA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        public DateTime? DT_INVENTARIO { get; set; }

        public decimal? QT_ESTOQUE { get; set; }

        public decimal? QT_RESERVA_SAIDA { get; set; }

        public decimal? QT_TRANSITO_ENTRADA { get; set; }
        public decimal? QT_LPN { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_CTRL_CALIDAD { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AVERIA { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public short? CD_AREA_ARMAZ { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AREA_AVARIA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_DISP_ESTOQUE { get; set; }

        [StringLength(40)]
        [Column]
        public string CODIGO_BASE { get; set; }

        [StringLength(40)]
        [Column]
        public string TALLE { get; set; }

        [StringLength(40)]
        [Column]
        public string COLOR { get; set; }

        [StringLength(40)]
        [Column]
        public string TEMPORADA { get; set; }

        [StringLength(40)]
        [Column]
        public string VL_CATEGORIA_01 { get; set; }

        [StringLength(40)]
        [Column]
        public string VL_CATEGORIA_02 { get; set; }
    }
}
