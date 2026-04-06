using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    public class V_FACTURAC_LISTA_PREC_WFAC255
    {
        [Key]
        public int CD_LISTA_PRECIO { get; set; }
        
        [Required]
        [StringLength(50)]
        public string DS_LISTA_PRECIO { get; set; }

        [Required]
        [StringLength(15)]
        public string CD_MONEDA { get; set; }

        [StringLength(80)]
        public string DS_MONEDA { get; set; }
        
        [StringLength(4)]
        public string DS_SIMBOLO { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
    }
}
