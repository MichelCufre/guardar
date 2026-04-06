using System;
using System.ComponentModel.DataAnnotations;

namespace WIS.Persistence.Database
{
    public class T_CLIENTE_DIASVALIDEZ_VENTANA
    {

        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        public int CD_EMPRESA { get; set; }

        public short QT_DIAS_VALIDADE_LIBERACION { get; set; }

        [StringLength(20)]
        public string CD_VENTANA_LIBERACION { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public DateTime? DT_ADDROW { get; set; }
    }
}
