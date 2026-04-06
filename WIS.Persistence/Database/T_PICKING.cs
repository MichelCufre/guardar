namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PICKING")]
    public partial class T_PICKING
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_PICKING()
        {
            T_CONTENEDOR = new HashSet<T_CONTENEDOR>();
            T_DET_PICKING = new HashSet<T_DET_PICKING>();
            T_DOCUMENTO_LIBERACION = new HashSet<T_DOCUMENTO_LIBERACION>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_PREPARACION { get; set; }

        public DateTime? DT_INICIO { get; set; }

        public DateTime? DT_FIN { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AVISO { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_PREPARACION { get; set; }

        public short? CD_SITUACAO { get; set; }

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

        public decimal? QT_RECHAZOS { get; set; }

        public short? CD_DESTINO { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_DOCUMENTO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_DOCUMENTO { get; set; }

        public int? CD_FUNCIONARIO_ASIGNADO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AGRUPACION { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PRIORIZAR_DESBORDE { get; set; }

        public short? VL_PORCENTAJE_VENTANA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_VENTANA_POR_CLIENTE { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_REQUIERE_UBICACION { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PRIORIZAR_LOTE_PICK { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SIMULACRO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_USAR_SOLO_STK_PICKING { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_EXCLUIR_UBICACIONES_PICKING { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PICK_VENCIDO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_VALIDAR_PRODUCTO_PROVEEDOR { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_CONTENEDOR> T_CONTENEDOR { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DET_PICKING> T_DET_PICKING { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DOCUMENTO_LIBERACION> T_DOCUMENTO_LIBERACION { get; set; }
    }
}
