using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_DOCUMENTO_AJUSTE_STOCK")]
    public partial class V_DOCUMENTO_AJUSTE_STOCK
    {
        [Key]
        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(40)]
        public string CD_CGC_EMPRESA { get; set; }

        public int? QT_AJUSTE_POSITIVO { get; set; }

        public int? QT_AJUSTE_NEGATIVO { get; set; }
    }
}
