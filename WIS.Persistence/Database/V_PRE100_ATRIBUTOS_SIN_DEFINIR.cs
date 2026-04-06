using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_PRE100_ATRIBUTOS_SIN_DEFINIR")]
    public partial class V_PRE100_ATRIBUTOS_SIN_DEFINIR
    {
        [Key]
        [Column(Order = 0)]
        public int ID_ATRIBUTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(1)]
        public string FL_CABEZAL { get; set; }

        [StringLength(50)]
        public string NM_ATRIBUTO { get; set; }

        [StringLength(400)]
        public string DS_ATRIBUTO { get; set; }

        [StringLength(10)]
        public string ID_ATRIBUTO_TIPO { get; set; }

        [StringLength(100)]
        public string DS_ATRIBUTO_TIPO { get; set; }
    }
}
