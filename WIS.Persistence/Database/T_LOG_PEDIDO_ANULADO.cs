using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_LOG_PEDIDO_ANULADO")]
    public partial class T_LOG_PEDIDO_ANULADO
    {
        [Required]
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_PEDIDO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CLIENTE { get; set; }

        public int? CD_EMPRESA { get; set; }

        public decimal? QT_ANULADO { get; set; }

        public int? CD_FUNCIONARIO { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_MOTIVO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_APLICACAO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_LOG_PEDIDO_ANULADO { get; set; }

        public long? NU_INTERFAZ_EJECUCION { get; set; }
    }
}
