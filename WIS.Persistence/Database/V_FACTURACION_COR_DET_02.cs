using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    public class V_FACTURACION_COR_DET_02
    {
        [Key]
        public int NU_PALLET_DET { get; set; }
        
        [StringLength(20)]
        public string CD_FACTURACION { get; set; }
        
        [StringLength(20)]
        public string NU_COMPONENTE { get; set; }
        
        [StringLength(40)]
        public string NU_PALLET { get; set; }
        
        [StringLength(1)]
        public string FL_APLICO_MINIMO { get; set; }
        
        public int? CD_EMPRESA { get; set; }
        public int? NU_EJECUCION_FACTURACION { get; set; }
        public int? NU_AGENDA_INGRESO { get; set; }
        public decimal? VL_PRECIO_UNITARIO { get; set; }
        public decimal? QT_RESULTADO { get; set; }
        public decimal? QT_TOTAL { get; set; }
        public DateTime DT_ADDROW { get; set; }
        public DateTime DT_DESTRUCCION_PALLET { get; set; }
        public DateTime? DT_HASTA { get; set; }
        public DateTime? DT_DESDE { get; set; }
    }
}
