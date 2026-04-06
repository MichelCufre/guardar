namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_FAMILIA_PRODUTO")]
    public partial class T_FAMILIA_PRODUTO
    {
        public T_FAMILIA_PRODUTO()
        {
            T_PRODUTO = new HashSet<T_PRODUTO>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_FAMILIA_PRODUTO { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_FAMILIA_PRODUTO { get; set; }

        public DateTime DT_UPDROW { get; set; }

        public DateTime DT_ADDROW { get; set; }


        public virtual ICollection<T_PRODUTO> T_PRODUTO { get; set; }
    }
}
