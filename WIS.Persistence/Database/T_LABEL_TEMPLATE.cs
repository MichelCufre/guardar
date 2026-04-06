namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_LABEL_TEMPLATE")]
    public partial class T_LABEL_TEMPLATE
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(15)]
        public string CD_LABEL_ESTILO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string CD_LENGUAJE_IMPRESION { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_PRE_CUERPO { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_CUERPO { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_POST_CUERPO { get; set; }

        public decimal? VL_LABEL_ALTURA { get; set; }

        public decimal? VL_LABEL_LARGURA { get; set; }

        public virtual T_LABEL_ESTILO T_LABEL_ESTILO { get; set; }
    }
}
