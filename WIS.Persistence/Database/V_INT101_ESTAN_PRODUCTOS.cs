namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_INT101_ESTAN_PRODUCTOS")]
    public partial class V_INT101_ESTAN_PRODUCTOS
    {
        [StringLength(2)]
        [Column]
        public string CD_CLASSE { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(4)]
        [Column]
        public string CD_EXCLUSIVO { get; set; }

        [StringLength(4)]
        [Column]
        public string CD_FAMILIA_PRODUTO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_GRUPO_CONSULTA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_MERCADOLOGICO { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_NAM { get; set; }

        [StringLength(11)]
        [Column]
        public string CD_NIVEL { get; set; }

        [StringLength(1)]
        [Column]
        public string CD_ORIGEM { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }

        [StringLength(15)]
        [Column]
        public string CD_PRODUTO_UNICO { get; set; }

        [StringLength(2)]
        [Column]
        public string CD_RAMO_PRODUTO { get; set; }

        [StringLength(2)]
        [Column]
        public string CD_ROTATIVIDADE { get; set; }

        [StringLength(3)]
        [Column]
        public string CD_SITUACAO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_UND_MEDIDA_FACT { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_UNIDADE_MEDIDA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_UNID_EMB { get; set; }

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

        [StringLength(35)]
        [Column]
        public string DS_CLASSE { get; set; }

        [StringLength(4)]
        [Column]
        public string DS_DIFER_PESO_QTDE { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_DISPLAY { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_FAMILIA_PRODUTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_HELP_COLECTOR { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_RAMO_PRODUTO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_REDUZIDA { get; set; }

        [StringLength(10)]
        [Column]
        public string DT_ADDROW { get; set; }

        public DateTime? DT_ADDROW_INTERFAZ { get; set; }

        [StringLength(10)]
        [Column]
        public string DT_SITUACAO { get; set; }

        [StringLength(10)]
        [Column]
        public string DT_UPDROW { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ACEPTA_DECIMALES { get; set; }

        [StringLength(13)]
        [Column]
        public string FT_CONVERSAO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AGRUPACION { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_CROSS_DOCKING { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_MANEJA_TOMA_DATO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_MANEJO_IDENTIFICADOR { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_PROCESADO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_REDONDEO_VALIDEZ { get; set; }

        [StringLength(8)]
        [Column]
        public string ND_MODALIDAD_INGRESO_LOTE { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_REGISTRO { get; set; }

        [StringLength(13)]
        [Column]
        public string PS_BRUTO { get; set; }

        [StringLength(13)]
        [Column]
        public string PS_LIQUIDO { get; set; }

        [StringLength(4)]
        [Column]
        public string QT_DIAS_DURACAO { get; set; }

        [StringLength(4)]
        [Column]
        public string QT_DIAS_VALIDADE { get; set; }

        [StringLength(4)]
        [Column]
        public string QT_DIAS_VALIDADE_LIBERACION { get; set; }

        [StringLength(9)]
        [Column]
        public string QT_ESTOQUE_MAXIMO { get; set; }

        [StringLength(9)]
        [Column]
        public string QT_ESTOQUE_MINIMO { get; set; }

        [StringLength(3)]
        [Column]
        public string QT_SUBBULTO { get; set; }

        [StringLength(10)]
        [Column]
        public string QT_UND_BULTO { get; set; }

        [StringLength(10)]
        [Column]
        public string QT_UND_DISTRIBUCION { get; set; }

        [StringLength(13)]
        [Column]
        public string SG_PRODUTO { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_CARGA { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_DISPLAY { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_MANEJO_FECHA { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_PESO_PRODUTO { get; set; }

        [StringLength(11)]
        [Column]
        public string VL_ALTURA { get; set; }

        [StringLength(15)]
        [Column]
        public string VL_AVISO_AJUSTE { get; set; }

        [StringLength(15)]
        [Column]
        public string VL_CUBAGEM { get; set; }

        [StringLength(17)]
        [Column]
        public string VL_CUSTO_ULT_ENT { get; set; }

        [StringLength(11)]
        [Column]
        public string VL_LARGURA { get; set; }

        [StringLength(11)]
        [Column]
        public string VL_PRECIO_DISTRIB { get; set; }

        [StringLength(11)]
        [Column]
        public string VL_PRECIO_EGRESO { get; set; }

        [StringLength(11)]
        [Column]
        public string VL_PRECIO_INGRESO { get; set; }

        [StringLength(11)]
        [Column]
        public string VL_PRECIO_SEG_DISTR { get; set; }

        [StringLength(11)]
        [Column]
        public string VL_PRECIO_SEG_STOCK { get; set; }

        [StringLength(11)]
        [Column]
        public string VL_PRECIO_STOCK { get; set; }

        [StringLength(17)]
        [Column]
        public string VL_PRECO_VENDA { get; set; }

        [StringLength(11)]
        [Column]
        public string VL_PROFUNDIDADE { get; set; }
    }
}
