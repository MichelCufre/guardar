using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_INV100_ANALISIS_INVENTARIO")]
    public partial class V_INV100_ANALISIS_INVENTARIO
    {
        [Key]
        [Column(Order = 0)]
        public decimal NU_INVENTARIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public int? CD_EMPRESA_INVENTARIO { get; set; }

        [Key]
        [Column(Order = 1)]
        public decimal NU_INVENTARIO_ENDERECO { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal NU_INVENTARIO_ENDERECO_DET { get; set; }

        public decimal? QT_INVENTARIO { get; set; }

        public decimal? QT_DIFERENCIA { get; set; }

        [StringLength(3)]
        [Column]
        public string CD_MOTIVO_AJUSTE { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_ESTADO_INV_ENDERECO_DET { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(60)]
        [Column]
        public string DS_MOTIVO_AJUSTE { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
