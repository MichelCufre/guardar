using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_DET_DOCU_EGRESO_CAMBIO")]
    public partial class T_DET_DOCU_EGRESO_CAMBIO
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_SECUENCIA { get; set; }

        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        [Required]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [StringLength(6)]
        public string TP_DOCUMENTO_INGRESO { get; set; }

        [StringLength(10)]
        public string NU_DOCUMENTO_INGRESO { get; set; }

        public decimal? QT_DESAFECTADA { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        public decimal? VL_FOB { get; set; }

        public decimal? VL_CIF { get; set; }

        public decimal? QT_DESCARGADA { get; set; }

        [StringLength(64)]
        public string VL_DATO_AUDITORIA { get; set; }

        public decimal? VL_TRIBUTO { get; set; }

        [StringLength(40)]
        public string NU_PROCESO { get; set; }
    }
}
