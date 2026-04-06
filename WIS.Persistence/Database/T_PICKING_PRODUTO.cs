using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("T_PICKING_PRODUTO")]
    public partial class T_PICKING_PRODUTO
    {
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        [Required]
        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_SEPARACAO { get; set; }

        public int? QT_ESTOQUE_MINIMO { get; set; }

        public int? QT_ESTOQUE_MAXIMO { get; set; }

        public decimal QT_PADRAO_PICKING { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public int? QT_DESBORDE { get; set; }
        public int? QT_PADRON_DESBORDE { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string TP_PICKING { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [Required]
        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_SEC_PICKING_PRODUTO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        public string CD_UNIDAD_CAJA_AUT { get; set; }

        public int? QT_UNIDAD_CAJA_AUT { get; set; }

        public string FL_CONF_CD_BARRAS_AUT { get; set; }

        public int NU_PRIORIDAD { get; set; }

        public virtual T_ENDERECO_ESTOQUE T_ENDERECO_ESTOQUE { get; set; }
    }
}
