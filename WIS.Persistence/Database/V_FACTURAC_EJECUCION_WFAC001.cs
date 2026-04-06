namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_FACTURAC_EJECUCION_WFAC001")]
    public partial class V_FACTURAC_EJECUCION_WFAC001
    {
        [Key]
        public int NU_EJECUCION { get; set; }
        
        [StringLength(100)]
        public string NM_EJECUCION { get; set; }
        
        [StringLength(30)]
        public string DS_SITUACAO { get; set; }
        
        [StringLength(30)]
        public string NM_FUNCIONARIO_EJECUCION { get; set; }

        [StringLength(30)]
        public string NM_FUNCIONARIO_PROGRAMACION { get; set; }

        [StringLength(30)]
        public string NM_FUNCIONARIO_APROBACION { get; set; }

        [StringLength(30)]
        public string NM_FUNCIONARIO_ANULACION { get; set; }

        [StringLength(1)]
        public string FL_EJEC_POR_HORA { get; set; }

        public DateTime? DT_DESDE { get; set; }
        public DateTime? DT_HASTA { get; set; }
        public DateTime? DT_CORTE_QUINCENA { get; set; }
        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_PROGRAMACION { get; set; }
        public DateTime? DT_EJECUCION { get; set; }
        public DateTime? DT_APROBACION { get; set; }
        public DateTime? DT_ENVIADA { get; set; }
        public DateTime? DT_ANULACION { get; set; }
        public short? CD_SITUACAO { get; set; }
        public int? CD_FUNC_EJECUCION { get; set; }
        public int? CD_FUNC_PROGRAMACION { get; set; }
        public int? CD_FUNC_APROBACION { get; set; }
        public int? CD_FUNC_ANULACION { get; set; }
        public int? CD_FUNC_ENVIADA { get; set; }
    }
}
