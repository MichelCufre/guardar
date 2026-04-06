using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_PRDC_PROD_SALIDA_PREP")]
    public partial class V_PRDC_PROD_SALIDA_PREP
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string CD_PRDC_DEFINICION { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PREPARACION { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal NU_CONTENEDOR { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal CD_FAIXA { get; set; }

        public decimal? QT_SALIDA { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO { get; set; }
    }
}
