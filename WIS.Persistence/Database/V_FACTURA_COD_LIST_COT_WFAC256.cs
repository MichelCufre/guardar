using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    public class V_FACTURA_COD_LIST_COT_WFAC256
    {
        [Key]
        [StringLength(50)]
        public string CD_FACTURACION { get; set; }

        [Key]
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }

        [Key]
        public int CD_LISTA_PRECIO { get; set; }

        [Required]
        [StringLength(100)]
        public string DS_SIGNIFICADO { get; set; }
        
        [Required]
        [StringLength(50)]
        public string DS_LISTA_PRECIO { get; set; }
        
        [StringLength(100)]
        public string DS_FUNCIONARIO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }
        public decimal? QT_IMPORTE { get; set; }
        public decimal? QT_IMPORTE_MINIMO { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        public DateTime? DT_ADDROW { get; set; }
    }
}
