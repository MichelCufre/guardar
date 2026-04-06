namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("T_PRDC_LINEA_CONSUMIDO")]
    public partial class T_PRDC_LINEA_CONSUMIDO
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(10)]
        public string CD_PRDC_DEFINICION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string NU_PRDC_INGRESO { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_FORMULA { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PASADA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 5)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_CONSUMIDO { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        [StringLength(1)]
        public string FL_SEMIACABADO { get; set; }

        [StringLength(1)]
        public string FL_CONSUMIBLE { get; set; }
    }
}
