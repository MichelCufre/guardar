using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;

namespace WIS.Persistence.Database
{

    [Table("V_REC280_PANEL_SUGERENCIA_DET")]
    public partial class V_REC280_PANEL_SUGERENCIA_DET
    {
        [Key]
        public long NU_ALM_SUGERENCIA_DET { get; set; }

        [Key]
        public int NU_ALM_ESTRATEGIA { get; set; }

        [Key]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [Key]
        [StringLength(24)]
        public string CD_ALM_OPERATIVA_ASOCIABLE { get; set; }

        [Key]
        [StringLength(10)]
        public string TP_ALM_OPERATIVA_ASOCIABLE { get; set; }

        [StringLength(100)]
        public string DS_ALM_OPERATIVA_ASOCIABLE { get; set; }

        [Key]
        [StringLength(2)]
        public string CD_CLASSE { get; set; }

        [StringLength(100)]
        public string DS_CLASSE { get; set; }

        [Key]
        [StringLength(50)]
        public string CD_GRUPO { get; set; }

        [StringLength(100)]
        public string DS_GRUPO { get; set; }

        [Key]
        public int CD_EMPRESA_PRODUTO { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA_PRODUTO { get; set; }

        [Key]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [Key]
        [StringLength(40)]
        public string CD_REFERENCIA { get; set; }

        [Key]
        [StringLength(40)]
        public string CD_AGRUPADOR { get; set; }

        [Key]
        [StringLength(40)]
        public string CD_ENDERECO_SUGERIDO { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO_SUGERIDO_AGRUPADOR { get; set; }

        [Key]
        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA_AGRUPADOR { get; set; }

        [Key]
        [StringLength(40)]
        public string CD_PRODUTO_AGRUPADOR { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO_AGRUPADOR { get; set; }

        [Key]
        public decimal CD_FAIXA_AGRUPADOR { get; set; }

        [Key]
        [StringLength(40)]
        public string NU_IDENTIFICADOR_AGRUPADOR { get; set; }

        public decimal QT_PRODUTO_AGRUPADOR { get; set; }

        public DateTime? DT_FABRICACAO_AGRUPADOR { get; set; }

        public decimal? QT_AUDITADA_AGRUPADOR { get; set; }

        public decimal? QT_CLASIFICADA_AGRUPADOR { get; set; }

        [Key]
        public long NU_ALM_SUGERENCIA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        [StringLength(30)]
        public string NM_FUNCIONARIO { get; set; }

        [StringLength(1)]
        public string CD_ESTADO { get; set; }

        [StringLength(100)]
        public string DS_ESTADO { get; set; }

        [StringLength(20)]
        public string CD_MOTVO_RECHAZO { get; set; }

        [StringLength(100)]
        public string DS_MOTVO_RECHAZO { get; set; }
        public int? NU_ALM_LOGICA_INSTANCIA { get; set; }

        [StringLength(200)]
        public string DS_ALM_LOGICA_INSTANCIA { get; set; }

        public short? NU_ALM_LOGICA { get; set; }

        [StringLength(200)]
        public string DS_ALM_LOGICA { get; set; }

    }
}
