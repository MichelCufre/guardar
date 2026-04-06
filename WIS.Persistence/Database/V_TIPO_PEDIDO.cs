namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_TIPO_PEDIDO")]
    public partial class V_TIPO_PEDIDO
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(6)]
        public string TP_PEDIDO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(60)]
        public string DS_TIPO_PEDIDO { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string DS_MEMO { get; set; }
    }
}
