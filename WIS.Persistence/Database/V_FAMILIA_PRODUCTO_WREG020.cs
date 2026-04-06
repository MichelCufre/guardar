namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_FAMILIA_PRODUCTO_WREG020")]
    public partial class V_FAMILIA_PRODUCTO_WREG020
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
