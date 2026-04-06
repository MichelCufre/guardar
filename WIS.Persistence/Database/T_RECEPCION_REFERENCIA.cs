namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_RECEPCION_REFERENCIA")]
    public partial class T_RECEPCION_REFERENCIA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_RECEPCION_REFERENCIA()
        {
            T_RECEPCION_REFERENCIA_DET = new HashSet<T_RECEPCION_REFERENCIA_DET>();
            T_RECEPC_AGENDA_REFERENCIA_REL = new HashSet<T_RECEPC_AGENDA_REFERENCIA_REL>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_RECEPCION_REFERENCIA { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string NU_REFERENCIA { get; set; }

        [Required]
        [StringLength(6)]
        [Column]
        public string TP_REFERENCIA { get; set; }

        public int CD_EMPRESA { get; set; }

        public DateTime? DT_ENTREGA { get; set; }

        public DateTime? DT_VENCIMIENTO_ORDEN { get; set; }

        public DateTime? DT_EMITIDA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_MEMO { get; set; }

        [StringLength(200)]
        [Column]
        public string VL_SERIALIZADO { get; set; }

        public int CD_SITUACAO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_INTERFAZ_EJECUCION { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO2 { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO3 { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_ESTADO_REFERENCIA { get; set; }

        [StringLength(6)]
        [Column]
        public string CD_MONEDA { get; set; }

        public virtual T_EMPRESA T_EMPRESA { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_RECEPCION_REFERENCIA_DET> T_RECEPCION_REFERENCIA_DET { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_RECEPC_AGENDA_REFERENCIA_REL> T_RECEPC_AGENDA_REFERENCIA_REL { get; set; }
    }
}
