namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_ROTA")]
    public partial class T_ROTA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_ROTA()
        {
            T_CLIENTE = new HashSet<T_CLIENTE>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_ROTA { get; set; }

        public short? CD_SITUACAO { get; set; }

        public short? CD_ONDA { get; set; }

        public int? CD_TRANSPORTADORA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_ROTA { get; set; }

        public short? CD_PORTA { get; set; }

        public DateTime? DT_SITUACAO { get; set; }

        public DateTime? DT_ALTERACAO { get; set; }

        public DateTime? DT_CADASTRAMENTO { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ORDEM_CARGA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_ZONA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_CLIENTE> T_CLIENTE { get; set; }
        public virtual ICollection<T_CLIENTE_RUTA_PREDIO> T_CLIENTE_RUTA_PREDIO { get; set; }

        public virtual T_ONDA T_ONDA { get; set; }

        public virtual T_PORTA_EMBARQUE T_PORTA_EMBARQUE { get; set; }
    }
}
