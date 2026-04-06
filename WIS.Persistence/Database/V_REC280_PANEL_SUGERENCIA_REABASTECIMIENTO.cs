using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_REC280_PANEL_SUGERENCIA_REABASTECIMIENTO")]
    public partial class V_REC280_PANEL_SUGERENCIA_REABASTECIMIENTO
    {
        [Key]
        public decimal NU_ALM_REABASTECIMIENTO { get; set; }

        public int NU_ETIQUETA_LOTE { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_REFERENCIA { get; set; }

        [Required]
        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_ENDERECO_SUGERIDO { get; set; }

        [Required]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        public int CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [Required]
        [StringLength(1)]
        public string CD_ESTADO { get; set; }

        [StringLength(100)]
        public string DS_ESTADO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        [StringLength(30)]
        public string NM_FUNCIONARIO { get; set; }

      
        [Column(Order = 17)]
        public long? NU_TRANSACCION { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        public decimal? QT_AUDITADA { get; set; }

        public decimal? QT_CLASIFICADA { get; set; }
        public DateTime? DT_FABRICACAO { get; set; }

        [StringLength(1)]
        public string FL_IGNORAR_STOCK { get; set; }

    }
}