using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_STO395_MOVTO_STOCK")]
    public partial class V_STO395_MOVTO_STOCK
    {
        [Key]
        public DateTime? DT_ADDROW { get; set; }

        [Key]
        [Column]
        [StringLength(40)]
        public string NU_DOCTO { get; set; }

        [Key]
        [Column]
        [StringLength(40)]
        public string NU_DOCTO_EXT { get; set; }
        
        [Key]
        [Column]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        public int? CD_EMPRESA { get; set; }

        [StringLength(8)]
        [Column]
        public string HR_ADDROW { get; set; }

        [StringLength(120)]
        [Column]
        public string CD_AGENTE { get; set; }

        [StringLength(40)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(300)]
        [Column]
        public string DS_CLIENTE { get; set; }

        public decimal? CD_FAIXA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_PRODUTO { get; set; }

        [StringLength(200)]
        [Column]
        public string CD_FAMILIA_PRODUTO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_FAMILIA_PRODUTO { get; set; }

        [StringLength(200)]
        [Column]
        public string CD_PRODUTO_EMPRESA { get; set; }

        [StringLength(200)]
        [Column]
        public string CD_MERCADOLOGICO { get; set; }

        [StringLength(200)]
        [Column]
        public string CD_SITUACAO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_REDUZIDA { get; set; }

        public decimal? QT_ITEM { get; set; }

        public int? CD_MOVIMIENTO { get; set; }

        [StringLength(24)]
        [Column]
        public string DS_MOVIMIENTO { get; set; }

        [StringLength(10)]
        [Column]
        public string TP_MOVIMIENTO { get; set; }

        [StringLength(64)]
        [Column]
        public string DS_MEMO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
