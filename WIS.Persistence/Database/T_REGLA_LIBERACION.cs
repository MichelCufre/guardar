namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_REGLA_LIBERACION")]
    public partial class T_REGLA_LIBERACION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_REGLA_LIBERACION()
        {
            T_REGLA_CLIENTES = new HashSet<T_REGLA_CLIENTES>();
            T_REGLA_CONDICION_LIBERACION = new HashSet<T_REGLA_CONDICION_LIBERACION>();
            T_REGLA_TIPO_EXPEDICION = new HashSet<T_REGLA_TIPO_EXPEDICION>();
            T_REGLA_TIPO_PEDIDO = new HashSet<T_REGLA_TIPO_PEDIDO>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_REGLA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_REGLA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ACTIVE { get; set; }

        public short? NU_ORDEN { get; set; }

        public DateTime? DT_INICIO { get; set; }

        public DateTime? DT_FIN { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_DIAS { get; set; }

        public long? HR_INICIO { get; set; }

        public long? HR_FIN { get; set; }

        public int? NU_FRECUENCIA { get; set; }

        [StringLength(50)]
        [Column]
        public string TP_FRECUENCIA { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_PALLET_COMPLETO { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_STOCK { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_CONTROLA_STOCK { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_PALLET_INCOMPLETO { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_ORDEN_PEDIDOS { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_RESPETAR_FIFO { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_PREPARAR_SOLO_CAMION { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_REPARTIR_ESCASEZ { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_LIBERAR_POR_UNIDAD { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_AGRUPAR_POR_CAMION { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_AGRUPACION { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_LIBERAR_POR_CURVAS { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public int CD_EMPRESA { get; set; }

        public short CD_ONDA { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public DateTime? DT_ULTIMA_EJECUCION { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_ORDEN_PEDIDOS_AUTO { get; set; }

        public short? NU_CLIS_POR_PREPARACION { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SOLO_PEDIDOS_NUEVOS { get; set; }

        public short? VL_PORCENTAJE_VENTANA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_VENTANA_POR_CLIENTE { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PRIORIZAR_DESBORDE { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_RESPETAR_INTERVALO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_EXCLUIR_UBICACIONES_PICKING { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_USAR_SOLO_STK_PICKING { get; set; }

        public short? NU_DIAS_COLA_TRABAJO { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_REGLA_CLIENTES> T_REGLA_CLIENTES { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_REGLA_CONDICION_LIBERACION> T_REGLA_CONDICION_LIBERACION { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_REGLA_TIPO_EXPEDICION> T_REGLA_TIPO_EXPEDICION { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_REGLA_TIPO_PEDIDO> T_REGLA_TIPO_PEDIDO { get; set; }
    }
}
