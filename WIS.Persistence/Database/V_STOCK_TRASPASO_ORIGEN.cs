namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_STOCK_TRASPASO_ORIGEN")]
    public partial class V_STOCK_TRASPASO_ORIGEN
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_TRASPASO { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA_ORIGEN { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string CD_PRODUTO_ORIGEN { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR_ORIGEN { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal CD_FAIXA_ORIGEN { get; set; }

        public decimal? QT_TRASPASO_ORIGEN { get; set; }
    }
}
