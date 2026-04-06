namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PREDIOS_PICKING_PRE050")]
    public partial class V_PREDIOS_PICKING_PRE050
    {
        [Key]
        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        [StringLength(3)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
