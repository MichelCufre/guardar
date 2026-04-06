using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_INV050_AVANCE_INVENTARIO")]
    public partial class V_INV050_AVANCE_INVENTARIO
    {
        [Key]
        public decimal NU_INVENTARIO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_INVENTARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public int? QT_TOTAL { get; set; }

        public int? QT_PENDIENTES { get; set; }

        public int? QT_REALIZADOS { get; set; }

        public int? QT_ERRONEOS { get; set; }

        public int? QT_FINALIZADOS { get; set; }

        public int? QT_FUNCIONARIOS { get; set; }

        public int? QT_ENDERECOS { get; set; }

        public decimal? QT_PCENT_REALIZADOS { get; set; }

        public decimal? QT_PCENT_TOTAL_ERRONEOS { get; set; }

        public decimal? QT_TIEMPO_INSUMIDO { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_ESTADO_INVENTARIO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_ESTADO_INVENTARIO { get; set; }

        [StringLength(9)]
        [Column]
        public string TP_INVENTARIO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TP_INVENTARIO { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_CIERRE_CONTEO { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_CIERRE_CONTEO { get; set; }
    }
}
