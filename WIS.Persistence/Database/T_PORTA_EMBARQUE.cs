namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PORTA_EMBARQUE")]
    public partial class T_PORTA_EMBARQUE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_PORTA_EMBARQUE()
        {
            T_ROTA = new HashSet<T_ROTA>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_PORTA { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_PORTA { get; set; }

        public short? CD_SITUACAO { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_PUERTA { get; set; }

        public virtual T_ENDERECO_ESTOQUE T_ENDERECO_ESTOQUE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_ROTA> T_ROTA { get; set; }
    }
}
