using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_IMPRESORA")]
    public partial class T_IMPRESORA
    {
        [Key]        
        [StringLength(50)]
        public string CD_IMPRESORA { get; set; }

        [StringLength(100)]
        public string DS_IMPRESORA { get; set; }

        [Key]        
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [StringLength(10)]
        public string CD_LENGUAJE_IMPRESION { get; set; }

        public int? CD_SERVIDOR { get; set; }

        public virtual T_LENGUAJE_IMPRESION T_LENGUAJE_IMPRESION { get; set; }
    }
}
