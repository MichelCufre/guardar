namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_STOCK_TRASPASO_DEST")]
    public partial class V_STOCK_TRASPASO_DEST
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_TRASPASO { get; set; }

        public int? CD_EMPRESA_DEST { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_PRODUTO_DEST { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_IDENTIFICADOR_DEST { get; set; }

        public decimal? CD_FAIXA_DEST { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(40)]
        public string CD_PRODUTO_ORIGEN { get; set; }

        public decimal? QT_TRASPASO_DEST { get; set; }
    }
}
