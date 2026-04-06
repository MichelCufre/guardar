namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_STOCK_TOTAL")]
    public partial class V_STOCK_TOTAL
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 2)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_FISICO_SL { get; set; }

        public decimal? QT_FISICO_SD { get; set; }

        public decimal? QT_SALIDA_SL { get; set; }

        public decimal? QT_SALIDA_SD { get; set; }

        public decimal? QT_ENTRADA_SL { get; set; }

        public decimal? QT_ENTRADA_SD { get; set; }

        public decimal? QT_LIBRE { get; set; }

        public decimal? QT_DISPONIBLE { get; set; }
    }
}
