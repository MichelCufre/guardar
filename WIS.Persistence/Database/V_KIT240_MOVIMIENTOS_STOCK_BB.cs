using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_KIT240_MOVIMIENTOS_STOCK_BB")]
    public partial class V_KIT240_MOVIMIENTOS_STOCK_BB
    {
        [Key]
        [StringLength(20)]
        public string NU_MOVIMIENTO_BB { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_MOVIMIENTO { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO_ORIGEN { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO_DESTINO { get; set; }

        [StringLength(100)]
        public string DS_DOMINIO_VALOR { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(100)]
        public string FULLNAME { get; set; }

        public DateTime? DT_MOVIMIENTO { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(10)]
        public string NU_PRDC_INGRESO { get; set; }
    }
}
