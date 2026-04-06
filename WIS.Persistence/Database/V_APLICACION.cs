using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_APLICACION")]
    public partial class V_APLICACION
    {

        [Key]
        [Column(Order = 0)]
        [StringLength(30)]
        public string CD_APLICACION { get; set; }

        [StringLength(100)]
        public string DS_APLICACION { get; set; }

    }
}
