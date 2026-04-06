using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    public class V_FACTURACION_PROC_WFAC004
    {

        [Key]
        public int NU_EJECUCION { get; set; }

        [Key]
        [StringLength(20)]
        public string CD_PROCESO { get; set; }

        [Key]
        public int CD_EMPRESA { get; set; }
        
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }
        
        [StringLength(200)]
        public string DS_PROCESO { get; set; }
        
        [StringLength(30)]
        public string DS_SITUACAO { get; set; }
        
        [StringLength(1)]
        public string TP_PROCESO { get; set; }
        
        [StringLength(1)]
        public string FL_EJEC_POR_HORA { get; set; }
        
        [StringLength(3)]
        public string ID_ESTADO { get; set; }
        
        public short? CD_SITUACAO_ERROR { get; set; }
        public short? CD_SITUACAO_REL { get; set; }
        public decimal? QT_RESULTADO { get; set; }
        public DateTime? DT_DESDE { get; set; }
        public DateTime? DT_HASTA { get; set; }
    }
}
