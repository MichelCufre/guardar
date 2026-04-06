namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_STOCK_POR_PRODUCTO")]
    public partial class V_STOCK_POR_PRODUCTO
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        public decimal? QT_REAL_ESTOQUE { get; set; }

        public decimal? QT_ESTOQUE { get; set; }

        public decimal? QT_RESERVA_SAIDA { get; set; }

        public decimal? QT_TRANSITO_ENTRADA { get; set; }
    }
}
