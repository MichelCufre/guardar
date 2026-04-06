namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_LOCALIZACION_WCOF050")]
    public partial class V_LOCALIZACION_WCOF050
    {
        [Key]
        [StringLength(30)]
        public string CD_APLICACION { get; set; }

        [Key]
        [StringLength(50)]
        public string CD_BLOQUE { get; set; }

        [Key]
        [StringLength(10)]
        public string CD_TIPO { get; set; }

        [Key]
        [StringLength(100)]
        public string CD_CLAVE { get; set; }

        [Key]
        [StringLength(10)]
        public string CD_IDIOMA { get; set; }

        [StringLength(4000)]
        public string DS_VALOR_NUEVO { get; set; }

        [StringLength(4000)]
        public string DS_VALOR { get; set; }

        [StringLength(10)]
        public string CD_IDIOMA_NUEVO { get; set; }
    }
}
