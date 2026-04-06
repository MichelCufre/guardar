using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_ENDERECO_ESTOQUE")]
    public partial class V_ENDERECO_ESTOQUE
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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AVERIA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_CTRL_CALIDAD { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_INVENTARIO { get; set; }

        public decimal? QT_ESTOQUE { get; set; }

        public decimal? QT_RESERVA_SAIDA { get; set; }

        public decimal? QT_TRANSITO_ENTRADA { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_TIPO_ENDERECO { get; set; }

        public short? CD_AREA_ARMAZ { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(2)]
        public string CD_CLASSE { get; set; }

        public short? CD_ROTATIVIDADE { get; set; }

        public int CD_FAMILIA_PRODUTO { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(20)]
        public string CD_MERCADOLOGICO { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_MANEJO_FECHA { get; set; }

        public DateTime? DT_VENCIMIENTO { get; set; }

        public DateTime? DT_VALIDADE { get; set; }

        [Key]
        [Column(Order = 9)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_SITUACAO { get; set; }

        [StringLength(20)]
        [Column]
        public string DS_REDUZIDA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }
    }
}
