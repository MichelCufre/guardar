namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_TIPO_PEDIDO")]
    public partial class T_TIPO_PEDIDO
    {
        [Key]
        [StringLength(6)]
        [Column]
        public string TP_PEDIDO { get; set; }

        [Required]
        [StringLength(60)]
        [Column]
        public string DS_TIPO_PEDIDO { get; set; }

        [Required]
        [StringLength(100)]
        [Column]
        public string DS_MEMO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
