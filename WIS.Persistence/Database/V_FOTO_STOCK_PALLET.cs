using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    public class V_FOTO_STOCK_PALLET
    {
        [Key]
        public DateTime DT_FOTO { get; set; }

        [Key]
        public int CD_EMPRESA { get; set; }

        [Key]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }
        
        [Key]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }
        
        [StringLength(1)]
        public string FL_ING_AGENDA { get; set; }
        
        [StringLength(40)]
        public string NU_PALLET { get; set; }
        
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }
        
        public decimal? QT_ESTOQUE { get; set; }
        public int? NU_AGENDA_INGRESO { get; set; }
    }
}
