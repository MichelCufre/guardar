namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_CONTENEDOR")]
    public partial class T_CONTENEDOR
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_CONTENEDOR()
        {
            T_DET_PICKING = new HashSet<T_DET_PICKING>();
        }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_CONTENEDOR { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string TP_CONTENEDOR { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        [StringLength(2)]
        [Column]
        public string CD_SUB_CLASSE { get; set; }

        public short? CD_PORTA { get; set; }

        public int? CD_CAMION { get; set; }

        public DateTime? DT_PULMON { get; set; }

        public DateTime? DT_EXPEDIDO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public int? CD_FUNCIONARIO_EXPEDICION { get; set; }

        public decimal? PS_REAL { get; set; }

        public decimal? VL_ALTURA { get; set; }

        public decimal? VL_LARGURA { get; set; }

        public decimal? VL_PROFUNDIDADE { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_UNIDAD_BULTO { get; set; }

        public int? QT_BULTO { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_CONTENEDOR { get; set; }

        public int? CD_CAMION_CONGELADO { get; set; }

        public int? NU_UNIDAD_TRANSPORTE { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGRUPADOR { get; set; }

        public int? NU_VIAJE { get; set; }

        public short? CD_CANAL { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_CONTENEDOR_EMPAQUE { get; set; }

        public int? CD_CAMION_FACTURADO { get; set; }

        [StringLength(1)]
        [Column]
        public string TP_CONTROL { get; set; }

        public decimal? VL_CUBAGEM { get; set; }

        [StringLength(20)]
        [Column]
        public string ID_PRECINTO_1 { get; set; }

        [StringLength(20)]
        [Column]
        public string ID_PRECINTO_2 { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_HABILITADO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SEPARADO_DOS_FASES { get; set; }

        [StringLength(1)]
        [Column]
        public string VL_CONTROL { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

		[StringLength(50)]
		[Column]
		public string ID_EXTERNO_CONTENEDOR { get; set; }

        public long? NU_LPN { get; set; }

        public long? ID_EXTERNO_TRACKING { get; set; }

        [StringLength(100)]
        [Column]
        public string CD_BARRAS { get; set; }

        public virtual T_PICKING T_PICKING { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_DET_PICKING> T_DET_PICKING { get; set; }
    }
}
