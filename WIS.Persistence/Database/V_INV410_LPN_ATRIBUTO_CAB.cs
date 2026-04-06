using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_INV410_LPN_ATRIBUTO_CAB")]
    public partial class V_INV410_LPN_ATRIBUTO_CAB
    {
        [Key]
        public long NU_LPN { get; set; }

        [StringLength(50)]
        public string ID_LPN_EXTERNO { get; set; }

        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

        [StringLength(30)]        
        public string NM_LPN_TIPO { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [Key]
        public int ID_ATRIBUTO { get; set; }

        [StringLength(50)]
        public string NM_ATRIBUTO { get; set; }

        [StringLength(400)]        
        public string VL_ATRIBUTO { get; set; }
    }
}