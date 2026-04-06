namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_DET_PEDIDO_SAIDA")]
    public partial class T_DET_PEDIDO_SAIDA
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(1)]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AGRUPACION { get; set; }

        public decimal? QT_PEDIDO { get; set; }

        public decimal? QT_LIBERADO { get; set; }

        public decimal? QT_ANULADO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public decimal? QT_PEDIDO_ORIGINAL { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_MEMO { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_SERIALIZADO_1 { get; set; }

        public decimal? VL_PORCENTAJE_TOLERANCIA { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public decimal? QT_ANULADO_FACTURA { get; set; }

        public decimal? QT_ABASTECIDO { get; set; }

        public decimal? QT_CONTROLADO { get; set; }

        public decimal? QT_CROSS_DOCK { get; set; }

        public decimal? QT_EXPEDIDO { get; set; }

        public decimal? QT_FACTURADO { get; set; }

        public decimal? QT_PREPARADO { get; set; }

        public decimal? QT_TRANSFERIDO { get; set; }

        public DateTime? DT_GENERICO_1 { get; set; }

        public decimal? NU_GENERICO_1 { get; set; }

        [StringLength(400)]
        [Column]
        public string VL_GENERICO_1 { get; set; }

        public decimal? QT_CARGADO { get; set; }

        public decimal? QT_UND_ASOCIADO_CAMION { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SEMIACABADO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CONSUMIBLE { get; set; }

        [StringLength(200)]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO3 { get; set; }

        [StringLength(200)]
        public string DS_ANEXO4 { get; set; }

        public virtual T_PEDIDO_SAIDA T_PEDIDO_SAIDA { get; set; }
    }
}
