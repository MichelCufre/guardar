namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PICKING_PENDIENTE_WPRE161")]
    public partial class V_PICKING_PENDIENTE_WPRE161
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION { get; set; }

        public long? QT_PEDIDOS { get; set; }

        public double? AUXPEDI_COMPLETOS { get; set; }

        public double? AUXPORC_PEDIDOS { get; set; }

        public long? QT_CLIENTES { get; set; }

        public double? AUXCLIENTES_COMPLETOS { get; set; }

        public double? AUXPORC_CLIENTES { get; set; }

        public decimal? QT_PRODUCTOS { get; set; }

        public decimal? AUXUNIDADES_PREPARADAS { get; set; }

        public decimal? AUXPORC_UNIDADES { get; set; }

        public long? QT_PICKEOS { get; set; }

        public double? AUXPICKEOS_PREPARADOS { get; set; }

        public double? AUXPORC_PICKEOS { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
