namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_ENDERECO_ESTOQUE")]
    public partial class T_ENDERECO_ESTOQUE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_ENDERECO_ESTOQUE()
        {
            T_PICKING_PRODUTO = new HashSet<T_PICKING_PRODUTO>();
            T_PORTA_EMBARQUE = new HashSet<T_PORTA_EMBARQUE>();
            T_PRDC_LINEA_ENTRADA = new HashSet<T_PRDC_LINEA>();
            T_PRDC_LINEA_SALIDA = new HashSet<T_PRDC_LINEA>();
            T_PRDC_LINEA_PRODUCCION = new HashSet<T_PRDC_LINEA>();
            T_ETIQUETA_LOTE = new HashSet<T_ETIQUETA_LOTE>();
            T_ETIQUETA_LOTE_SUGERIDO = new HashSet<T_ETIQUETA_LOTE>();
            T_EQUIPO = new HashSet<T_EQUIPO>();
        }

        [Key]
        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        public int CD_EMPRESA { get; set; }

        public short CD_TIPO_ENDERECO { get; set; }

        public short CD_ROTATIVIDADE { get; set; }

        public int CD_FAMILIA_PRINCIPAL { get; set; }

        [Required]
        [StringLength(2)]
        [Column]
        public string CD_CLASSE { get; set; }

        public short CD_SITUACAO { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string ID_ENDERECO_BAIXO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ENDERECO_SEP { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string ID_NECESSIDADE_RESUPRIR { get; set; }

        public DateTime DT_UPDROW { get; set; }

        public DateTime DT_ADDROW { get; set; }

        [StringLength(5)]
        [Column]
        public string CD_CONTROL { get; set; }

        public short CD_AREA_ARMAZ { get; set; }

        [StringLength(4)]
        [Column]
        public string NU_COMPONENTE { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_ZONA_UBICACION { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(10)]
        [Column]
        public string ID_BLOQUE { get; set; }

        [StringLength(10)]
        [Column]
        public string ID_CALLE { get; set; }

        public int? NU_COLUMNA { get; set; }

        public int? NU_ALTURA { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_SECTOR { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_CONTROL_ACCESO { get; set; }

        public int? NU_PROFUNDIDAD { get; set; }

        [StringLength(100)]
        public string CD_BARRAS { get; set; }

        public long? NU_ORDEN_DEFAULT { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PORTA_EMBARQUE> T_PORTA_EMBARQUE { get; set; }

        public virtual T_EMPRESA T_EMPRESA { get; set; }

        public virtual T_ZONA_UBICACION T_ZONA_UBICACION { get; set; }

        public virtual T_CONTROL_ACCESO T_CONTROL_ACCESO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PICKING_PRODUTO> T_PICKING_PRODUTO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PRDC_LINEA> T_PRDC_LINEA_ENTRADA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PRDC_LINEA> T_PRDC_LINEA_SALIDA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        public virtual ICollection<T_PRDC_LINEA> T_PRDC_LINEA_SALIDA_TRAN { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        public virtual ICollection<T_PRDC_LINEA> T_PRDC_LINEA_PRODUCCION { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ETIQUETA_LOTE> T_ETIQUETA_LOTE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ETIQUETA_LOTE> T_ETIQUETA_LOTE_SUGERIDO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_EQUIPO> T_EQUIPO { get; set; }
        public virtual T_TIPO_ENDERECO T_TIPO_ENDERECO { get; set; }

        public virtual T_TIPO_AREA T_TIPO_AREA { get; set; }
    }
}
