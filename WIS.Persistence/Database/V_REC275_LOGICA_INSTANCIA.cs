namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_REC275_LOGICA_INSTANCIA")]
    public partial class V_REC275_LOGICA_INSTANCIA
    {
        [StringLength(200)]
        [Column]
        public string DS_ALM_LOGICA { get; set; }

        [Key]
        public int NU_ALM_LOGICA_INSTANCIA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_ALM_LOGICA_INSTANCIA { get; set; }
        public DateTime DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        public int NU_ALM_ESTRATEGIA { get; set; }
        public short NU_ALM_LOGICA { get; set; }
        public short NU_ORDEN { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ORDEN_ASC { get; set; }

    }
}
