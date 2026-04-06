namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_PAR400_TIPO_ATRIBUTO")]
    public partial class V_PAR400_TIPO_ATRIBUTO
    {
        [Key]
        [StringLength(10)]
        [Column]
        public string ID_ATRIBUTO_TIPO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ATRIBUTO_TIPO { get; set; }

    }
}
