namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PEDIDOS_EXPEDIDOS_EXEL")]
    public partial class V_PEDIDOS_EXPEDIDOS_EXEL
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_CAMION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(30)]
        [Column]
        public string CD_EXTERNO { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string CD_ROTA { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(40)]
        public string CD_TRANSPORTADORA { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_CONTENEDOR { get; set; }

        [StringLength(6)]
        [Column]
        public string TP_PEDIDO { get; set; }
	}
}
