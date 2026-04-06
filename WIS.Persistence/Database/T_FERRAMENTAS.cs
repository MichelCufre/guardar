namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FERRAMENTAS")]
    public partial class T_FERRAMENTAS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public T_FERRAMENTAS()
        {
            T_EQUIPO = new HashSet<T_EQUIPO>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_FERRAMENTA { get; set; }

        [StringLength(40)]
        [Column]
        public string DS_FERRAMENTA { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AUTOASIGNADO { get; set; }

        public short CD_SITUACAO { get; set; }

        public DateTime DT_UPDROW { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public virtual ICollection<T_EQUIPO> T_EQUIPO { get; set; }

    }
}
