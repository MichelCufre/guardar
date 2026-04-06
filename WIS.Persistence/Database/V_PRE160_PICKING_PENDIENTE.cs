namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRE160_PICKING_PENDIENTE")]
    public partial class V_PRE160_PICKING_PENDIENTE
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        public long? QT_PREPARACIONES { get; set; }

        public long? QT_PEDIDOS { get; set; }

        public long? QT_CLIENTES { get; set; }

        public decimal? QT_PRODUCTOS { get; set; }

        public long? QT_PICKEOS { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }
    }
}
