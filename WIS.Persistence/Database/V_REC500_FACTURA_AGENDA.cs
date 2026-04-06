using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    [Table("V_REC500_FACTURA_AGENDA")]
    public class V_REC500_FACTURA_AGENDA
    {
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        public int CD_EMPRESA { get; set; }
        [Key]
        [Column(Order = 2)]
        public int NU_AGENDA { get; set; }
        [Key]
        [Column(Order = 3)]
        public int NU_RECEPCION_FACTURA { get; set; }

        [StringLength(12)]
        public string NU_FACTURA { get; set; }

        [StringLength(3)]
        public string NU_SERIE { get; set; }

    }
}
