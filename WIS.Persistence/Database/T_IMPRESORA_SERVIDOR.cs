namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_IMPRESORA_SERVIDOR")]
    public partial class T_IMPRESORA_SERVIDOR
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_SERVIDOR { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_SERVIDOR { get; set; }

        public string CLIENTID { get; set; }

        [StringLength(70)]
        [Column]
        public string VL_DOMINIO_SERVIDOR { get; set; }

        [StringLength(4000)]
        [Column]
        public string VL_PASS_SERVIDOR { get; set; }

        [StringLength(100)]
        [Column]
        public string VL_URL_SERVIDOR { get; set; }

        [StringLength(50)]
        [Column]
        public string VL_USER_SERVIDOR { get; set; }
                
    }
}
