namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_PAR400_ATRIBUTOS_VALIDACION_ASOCIADA")]
    public partial class V_PAR400_ATRIBUTOS_VALIDACION_ASOCIADA
    {
        [Key]
        public int ID_ATRIBUTO { get; set; }

        [Key]
        public short ID_VALIDACION { get; set; }

        [StringLength(100)]
        [Column]
        public string VL_ARGUMENTO { get; set; }

    }
}