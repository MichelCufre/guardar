namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_SALDO_ORDEN_COMPRA_FAC")]
    public partial class V_SALDO_ORDEN_COMPRA_FAC
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_RECEPCION_REFERENCIA { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(40)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_AGENDADA { get; set; }

        public decimal? QT_SALDO { get; set; }



    }
}
