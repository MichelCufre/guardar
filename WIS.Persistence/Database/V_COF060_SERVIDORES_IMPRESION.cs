namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_COF060_SERVIDORES_IMPRESION")]
    public partial class V_COF060_SERVIDORES_IMPRESION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_SERVIDOR { get; set; }

        [StringLength(200)]
        public string DS_SERVIDOR { get; set; }

        [StringLength(100)]
        public string VL_URL_SERVIDOR { get; set; }

        [StringLength(70)]
        public string VL_DOMINIO_SERVIDOR { get; set; }

        public string CLIENTID { get; set; }
    }
}
