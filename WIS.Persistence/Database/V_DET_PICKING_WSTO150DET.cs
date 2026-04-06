namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_DET_PICKING_WSTO150DET")]
    public partial class V_DET_PICKING_WSTO150DET
    {
        public int NU_PREPARACION { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 1)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_SEQ_PREPARACION { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        public long? NU_CARGA { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_AGRUPACION { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        public decimal? QT_PREPARADO { get; set; }

        public int? NU_CONTENEDOR { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public int? NU_CONTENEDOR_SYS { get; set; }

        public DateTime? DT_PICKEO { get; set; }

        public int? NU_CONTENEDOR_PICKEO { get; set; }

        public int? CD_FUNC_PICKEO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string CD_MERCADOLOGICO { get; set; }

        [StringLength(20)]
        [Column]
        public string DS_REDUZIDA { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_ESTADO { get; set; }
    }
}
