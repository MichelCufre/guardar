namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REG009_PRODUCTOS")]
    public partial class V_REG009_PRODUCTOS
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

        [Required]
        [StringLength(10)]
        [Column]
        public string CD_UNIDADE_MEDIDA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_UNIDADE_MEDIDA { get; set; }

        [Required]
        [StringLength(2)]
        [Column]
        public string CD_CLASSE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CLASSE { get; set; }

        public int CD_FAMILIA_PRODUTO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_FAMILIA_PRODUTO { get; set; }

        public short? CD_ROTATIVIDADE { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_ROTATIVIDADE { get; set; }

        public short CD_SITUACAO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string CD_MERCADOLOGICO { get; set; }

        [StringLength(20)]
        [Column]
        public string DS_REDUZIDA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_NAM { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_NAM { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_MANEJO_FECHA { get; set; }

        public short? QT_DIAS_VALIDADE_LIBERACION { get; set; }

        public short? QT_DIAS_DURACAO { get; set; }

        public short? QT_DIAS_VALIDADE { get; set; }

        public int? QT_ESTOQUE_MINIMO { get; set; }

        public int? QT_ESTOQUE_MAXIMO { get; set; }

        public decimal? PS_BRUTO { get; set; }

        public decimal? PS_LIQUIDO { get; set; }

        [StringLength(11)]
        [Column]
        public string CD_NIVEL { get; set; }

        public decimal? VL_ALTURA { get; set; }

        public decimal? VL_LARGURA { get; set; }

        public decimal? VL_PROFUNDIDADE { get; set; }

        public decimal? VL_CUBAGEM { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_CROSS_DOCKING { get; set; }

        public DateTime? DT_SITUACAO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public decimal? FT_CONVERSAO { get; set; }

        [StringLength(13)]
        [Column]
        public string SG_PRODUTO { get; set; }

        public short? TP_PESO_PRODUTO { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string ID_MANEJO_IDENTIFICADOR { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO4 { get; set; }

        [StringLength(18)]
        [Column]
        public string DS_ANEXO5 { get; set; }
        [StringLength(1)]
        [Column]
        public string TP_DISPLAY { get; set; }

        public decimal? VL_AVISO_AJUSTE { get; set; }

        public decimal QT_UND_DISTRIBUCION { get; set; }

        public short? QT_SUBBULTO { get; set; }

        public decimal? VL_CUSTO_ULT_ENT { get; set; }

        public decimal? VL_PRECO_VENDA { get; set; }

        public short? CD_EXCLUSIVO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_HELP_COLECTOR { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_GRUPO_CONSULTA { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_FACTURACION_COMP1 { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_FACTURACION_COMP2 { get; set; }

        public decimal QT_UND_BULTO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ACEPTA_DECIMALES { get; set; }

        [StringLength(10)]
        [Column]
        public string ND_MODALIDAD_INGRESO_LOTE { get; set; }

        public short CD_RAMO_PRODUTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_RAMO_PRODUTO { get; set; }

        public decimal? QT_GENERICO { get; set; }

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

        [StringLength(2)]
        [Column]
        public string CD_SUB_CLASSE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_SUB_CLASSE { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_VENTANA_LIBERACION { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_VENTANA_LIBERACION { get; set; }

    }
}
