using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_DOC363_AJUSTE_ACTA")]
    public partial class V_DOC363_AJUSTE_ACTA
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AJUSTE_STOCK { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public int? CD_EMPRESA { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        public decimal? QT_MOVIMIENTO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public decimal? QT_ACTA { get; set; }

        [StringLength(80)]
        public string VL_FILTRO { get; set; }
    }
}
