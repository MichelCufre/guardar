namespace WIS.Persistence.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_EGRESO_GEN_DOCOCUMENTO_WEXP")]
    public partial class V_EGRESO_GEN_DOCOCUMENTO_WEXP
    {
        [Key]
        [Column(Order = 0)]
        public int NU_PREPARACION { get; set; }

        [Key]
        [Column(Order = 1)]
        public int CD_CAMION { get; set; }

        [Key]
        [Column(Order = 2)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 3)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 5)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_PRODUTO { get; set; }
    }
}
