using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_INV413_REG_SEL")]
    public partial class V_INV413_REG_SEL
    {
        [Key]
        public decimal NU_INVENTARIO { get; set; }

        [Key]
        public decimal NU_INVENTARIO_ENDERECO { get; set; }

        [Key]
        public decimal NU_INVENTARIO_ENDERECO_DET { get; set; }

        public int? CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? CD_FAIXA { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        [StringLength(10)]
        public string ID_BLOQUE { get; set; }

        [StringLength(10)]
        public string ID_CALLE { get; set; }

        public int? NU_COLUMNA { get; set; }

        public int? NU_ALTURA { get; set; }

        [StringLength(20)]
        public string ND_ESTADO_INVENTARIO_ENDERECO { get; set; }

        [StringLength(100)]
        public string DS_ESTADO { get; set; }

        public int? NU_PROFUNDIDAD { get; set; }
    }
}
