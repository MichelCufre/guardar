
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("T_ATRIBUTO_TIPO")]
    public partial class T_ATRIBUTO_TIPO
    {

        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string ID_ATRIBUTO_TIPO { get; set; }

        [Required]
        [StringLength(100)]
        public string DS_ATRIBUTO_TIPO { get; set; }

    }
}
