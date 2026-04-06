
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

namespace WIS.Persistence.Database
{

    [Table("T_LPN_CONSOLIDACION_TIPO")]
    public partial class T_LPN_CONSOLIDACION_TIPO
    {

        [Key]
        [Column(Order = 0)]
        public int ID_CONSOLIDACION_TIPO { get; set; }

        [Required]
        [StringLength(50)]
        public string NM_CONSOLIDACION { get; set; }

    }
}
