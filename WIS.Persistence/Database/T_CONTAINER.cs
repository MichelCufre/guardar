namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_CONTAINER")]
    public partial class T_CONTAINER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_CONTAINER()
        {
            T_RECEPC_AGENDA_CONTAINER_REL = new HashSet<T_RECEPC_AGENDA_CONTAINER_REL>();
        }

        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string NU_CONTAINER { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short NU_SEQ_CONTAINER { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_CONTAINER { get; set; }

        public int? CD_EMPRESA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime? DT_VEN_RET_TERMINAL { get; set; }

        public DateTime? DT_FIN_RET_TERMINAL { get; set; }

        public DateTime? DT_MAX_DEVOLUCION { get; set; }

        public DateTime? DT_INI_DEVOLUCION { get; set; }

        public DateTime? DT_FIN_DEVOLUCION { get; set; }

        public DateTime? DT_FIN_APERTURA { get; set; }

        public short? CD_SITUACAO { get; set; }

        public decimal? PS_TARA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_CONSOLIDADO { get; set; }

        public short? CD_ENDERECO_DEPOSITO { get; set; }

        public int? CD_FUNC_ENCARGADO { get; set; }

        public int? CD_FUNC_VERIFICADOR { get; set; }

        [StringLength(400)]
        [Column]
        public string DS_MEMO { get; set; }

        public DateTime? DT_ENTREGA_DOCUMENTO { get; set; }

        public int? CD_TERMINAL_ENTREGA { get; set; }

        public int? CD_TERMINAL_DEVOLUCION { get; set; }

        public int? CD_TRANSPORTISTA_RETIRO { get; set; }

        public int? CD_TRANSPORTISTA_DEVOLUCION { get; set; }

        [StringLength(40)]
        [Column]
        public string VL_PRECINTO_1 { get; set; }

        [StringLength(40)]
        [Column]
        public string VL_PRECINTO_2 { get; set; }

        [StringLength(400)]
        [Column]
        public string DS_OBSERVACIONES_1 { get; set; }

        [StringLength(400)]
        [Column]
        public string DS_OBSERVACIONES_2 { get; set; }

        [StringLength(40)]
        [Column]
        public string DS_BOOKING { get; set; }

        public DateTime? DT_POSICIONAMIENTO { get; set; }

        public DateTime? DT_APERTURA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_PALETIZADO { get; set; }

        public decimal? CD_FUNCIONARIO_APERTURA { get; set; }

        public decimal? CD_FUNCIONARIO_CIERRE { get; set; }

        public DateTime? DT_PORTERIA_ENTRADA { get; set; }

        public DateTime? DT_PORTERIA_SALIDA { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_RECEPC_AGENDA_CONTAINER_REL> T_RECEPC_AGENDA_CONTAINER_REL { get; set; }
    }
}
