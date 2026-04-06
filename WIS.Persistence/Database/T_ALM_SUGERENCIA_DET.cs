
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;


namespace WIS.Persistence.Database
{

    [Table("T_ALM_SUGERENCIA_DET")]
    public partial class T_ALM_SUGERENCIA_DET
    {
        [Key]
        public int NU_ALM_ESTRATEGIA { get; set; }

        [Key]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [Key]
        [StringLength(10)]
        public string TP_ALM_OPERATIVA_ASOCIABLE { get; set; }

        [Key]
        [StringLength(24)]
        public string CD_ALM_OPERATIVA_ASOCIABLE { get; set; }

        [Key]
        [StringLength(2)]
        public string CD_CLASSE { get; set; }

        [Key]
        [StringLength(50)]
        public string CD_GRUPO { get; set; }

        public int CD_EMPRESA { get; set; }

        [Key]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [StringLength(40)]
        public string CD_REFERENCIA { get; set; }

        [Key]
        [StringLength(40)]
        public string CD_AGRUPADOR { get; set; }

        [Key]
        [StringLength(40)]
        public string CD_ENDERECO_SUGERIDO { get; set; }

        public int CD_EMPRESA_AGRUPADOR { get; set; }

        [Key]
        [StringLength(40)]
        public string CD_PRODUTO_AGRUPADOR { get; set; }

        public decimal CD_FAIXA_AGRUPADOR { get; set; }

        [Key]
        [StringLength(40)]
        public string NU_IDENTIFICADOR_AGRUPADOR { get; set; }

        public decimal QT_PRODUTO_AGRUPADOR { get; set; }

        public DateTime? DT_FABRICACAO_AGRUPADOR { get; set; }

        public decimal? QT_AUDITADA_AGRUPADOR { get; set; }

        public decimal? QT_CLASIFICADA_AGRUPADOR { get; set; }

        public long NU_ALM_SUGERENCIA { get; set; }

        [Key]
        [StringLength(40)]
        public string CD_ENDERECO_SUGERIDO_AGRUPADOR { get; set; }

        [StringLength(1)]
        public string CD_ESTADO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }
        public DateTime? DT_UPDROW { get; set; }
        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public int? NU_ALM_LOGICA_INSTANCIA { get; set; }

        public decimal? VL_TIEMPO_CALCULO { get; set; }
        [Key]
        public long NU_ALM_SUGERENCIA_DET { get; set; }

        public virtual T_ALM_SUGERENCIA T_ALM_SUGERENCIA { get; set; }

        public virtual T_ALM_LOGICA_INSTANCIA T_ALM_LOGICA_INSTANCIA { get; set; }
    }
}