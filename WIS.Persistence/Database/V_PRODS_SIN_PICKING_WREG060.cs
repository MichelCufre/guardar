namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_PRODS_SIN_PICKING_WREG060")]
    public partial class V_PRODS_SIN_PICKING_WREG060
    {

        [Key]
        [Required]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Required]
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        [StringLength(65)]
        [Column]
        public string DS_PRODUTO { get; set; }
        
        [Required]
        public short CD_SITUACAO { get; set; }

        [StringLength(30)]
        [Column] 
        public string DS_SITUACAO { get; set; }

        public int CD_FAMILIA_PRODUTO { get; set; }

        public decimal? QT_UNIDADE_EMBALAGEM { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_EMBALAGEM_FAIXA { get; set; }

        public decimal? QT_ESTOQUE { get; set; }
    }
}
