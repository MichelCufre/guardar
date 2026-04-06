namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_EVENTO_TEMPLATE")]
    public partial class T_EVENTO_TEMPLATE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_EVENTO_TEMPLATE()
        {
            T_EVENTO_INSTANCIA = new HashSet<T_EVENTO_INSTANCIA>();
        }

        [Key]
        [Column(Order = 0)]
        [StringLength(15)]
        public string CD_LABEL_ESTILO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(60)]
        public string TP_NOTIFICACION { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_EVENTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_LABEL_ESTILO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_HTML { get; set; }

        public byte[] VL_CUERPO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        [StringLength(70)]
        [Column]
        public string VL_ASUNTO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public virtual ICollection<T_EVENTO_INSTANCIA> T_EVENTO_INSTANCIA { get; set; }
    
    }
}
