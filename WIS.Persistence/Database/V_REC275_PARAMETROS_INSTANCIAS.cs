namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_REC275_PARAMETROS_INSTANCIAS")]
    public partial class V_REC275_PARAMETROS_INSTANCIAS
    {
        [StringLength(200)]
        [Column]
        public string DS_ALM_PARAMETRO { get; set; }

        [Key]
        public int NU_ALM_LOGICA_INSTANCIA_PARAM { get; set; }
        public int NU_ALM_LOGICA_INSTANCIA { get; set; }

        [StringLength(100)]
        [Column]
        public string NM_ALM_PARAMETRO { get; set; }
        public DateTime DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        public short NU_ALM_PARAMETRO { get; set; }

        [StringLength(100)]
        [Column]
        public string VL_ALM_PARAMETRO { get; set; }
    }
}
