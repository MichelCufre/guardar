namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_SEG_PED_PRE640")]
    public partial class V_SEG_PED_PRE640
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

        [StringLength(1)]
        [Column]
        public string ND_ACTIVIDAD { get; set; }

        public int? NU_ULT_PREPARACION { get; set; }

        public decimal? QT_EXPEDIDO { get; set; }

        public decimal? QT_PEDIDO { get; set; }

        public decimal? QT_LIBERADO { get; set; }

        public decimal? QT_PREPARADO { get; set; }

        public decimal? QT_ANULADO { get; set; }

        public decimal? QT_FACTURADO { get; set; }

        public decimal? QT_CROSS_DOCK { get; set; }

        public decimal? QT_CONTROLADO { get; set; }

        public decimal? QT_TRANSFERIDO { get; set; }

        public decimal? QT_ABASTECIDO { get; set; }

        public decimal? QT_CARGADO { get; set; }

        //public int? CD_FUN_RESP { get; set; }

        //public DateTime? DT_FUN_RESP { get; set; }

        [StringLength(27)]
        [Column]
        public string DT_LIBERADO { get; set; }

        [StringLength(27)]
        [Column]
        public string DT_INI_PEDIDO { get; set; }

        [StringLength(27)]
        [Column]
        public string DT_FIN_PEDIDO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        //[StringLength(100)]
        //[Column]
        //public string FULLNAME { get; set; }

        [StringLength(21)]
        [Column]
        public string STATUS_PEOR { get; set; }

        public decimal? QT_PENDIENTE_LIB { get; set; }

        public decimal? QT_PENDIENTE_PRE { get; set; }

        public decimal? QT_PENDIENTE_EXP { get; set; }
    }
}
