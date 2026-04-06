namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_INV_CONTROL_MONO_PROD_LOTE")]
    public partial class V_INV_CONTROL_MONO_PROD_LOTE
    {
        [Key]
        public decimal NU_INVENTARIO_ENDERECO_DET { get; set; }

        public decimal NU_INVENTARIO_ENDERECO { get; set; }

        public decimal NU_INVENTARIO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO { get; set; }

        public decimal? CD_FAIXA { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_ESTOQUE { get; set; }

        public decimal? QT_DIFERENCIA { get; set; }

        public decimal? QT_SALDO { get; set; }
    }
}
