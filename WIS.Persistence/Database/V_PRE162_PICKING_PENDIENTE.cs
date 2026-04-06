namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRE162_PICKING_PENDIENTE")]
    public partial class V_PRE162_PICKING_PENDIENTE
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_PREPARACION { get; set; }

        public DateTime? DT_INICIO { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        public decimal? QT_PRODUCTOS { get; set; }

        public decimal? AUXUNIDADES_PREPARADAS { get; set; }

        public decimal? AUXPORC_UNIDADES { get; set; }

        public long? QT_PICKEOS { get; set; }

        public decimal? AUXPICKEOS_PREPARADOS { get; set; }

        public decimal? AUXPORC_PICKEOS { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        [StringLength(2)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TIPO_AGENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_AGENTE { get; set; }
    }
}
