using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_PRDC_MOVIMIENTO_BB")]
    public partial class T_PRDC_MOVIMIENTO_BB
    {
        [Key]
        [StringLength(20)]
        public string NU_MOVIMIENTO_BB { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO_ORIGEN { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO_DESTINO { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? CD_FAIXA { get; set; }

        public int? CD_EMPRESA { get; set; }

        public decimal? QT_MOVIMIENTO { get; set; }

        [StringLength(40)]
        public string ND_ACCION_MOVIMIENTO { get; set; }

        public DateTime? DT_MOVIMIENTO { get; set; }

        public int? USERID { get; set; }

        [StringLength(10)]
        public string NU_PRDC_INGRESO { get; set; }

        public long? NU_INTERFAZ_EJECUCION { get; set; }
    }

}
