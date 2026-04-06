namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_CONDICION_LIBERACION")]
    public partial class T_CONDICION_LIBERACION
    {
        [Key]
        [StringLength(6)]
        [Column]
        public string CD_CONDICION_LIBERACION { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_CONDICION_LIBERACION { get; set; }

        [StringLength(1000)]
        [Column]
        public string VL_EXPRESION_MOSTRAR_MARCADA { get; set; }
    }
}
