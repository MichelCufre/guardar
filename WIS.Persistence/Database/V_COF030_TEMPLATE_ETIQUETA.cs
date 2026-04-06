namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_COF030_TEMPLATE_ETIQUETA")]
    public partial class V_COF030_TEMPLATE_ETIQUETA
    {
        [Key]
        [StringLength(15)]
        [Column(Order = 0)]
        public string CD_LABEL_ESTILO { get; set; }

        [Key]
        [StringLength(10)]
        [Column(Order = 1)]
        public string CD_LENGUAJE_IMPRESION { get; set; }

        [StringLength(4000)]
        [Column(Order = 2)]
        public string VL_PRE_CUERPO { get; set; }    
        
        [StringLength(4000)]
        [Column(Order = 3)]
        public string VL_CUERPO { get; set; }

        [StringLength(4000)]
        [Column(Order = 4)]
        public string VL_POST_CUERPO { get; set; }

        [Column(Order = 5, TypeName = "number")]
        public decimal? VL_LABEL_ALTURA { get; set; }

        [Column(Order = 6, TypeName = "number")]
        public decimal? VL_LABEL_LARGURA { get; set; }
    }
}
