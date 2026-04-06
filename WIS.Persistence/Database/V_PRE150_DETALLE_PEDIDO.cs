using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_PRE150_DETALLE_PEDIDO")]
    public partial class V_PRE150_DETALLE_PEDIDO
    {
        [Key]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(40)]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        public string DS_AGENTE { get; set; }

        [StringLength(3)]
        public string TP_AGENTE { get; set; }

        [StringLength(100)]
        public string DS_TIPO_AGENTE { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [Key]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [Key]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [StringLength(1)]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        public decimal? QT_PEDIDO { get; set; }

        public decimal? QT_LIBERADO { get; set; }

        public decimal? QT_ANULADO { get; set; }

        public decimal? QT_PENDIENTE { get; set; }

        public decimal? QT_PENDIENTE_PREP { get; set; }

        public decimal? QT_EXPEDIDO { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO_EMPRESA { get; set; }

        [StringLength(20)]
        public string DS_REDUZIDA { get; set; }

        [StringLength(20)]
        public string CD_MERCADOLOGICO { get; set; }

        public DateTime? DT_LIBERAR_DESDE { get; set; }

        public DateTime? DT_LIBERAR_HASTA { get; set; }

        public int? NU_ULT_PREPARACION { get; set; }

        public DateTime? DT_ULT_PREPARACION { get; set; }

        public DateTime? DT_ENTREGA { get; set; }

        [StringLength(1)]
        public string ID_MANUAL { get; set; }

        public DateTime? DT_EMITIDO { get; set; }

        public int? CD_ROTA { get; set; }

        [StringLength(30)]
        public string DS_ROTA { get; set; }

        public decimal? QT_PEDIDO_ORIGINAL { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        public int? NU_PREPARACION_MANUAL { get; set; }

        public decimal? QT_PREPARADO { get; set; }

        public decimal? QT_FACTURADO { get; set; }

        [StringLength(200)]
        public string DS_MEMO { get; set; }

        [StringLength(1000)]
        public string DS_MEMO_PEDIDO { get; set; }

        [StringLength(1000)]
        public string DS_MEMO_1 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO4 { get; set; }

        public DateTime? DT_GENERICO_1 { get; set; }

        public decimal? NU_GENERICO_1 { get; set; }

        [StringLength(400)]
        public string VL_GENERICO_1 { get; set; }

        public int CD_FAMILIA_PRODUTO { get; set; }

        [StringLength(100)]
        public string DS_FAMILIA_PRODUTO { get; set; }

        [StringLength(6)]
        public string TP_PEDIDO { get; set; }
        [StringLength(60)]
        public string DS_TIPO_PEDIDO { get; set; }

        public short CD_RAMO_PRODUTO { get; set; }

        [StringLength(200)]
        public string DS_RAMO_PRODUTO { get; set; }

        public decimal? PS_BRUTO { get; set; }

        public decimal? PS_LIQUIDO { get; set; }

        public decimal? VL_CUBAGEM { get; set; }

        [StringLength(20)]
        public string ND_ACTIVIDAD { get; set; }

        [StringLength(1)]
        public string ID_AGRUPACION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
        
        [StringLength(4000)]
        public string VL_SERIALIZADO_1 { get; set; }
        
        public decimal? VL_PORCENTAJE_TOLERANCIA { get; set; }

        public decimal? QT_ANULADO_FACTURA { get; set; }

        public decimal? QT_ABASTECIDO { get; set; }

        public decimal? QT_CONTROLADO { get; set; }

        public decimal? QT_CROSS_DOCK { get; set; }

        public decimal? QT_TRANSFERIDO { get; set; }

        public decimal? QT_CARGADO { get; set; }

        public decimal? QT_UND_ASOCIADO_CAMION { get; set; }

        [StringLength(1)]
        public string FL_SEMIACABADO { get; set; }

        [StringLength(1)]
        public string FL_CONSUMIBLE { get; set; }

    }
}
