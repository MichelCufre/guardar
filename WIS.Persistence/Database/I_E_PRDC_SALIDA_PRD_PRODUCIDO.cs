using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("I_E_PRDC_SALIDA_PRD_PRODUCIDO")]
    public partial class I_E_PRDC_SALIDA_PRD_PRODUCIDO
    {
        [StringLength(1)]
        public string ID_PROCESADO { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string NU_REGISTRO { get; set; }

        [Required]
        [StringLength(20)]
        public string NU_REGISTRO_PADRE { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        [Required]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [StringLength(40)]
        public string DT_VENCIMIENTO { get; set; }

        public decimal? VL_MERCADERIA { get; set; }

        public decimal? VL_TRIBUTO { get; set; }

        public decimal? QT_PRODUCIDO { get; set; }

        [StringLength(40)]
        public string ND_ACCION_MOVIMIENTO { get; set; }

        [StringLength(1)]
        public string FL_SEMIACABADO { get; set; }

    }
}
