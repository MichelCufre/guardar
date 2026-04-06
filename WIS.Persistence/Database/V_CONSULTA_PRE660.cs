namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_CONSULTA_PRE660")]
    public partial class V_CONSULTA_PRE660
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CLIENTE { get; set; }

        public DateTime? DT_ENTREGA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public decimal? QT_EXPEDIDO { get; set; }

        public decimal? QT_PEDIDO { get; set; }

        public decimal? QT_LIBERADO { get; set; }

        public decimal? QT_PREPARADO { get; set; }

        public decimal? QT_FACTURADO { get; set; }

        public decimal? QT_CROSS_DOCK { get; set; }

        public decimal? QT_CONTROLADO { get; set; }

        public decimal? QT_TRANSFERIDO { get; set; }

        public decimal? QT_ABASTECIDO { get; set; }

        public decimal? QT_CARGADO { get; set; }

        public decimal? QT_ANULADO { get; set; }

        public DateTime? DT_PRIMER_LIBERACION { get; set; }

        public DateTime? DT_PRIMER_PICKEO { get; set; }

        public DateTime? DT_INICIO_CARGA { get; set; }

        public DateTime? DT_ULTIMO_PICKEO { get; set; }

        public DateTime? DT_ULTIMA_FACTURACION { get; set; }

        public DateTime? DT_ULTIMA_EXPEDICION { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
