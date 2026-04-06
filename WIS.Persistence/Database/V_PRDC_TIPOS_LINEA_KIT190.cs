namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRDC_TIPOS_LINEA_KIT190")]
    public partial class V_PRDC_TIPOS_LINEA_KIT190
    {
        [Key]
        [StringLength(20)]
        [Column]
        public string ND_TIPO_LINEA { get; set; }

        [Required]
        [StringLength(20)]
        [Column]
        public string CD_TIPO_LINEA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TIPO_LINEA { get; set; }
    }
}
