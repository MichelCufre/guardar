
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("T_ATRIBUTO_VALIDACION_ASOCIADA")]
    public partial class T_ATRIBUTO_VALIDACION_ASOCIADA
    {

        [Key]
        [Column(Order = 0)]
        public int ID_ATRIBUTO { get; set; }

        [Key]
        [Column(Order = 1)]
        public short ID_VALIDACION { get; set; }

        [StringLength(100)]
        public string VL_ARGUMENTO { get; set; }

    }
}