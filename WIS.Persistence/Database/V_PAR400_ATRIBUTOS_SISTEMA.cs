namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_PAR400_ATRIBUTOS_SISTEMA")]
    public partial class V_PAR400_ATRIBUTOS_SISTEMA
    {
        [StringLength(100)]
        [Column]
        public string DS_CAMPO { get; set; }

        [Key]
        [StringLength(100)]
        [Column]
        public string NM_CAMPO { get; set; }

    }
}
