using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_CODIGO_MULTIDATO")]
    public partial class V_CODIGO_MULTIDATO
    {

        [Key]
        [Column(Order = 0)]
        [StringLength(30)]
        public string CD_CODIGO_MULTIDATO { get; set; }

        [StringLength(100)]
        public string DS_CODIGO_MULTIDATO { get; set; }

    }
}
