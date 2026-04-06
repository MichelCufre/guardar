using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_DOCUMENTO_AJUSTE_STOCK_HIST")]
    public partial class T_DOCUMENTO_AJUSTE_STOCK_HIST
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AJUSTE_STOCK { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(16)]
        public string TP_OPERACION { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_OPERACION { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public decimal? CD_FAIXA { get; set; }

        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public int? CD_EMPRESA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public decimal? QT_MOVIMIENTO { get; set; }

        [StringLength(50)]
        public string DS_MOTIVO { get; set; }

        [StringLength(6)]
        public string TP_DOCUMENTO { get; set; }

        [StringLength(10)]
        public string NU_DOCUMENTO { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(3)]
        public string CD_MOTIVO_AJUSTE { get; set; }

        public DateTime? DT_MOTIVO { get; set; }

        public int? CD_FUNC_MOTIVO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [StringLength(30)]
        public string CD_APLICACAO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }
    }
}
