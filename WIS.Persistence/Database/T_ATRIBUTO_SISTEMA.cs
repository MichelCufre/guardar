using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_ATRIBUTO_SISTEMA")]
    public partial class T_ATRIBUTO_SISTEMA
    {
        [Key]
        [StringLength(100)]
        [Column]
        public string NM_CAMPO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CAMPO { get; set; }
    }
}
