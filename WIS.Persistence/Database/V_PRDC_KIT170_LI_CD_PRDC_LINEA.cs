namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_PRDC_KIT170_LI_CD_PRDC_LINEA")]
    public partial class V_PRDC_KIT170_LI_CD_PRDC_LINEA
    {
        [Key]
        [StringLength(10)]
        [Column]
        public string CD_PRDC_LINEA { get; set; }

        [StringLength(2000)]
        [Column]
        public string DS_PRDC_LINEA { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_TIPO_LINEA { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TIPO_LINEA { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

    }
}
