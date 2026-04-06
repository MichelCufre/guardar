using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_ATRIBUTO_ESTADO")]
    public partial class T_ATRIBUTO_ESTADO
    {        
        [Key]
        [StringLength(6)]
        public string ID_ESTADO { get; set; }

        [StringLength(100)]
        public string DS_ESTADO { get; set; }

    }
}
