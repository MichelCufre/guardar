using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_CAMBIO_DOCUMENTO_DET")]
    public partial class T_CAMBIO_DOCUMENTO_DET
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INT { get; set; }

        [StringLength(1)]
        public string ID_PROCESADO { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public decimal? CD_FAIXA { get; set; }

        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        [StringLength(10)]
        public string NU_DOCUMENTO_CAMBIO { get; set; }

        [StringLength(6)]
        public string TP_DOCUMENTO_CAMBIO { get; set; }

        public decimal? QT_CAMBIO { get; set; }
    }
}
