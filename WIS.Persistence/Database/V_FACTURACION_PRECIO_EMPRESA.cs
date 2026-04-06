using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    public class V_FACTURACION_PRECIO_EMPRESA
    {
        [Key]
        public int CD_EMPRESA { get; set; }
        
        [Key]
        public int CD_LISTA_PRECIO { get; set; }
        
        [Key]
        [StringLength(20)]
        public string CD_FACTURACION { get; set; }
        
        [Key]
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }
        
        [StringLength(100)]
        public string DS_SIGNIFICADO { get; set; }
        
        [StringLength(15)]
        public string CD_MONEDA { get; set; }
        
        [StringLength(80)]
        public string DS_MONEDA { get; set; }
        
        [StringLength(4)]
        public string DS_SIMBOLO { get; set; }
        
        public decimal? VL_PRECIO_UNITARIO { get; set; }
        public decimal? VL_PRECIO_MINIMO { get; set; }
    }
}
