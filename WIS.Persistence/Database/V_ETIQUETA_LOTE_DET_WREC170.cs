using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{
    [Table("V_ETIQUETA_LOTE_DET_WREC170")]
    public partial class V_ETIQUETA_LOTE_DET_WREC170
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_ETIQUETA_LOTE { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string NU_EXTERNO_ETIQUETA { get; set; }


        [Key]
        [Column(Order = 3)]
        [StringLength(3)]
        public string TP_ETIQUETA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 6)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_SUGERIDO { get; set; }

        public decimal? QT_ETIQUETA_GENERADA { get; set; }

        public decimal? QT_PRODUTO_RECIBIDO { get; set; }


    }
}




