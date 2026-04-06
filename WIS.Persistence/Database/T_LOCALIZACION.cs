namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_LOCALIZACION")]
    public partial class T_LOCALIZACION
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(30)]
        public string CD_APLICACION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string CD_BLOQUE { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string CD_TIPO { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(100)]
        public string CD_CLAVE { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(10)]
        public string CD_IDIOMA { get; set; }

        [StringLength(4000)]
        [Column]
        public string DS_VALOR { get; set; }
    }
}
