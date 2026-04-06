namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_INV_ENDERECO_DET_ERROR")]
    public partial class V_INV_ENDERECO_DET_ERROR
    {
        [Key]
        public decimal NU_ERROR { get; set; }

        [StringLength(400)]
        [Column]
        public string DS_ERROR { get; set; }

        public decimal? NU_INVENTARIO { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_INVENTARIO { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_ESTADO_INVENTARIO { get; set; }

        public decimal? NU_INVENTARIO_ENDERECO_DET { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_ESTADO_INV_ENDERECO_DET { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_INVENTARIO { get; set; }

        public decimal? QT_DIFERENCIA { get; set; }
    }
}
