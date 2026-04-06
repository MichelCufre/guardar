namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_RECEPCION_REFERENCIA_DET")]
    public partial class T_RECEPCION_REFERENCIA_DET
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_RECEPCION_REFERENCIA_DET()
        {
            T_RECEPCION_AGENDA_REFERENCIA = new HashSet<T_RECEPCION_AGENDA_REFERENCIA>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_RECEPCION_REFERENCIA_DET { get; set; }

        public int NU_RECEPCION_REFERENCIA { get; set; }

        [StringLength(40)]
        [Column]
        public string ID_LINEA_SISTEMA_EXTERNO { get; set; }

        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_REFERENCIA { get; set; }

        public decimal? QT_ANULADA { get; set; }

        public decimal? QT_AGENDADA { get; set; }

        public decimal? QT_RECIBIDA { get; set; }

        public decimal? QT_CONFIRMADA_INTERFAZ { get; set; }

        public decimal? IM_UNITARIO { get; set; }

        public DateTime? DT_VENCIMIENTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ANEXO1 { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public virtual T_RECEPCION_REFERENCIA T_RECEPCION_REFERENCIA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_RECEPCION_AGENDA_REFERENCIA> T_RECEPCION_AGENDA_REFERENCIA { get; set; }
    }
}
