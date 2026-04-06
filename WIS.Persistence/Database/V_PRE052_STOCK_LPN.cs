
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;


namespace WIS.Persistence.Database
{

    [Table("V_PRE052_STOCK_LPN")]
    public partial class V_PRE052_STOCK_LPN
    {

        public long NU_LPN { get; set; }
        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        public short CD_AREA_ARMAZ { get; set; }

        [Required]
        [StringLength(15)]
        public string DS_AREA_ARMAZ { get; set; }

        [Required]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [Required]
        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

        [Required]
        [StringLength(50)]
        public string ID_LPN_EXTERNO { get; set; }

        [StringLength(1)]
        public string ID_AVERIA { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

    }
}