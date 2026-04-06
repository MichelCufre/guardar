using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WIS.Persistence.Database
{
    public class V_FAC008_RESULTADO_DETALLE
    {
        [Key]
        public decimal NU_RESULTADO_DETALLE { get; set; }
        public int NU_EJECUCION { get; set; }
        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(20)]
        public string CD_FACTURACION { get; set; }

        [StringLength(100)]
        public string DS_FACTURACION { get; set; }

        [StringLength(20)]
        public string CD_PROCESO { get; set; }

        [StringLength(200)]
        public string DS_PROCESO { get; set; }
        public DateTime? DT_GENERICO { get; set; }

        [StringLength(1000)]
        public string VL_SERIALIZADO { get; set; }

        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }

        [StringLength(100)]
        public string DS_COMPONENTE { get; set; }
        public decimal? QT_RESULTADO { get; set; }
        public DateTime? DT_ADDROW { get; set; }

    }
}
