namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRE052_PICKING")]
    public partial class V_PRE052_PICKING
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_PREPARACION { get; set; }

        public DateTime? DT_INICIO { get; set; }

        public DateTime? DT_FIN { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        [StringLength(100)]
        [Column]
        public string FULLNAME { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AVISO { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_PREPARACION { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_DOMINIO_VALOR { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        [StringLength(6)]
        [Column]
        public string CD_CONTENEDOR_VALIDACION { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PREPARAR_SOLO_CON_CAMION { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PICK_AGRUPADO_POR_CAMION { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_RESPETAR_FIFO_EN_LOTE_AUTO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_MERCADERIA_AVERIADA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_LIBERAR_POR_UNIDADES { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_LIBERAR_POR_CURVAS { get; set; }

        [StringLength(2)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(1)]
        [Column]
        public string VL_CURSOR_PEDIDO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CONTROLA_STOCK_DOCUMENTO { get; set; }

        [StringLength(30)]
        [Column]
        public string VL_CURSOR_STOCK { get; set; }

        [StringLength(5)]
        [Column]
        public string FL_MODAL_PALLET_COMPLETO { get; set; }

        [StringLength(5)]
        [Column]
        public string FL_MODAL_PALLET_INCOMPLETO { get; set; }

        [StringLength(1)]
        [Column]
        public string VL_REPARTIR_ESCASEZ { get; set; }

        public decimal? VL_PORCENTAJE_REPARTO_COMUN { get; set; }

        public short? CD_ONDA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_ONDA { get; set; }

        public decimal? QT_RECHAZOS { get; set; }

        public short? CD_DESTINO { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_DOCUMENTO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_DOCUMENTO { get; set; }

        public int? CD_FUNCIONARIO_ASIGNADO { get; set; }

        [StringLength(100)]
        [Column]
        public string FUNC_ASOCIADO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AGRUPACION { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_VENTANA_POR_CLIENTE { get; set; }

        public short? VL_PORCENTAJE_VENTANA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PRIORIZAR_DESBORDE { get; set; }
        [StringLength(1)]
        [Column]
        public string FL_REQUIERE_UBICACION { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_EXCLUIR_UBICACIONES_PICKING { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PICK_VENCIDO { get; set; }


        [StringLength(1)]
        [Column]
        public string FL_VALIDAR_PRODUCTO_PROVEEDOR { get; set; }
    }
}
