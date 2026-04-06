
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    [Table("V_ORT_FUNC_COMP_COR18")]
    public partial class V_ORT_FUNC_COMP_COR18
    {

        [Required]
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string NU_COMPONENTE { get; set; }

        [Column]
        [StringLength(100)]
        public string DS_SIGNIFICADO { get; set; }

    }
}
