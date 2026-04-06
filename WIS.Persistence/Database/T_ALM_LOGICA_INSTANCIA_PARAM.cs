
namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("T_ALM_LOGICA_INSTANCIA_PARAM")]
    public partial class T_ALM_LOGICA_INSTANCIA_PARAM
    {
        [Key]
        public int NU_ALM_LOGICA_INSTANCIA_PARAM { get; set; }
        public int NU_ALM_LOGICA_INSTANCIA { get; set; }
        public DateTime DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        public short NU_ALM_PARAMETRO { get; set; }
        public short NU_ALM_LOGICA { get; set; }

        [StringLength(200)]
        [Column]
        public string VL_ALM_PARAMETRO { get; set; }

        [StringLength(64)]
        [Column]
        public string VL_AUDITORIA { get; set; }

        public virtual T_ALM_LOGICA_INSTANCIA T_ALM_LOGICA_INSTANCIA { get; set; }

        public virtual T_ALM_PARAMETRO T_ALM_PARAMETRO { get; set; }
    }
}
