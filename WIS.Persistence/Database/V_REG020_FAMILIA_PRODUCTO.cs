namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_REG020_FAMILIA_PRODUCTO")]
    public partial class V_REG020_FAMILIA_PRODUCTO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_FAMILIA_PRODUTO { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_FAMILIA_PRODUTO { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime DT_UPDROW { get; set; }
    }
}
