namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PICKING_PENDIENTE_WPRE160")]
    public partial class V_PICKING_PENDIENTE_WPRE160
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        public long? QT_PREPARACIONES { get; set; }

        public long? QT_PEDIDOS { get; set; }

        public long? QT_CLIENTES { get; set; }

        public decimal? QT_PRODUCTOS { get; set; }

        public long? QT_PICKEOS { get; set; }
    }
}
