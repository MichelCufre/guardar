namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("V_CROSS_DOCK_TEMP_WREC220")]
    public partial class V_CROSS_DOCK_TEMP_WREC220
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_ROTA { get; set; }

        public short? CD_ONDA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal CD_FAIXA { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(1)]
        public string ID_ESPECIFICA_IDENTIFICADOR { get; set; }
    }
}
