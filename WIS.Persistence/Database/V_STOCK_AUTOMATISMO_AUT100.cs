
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_STOCK_AUTOMATISMO_AUT100")]
    public partial class V_STOCK_AUTOMATISMO_AUT100
    {

        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 3)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [StringLength(1)]
        public string ID_AVERIA { get; set; }

        [StringLength(1)]
        public string ID_CTRL_CALIDAD { get; set; }

        [StringLength(1)]
        public string ID_INVENTARIO { get; set; }

        public decimal? QT_ESTOQUE { get; set; }

        public decimal? QT_RESERVA_SAIDA { get; set; }

        public decimal? QT_TRANSITO_ENTRADA { get; set; }

        public short CD_TIPO_ENDERECO { get; set; }

        public short CD_AREA_ARMAZ { get; set; }

        [StringLength(15)]
        public string DS_AREA_ARMAZ { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [Required]
        [StringLength(2)]
        public string CD_CLASSE { get; set; }

        public short? CD_ROTATIVIDADE { get; set; }

        public int CD_FAMILIA_PRODUTO { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_MERCADOLOGICO { get; set; }

        [StringLength(1)]
        public string TP_MANEJO_FECHA { get; set; }

        public DateTime? DT_VENCIMIENTO { get; set; }

        public DateTime? DT_VALIDADE { get; set; }

        public short CD_SITUACAO { get; set; }

        [StringLength(20)]
        public string DS_REDUZIDA { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO_EMPRESA { get; set; }

        [Required]
        [StringLength(10)]
        public string CD_UNIDADE_MEDIDA { get; set; }

        [StringLength(30)]
        public string DS_UNIDADE_MEDIDA { get; set; }

        [Required]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(100)]
        public string DS_FAMILIA_PRODUTO { get; set; }

        public decimal QT_UND_BULTO { get; set; }

        public decimal? QT_CONV_TOTAL { get; set; }

        [StringLength(200)]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO4 { get; set; }

        [StringLength(18)]
        public string DS_ANEXO5 { get; set; }

        public decimal? QT_VENTA_TOTAL { get; set; }

        public decimal? VL_PRECO_VENDA { get; set; }

        public decimal? PS_BRUTO { get; set; }

        public decimal? QT_PESO_TOTAL { get; set; }

        public decimal? VL_CUBAGEM { get; set; }

        public decimal? QT_CUBAGE_TOTAL { get; set; }

        [StringLength(50)]
        public string NU_DOMINIO { get; set; }

        [StringLength(100)]
        public string DS_DOMINIO_VALOR { get; set; }

        public short CD_RAMO_PRODUTO { get; set; }

        [StringLength(200)]
        public string DS_RAMO_PRODUTO { get; set; }

        [Required]
        [StringLength(20)]
        public string CD_ZONA_UBICACION { get; set; }

        [StringLength(100)]
        public string DS_ZONA_UBICACION { get; set; }

        public decimal? QT_GENERICO { get; set; }

        [Required]
        [StringLength(40)]
        public string ND_TIPO_ENDERECO { get; set; }

    }
}