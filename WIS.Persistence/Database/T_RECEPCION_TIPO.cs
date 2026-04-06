namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_RECEPCION_TIPO")]
    public partial class T_RECEPCION_TIPO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_RECEPCION_TIPO()
        {
            T_RECEPCION_REL_EMPRESA_TIPO = new HashSet<T_RECEPCION_REL_EMPRESA_TIPO>();
            T_RECEPCION_TIPO_REPORTE_DEF = new HashSet<T_RECEPCION_TIPO_REPORTE_DEF>();

        }

        [Key]
        [StringLength(6)]
        [Column]
        public string TP_RECEPCION { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_TIPO_RECEPCION { get; set; }

        [StringLength(5)]
        [Column]
        public string TP_MANEJO_NU_DOCUMENTO { get; set; }

        [StringLength(3)]
        [Column]
        public string FL_OC_SIN_SALDO_AGENDA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ESPECIFICAR_PREDIO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CARGA_AUTO_DETALLE { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_DIGITACION_HABILITADA { get; set; }

        [StringLength(1)]
        [Column]
        public string VL_CIERRE_AGENDA { get; set; }

        [StringLength(1)]
        [Column]
        public string VL_CROSS_DOCKING { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PRODUCTOS_ACTIVOS { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PRODUCTOS_NO_ESPERADOS { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_RECIBIR_LOTES_NO_ESPERADOS { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CONTROLAR_VENCIMIENTO { get; set; }

        [StringLength(1)]
        [Column]
        public string VL_ETIQUETAS_RECEPCION { get; set; }

        [StringLength(1)]
        [Column]
        public string VL_ESPECIFICAR_LOTES { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_AGENDAR_HORARIO_PUERTA { get; set; }

        [StringLength(1)]
        [Column]
        public string VL_ESTADO_ETIQUETA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PERMITIR_ALMACENAR_AVERIA { get; set; }

        [StringLength(20)]
        [Column]
        public string VL_INTERFAZ_EN_CIERRE_AGENDA { get; set; }

        [StringLength(3)]
        [Column]
        public string FL_OC_SIN_SALDO_RECEPCION { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CANCELAR_SALDO_AL_CIERRE { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_REFERENCIA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_INGRESO_FACTURA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ACEPTA_QT_MAYOR_A_AGENDADO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PERMITE_AUTO_RECEPCION { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_MANIPULEO_TAREA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_MONO_REFERENCIA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CTRL_PESO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CTRL_VOLUMEN { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CTRL_VENCIMIENTO { get; set; }

        [StringLength(20)]
        [Column]
        public string TP_SELECCION_REFERENCIA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_HABILITAR_EMPRESA_DEFAULT { get; set; }
        [StringLength(1)]
        [Column]
        public string FL_MOTIVO_REQUERIDO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PERMITE_PLANIFICAR_LPN { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PERMITE_RECIBIR_LPN { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PERMITE_LPN_NO_ESPERADO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_RECEPCION_REL_EMPRESA_TIPO> T_RECEPCION_REL_EMPRESA_TIPO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_RECEPCION_TIPO_REPORTE_DEF> T_RECEPCION_TIPO_REPORTE_DEF { get; set; }



    }
}
