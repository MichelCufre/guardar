namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRE050_PEND_LIB")]
    public partial class V_PRE050_PEND_LIB
    {
        public short? CD_ONDA { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CLIENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [Required]
        [StringLength(6)]
        [Column]
        public string CD_CONDICION_LIBERACION { get; set; }

        public int? CD_ROTA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_ROTA { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_TIPO_PEDIDO { get; set; }

        [Required]
        [StringLength(6)]
        [Column]
        public string TP_PEDIDO { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_EXPEDICION { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_ZONA { get; set; }

        [StringLength(6)]
        [Column]
        public string CD_GRUPO_EXPEDICION { get; set; }

        public DateTime? DT_LIBERAR_DESDE { get; set; }

        public DateTime? DT_LIBERAR_HASTA { get; set; }

        public DateTime? DT_ENTREGA { get; set; }

        public DateTime? AUX_HR_ENTREGA { get; set; }

        public DateTime? DT_EMITIDO { get; set; }

        public short? NU_ORDEN_LIBERACION { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ENDERECO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO4 { get; set; }

        public int? CD_TRANSPORTADORA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TRANSPORTADORA { get; set; }

        [StringLength(5)]
        [Column]
        public string CD_UF { get; set; }

        [StringLength(25)]
        [Column]
        public string DS_UF { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public int? NU_ULT_PREPARACION { get; set; }

        public long? QT_LINEAS { get; set; }

        public decimal? VL_PESO_TOTAL_BRUTO { get; set; }

        public decimal? VL_VOLUMEN_TOTAL { get; set; }

        public int? QT_PRODUCTOS_SIN_VOLUMEN { get; set; }

        public int? QT_PRODUCTOS_SIN_PESO_BRUTO { get; set; }

        public decimal? QT_UNIDADES { get; set; }

        public decimal? QT_LIBERADO { get; set; }

        public decimal? AUX_VL_VOLUMEN { get; set; }

        public short? AUX_NU_ORDEN_LIBERACION { get; set; }

        public decimal? AUX_NU_ORDEN { get; set; }

        [StringLength(200)]
        [Column]
        public string VL_COMPARTE_CONTENEDOR_PICKING { get; set; }

        public decimal? VL_PORCENTAJE_VIDA_UTIL { get; set; }

        public decimal? QT_PALLET { get; set; }

        public decimal? QT_UND_PEDIDO { get; set; }

        public decimal? QT_UND_PEN_CAMION { get; set; }

        public decimal? QT_PALL_PEN_CAMION { get; set; }
    }
}
