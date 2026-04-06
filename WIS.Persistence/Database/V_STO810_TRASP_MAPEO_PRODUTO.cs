using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_STO810_TRASP_MAPEO_PRODUTO")]
    public partial class V_STO810_TRASP_MAPEO_PRODUTO
    {
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        public int CD_EMPRESA_DESTINO { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA_DESTINO { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_PRODUTO_DESTINO { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO_DESTINO { get; set; }

        public decimal CD_FAIXA_DESTINO { get; set; }

        public decimal QT_ORIGEN { get; set; }

        public decimal QT_DESTINO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }
    }
}
