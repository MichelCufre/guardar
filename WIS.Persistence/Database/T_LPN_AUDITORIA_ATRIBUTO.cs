using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("T_LPN_AUDITORIA_ATRIBUTO")]
    public partial class T_LPN_AUDITORIA_ATRIBUTO
    {

        [Key]
        [Column(Order = 0)]
        public long NU_AUDITORIA { get; set; }

        [Key]
        [Column(Order = 1)]
        public int ID_ATRIBUTO { get; set; }

        [StringLength(400)]
        public string VL_LPN_DET_ATRIBUTO { get; set; }

    }
}
