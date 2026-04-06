namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_SITUACAO")]
    public partial class T_SITUACAO
    {
        public T_SITUACAO()
        {
            this.T_PRDC_INGRESO = new HashSet<T_PRDC_INGRESO>();
            this.T_PRDC_DEFINICION = new HashSet<T_PRDC_DEFINICION>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_SITUACAO { get; set; }

        [Required]
        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string ID_CODIGO_INTERNO_CET { get; set; }

        public DateTime DT_UPDROW { get; set; }

        public DateTime DT_ADDROW { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PRDC_INGRESO> T_PRDC_INGRESO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<T_PRDC_DEFINICION> T_PRDC_DEFINICION { get; set; }
    }
}
