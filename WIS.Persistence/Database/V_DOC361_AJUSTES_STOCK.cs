using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_DOC361_AJUSTES_STOCK")]
    public partial class V_DOC361_AJUSTES_STOCK
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AJUSTE_STOCK { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        public decimal? QT_MOVIMIENTO { get; set; }

        public int? CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(40)]
        public string CD_CGC_EMPRESA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_MOTIVO { get; set; }

        [StringLength(60)]
        public string DS_MOTIVO_AJUSTE { get; set; }

        [StringLength(100)]
        public string USUARIO_MOTIVO { get; set; }
    }
}
