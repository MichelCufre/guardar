using System;
using System.ComponentModel.DataAnnotations;

namespace WIS.Persistence.Database
{
    public class V_CLIENTE_CONFIG_DIAS_VALIDEZ
    {
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        public string DS_CLIENTE { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        public short QT_DIAS_VALIDADE_LIBERACION { get; set; }

        [StringLength(20)]
        public string CD_VENTANA_LIBERACION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(100)]
        public string DS_VENTANA_LIBERACION { get; set; }

    }
}
