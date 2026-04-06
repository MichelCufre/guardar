using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_DET_ETIQUETA_SIN_CLASIFICAR")]
    public partial class V_DET_ETIQUETA_SIN_CLASIFICAR
    {

        [Key]
        [Column(Order = 0)]
        public int NU_ETIQUETA_LOTE { get; set; }

        [Key]
        [Column(Order = 1)]
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

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [Key]
        [Column(Order = 7)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 8)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }
    }
}
