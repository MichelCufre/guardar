namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_TEMP_CONDICION_LIBERACION")]
    public partial class T_TEMP_CONDICION_LIBERACION
    {
        public short? CD_ONDA { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(6)]
        public string CD_CONDICION_LIBERACION { get; set; }

        [Key]
        [Column(Order = 1)]
        public decimal NU_TEMP_CONDICION_LIBERACION { get; set; }

        public decimal? CD_EMPRESA { get; set; }

        public DateTime? DT_ADDROW { get; set; }
    }
}
