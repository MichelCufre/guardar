namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_ROTATIVIDADE")]
    public partial class T_ROTATIVIDADE
    {
        public T_ROTATIVIDADE()
        {
            T_PRODUTO = new HashSet<T_PRODUTO>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_ROTATIVIDADE { get; set; }

        [Required]
        [StringLength(30)]
        [Column]
        public string DS_ROTATIVIDADE { get; set; }

        public short QT_MAX_DIAS_ESTOCAGEM { get; set; }

        public DateTime DT_UPDROW { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public virtual ICollection<T_PRODUTO> T_PRODUTO { get; set; }
    }
}
