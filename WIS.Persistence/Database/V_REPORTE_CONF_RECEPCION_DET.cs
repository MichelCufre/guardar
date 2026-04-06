using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_REPORTE_CONF_RECEPCION_DET")]
    public partial class V_REPORTE_CONF_RECEPCION_DET
    {

        [Key]
        [Column(Order = 0)]
        public int NU_AGENDA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 4)]
        public int CD_EMPRESA { get; set; }

        public short? CD_SITUACAO { get; set; }

        public decimal? QT_AGENDADO { get; set; }

        public decimal? QT_CROSS_DOCKING { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        public decimal? VL_PRECIO { get; set; }

        public decimal? QT_RECIBIDA { get; set; }

        public DateTime? DT_ACEPTADA_RECEPCION { get; set; }

        public int? CD_FUNC_ACEPTO_RECEPCION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public decimal? QT_ACEPTADA { get; set; }

        public decimal? QT_AGENDADO_ORIGINAL { get; set; }

        public decimal? QT_RECIBIDA_FICTICIA { get; set; }

        public decimal? VL_CIF { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO_EMPRESA { get; set; }

        [StringLength(20)]
        public string CD_NAM { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_MERCADOLOGICO { get; set; }

        [StringLength(13)]
        public string SG_PRODUTO { get; set; }

        public short? TP_PESO_PRODUTO { get; set; }

        [StringLength(4)]
        public string DS_DIFER_PESO_QTDE { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [Required]
        [StringLength(10)]
        public string CD_UNIDADE_MEDIDA { get; set; }

        public int CD_FAMILIA_PRODUTO { get; set; }

        public short? CD_ROTATIVIDADE { get; set; }

        [Required]
        [StringLength(2)]
        public string CD_CLASSE { get; set; }

        public int? QT_ESTOQUE_MINIMO { get; set; }

        public int? QT_ESTOQUE_MAXIMO { get; set; }

        public decimal? PS_LIQUIDO { get; set; }

        public decimal? PS_BRUTO { get; set; }

        public decimal? FT_CONVERSAO { get; set; }

        public decimal? VL_CUBAGEM { get; set; }

        public decimal? VL_PRECO_VENDA { get; set; }

        public decimal? VL_CUSTO_ULT_ENT { get; set; }

        [StringLength(1)]
        public string CD_ORIGEM { get; set; }

        [StringLength(20)]
        public string DS_REDUZIDA { get; set; }

        [StringLength(11)]
        public string CD_NIVEL { get; set; }

        [StringLength(10)]
        public string CD_UNID_EMB { get; set; }

        public short CD_SITUACAO_PRODUTO { get; set; }

        public DateTime? DT_SITUACAO { get; set; }

        public short? QT_DIAS_VALIDADE { get; set; }

        public short? QT_DIAS_DURACAO { get; set; }

        [StringLength(1)]
        public string ID_CROSS_DOCKING { get; set; }

        [StringLength(1)]
        public string ID_REDONDEO_VALIDEZ { get; set; }

        [StringLength(1)]
        public string ID_AGRUPACION { get; set; }

        [Required]
        [StringLength(1)]
        public string ID_MANEJO_IDENTIFICADOR { get; set; }

        [StringLength(1)]
        public string TP_DISPLAY { get; set; }

        [StringLength(200)]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO4 { get; set; }

        public decimal? VL_ALTURA { get; set; }

        public decimal? VL_LARGURA { get; set; }

        public decimal? VL_PROFUNDIDADE { get; set; }

        [StringLength(1)]
        public string TP_MANEJO_FECHA { get; set; }

        public decimal? VL_AVISO_AJUSTE { get; set; }

        [StringLength(200)]
        public string DS_HELP_COLECTOR { get; set; }

        public short? QT_SUBBULTO { get; set; }

        public short? CD_EXCLUSIVO { get; set; }

        public decimal QT_UND_DISTRIBUCION { get; set; }

        public short? QT_DIAS_VALIDADE_LIBERACION { get; set; }

        public decimal QT_UND_BULTO { get; set; }

        [StringLength(1)]
        public string ID_MANEJA_TOMA_DATO { get; set; }

        [StringLength(18)]
        public string DS_ANEXO5 { get; set; }

        [StringLength(20)]
        public string CD_GRUPO_CONSULTA { get; set; }

        [StringLength(200)]
        public string DS_DISPLAY { get; set; }

        public decimal? VL_PRECIO_SEG_DISTR { get; set; }

        public decimal? VL_PRECIO_SEG_STOCK { get; set; }

        public decimal? VL_PRECIO_DISTRIB { get; set; }

        public decimal? VL_PRECIO_EGRESO { get; set; }

        public decimal? VL_PRECIO_INGRESO { get; set; }

        public decimal? VL_PRECIO_STOCK { get; set; }

        [StringLength(10)]
        public string CD_UND_MEDIDA_FACT { get; set; }

        [StringLength(15)]
        public string CD_PRODUTO_UNICO { get; set; }

        public short CD_RAMO_PRODUTO { get; set; }

        [StringLength(100)]
        public string DS_FAMILIA_PRODUTO { get; set; }

        [Required]
        [StringLength(30)]
        public string DS_UNIDADE_MEDIDA { get; set; }

        [StringLength(30)]
        public string DS_ROTATIVIDADE { get; set; }

        [Required]
        [StringLength(100)]
        public string DS_CLASSE { get; set; }

    }
}
