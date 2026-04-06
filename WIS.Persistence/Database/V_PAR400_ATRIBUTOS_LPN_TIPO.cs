namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_PAR400_ATRIBUTOS_LPN_TIPO")]
    public partial class V_PAR400_ATRIBUTOS_LPN_TIPO
    {
        [Key]
        public int ID_ATRIBUTO { get; set; }

        [Key]
        [StringLength(10)]
        [Column]
        public string TP_LPN_TIPO { get; set; }

        [StringLength(40)]
        [Column]
        public string VL_INICIAL { get; set; }

    }
}
