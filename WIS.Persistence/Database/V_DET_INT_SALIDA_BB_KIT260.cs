using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_DET_INT_SALIDA_BB_KIT260")]
    public partial class V_DET_INT_SALIDA_BB_KIT260
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(8)]
        public string TIPO { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [StringLength(100)]
        public string DS_DOMINIO_VALOR { get; set; }

        public decimal? QT_MOVIMIENTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string NU_REGISTRO { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long? NU_INTERFAZ_EJECUCION { get; set; }
    }
}
