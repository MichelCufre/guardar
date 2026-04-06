
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;


namespace WIS.Persistence.Database
{

    [Table("T_ALM_DET_PRODUTO_TEMP")]
    public partial class T_ALM_DET_PRODUTO_TEMP
    {

        public int NU_ETIQUETA_LOTE { get; set; }

        public int USERID { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal CD_FAIXA { get; set; }

        public DateTime? DT_FABRICACAO { get; set; }

        public decimal? VL_ALTURA { get; set; }

        public decimal? VL_LARGURA { get; set; }

        public decimal? VL_PROFUNDIDADE { get; set; }

        public decimal? VL_CUBAGEM { get; set; }

        public decimal? PS_BRUTO { get; set; }

        public decimal? QT_ESTOQUE { get; set; }

        public decimal QT_UND_BULTO { get; set; }

        [Required]
        [StringLength(2)]
        public string CD_CLASSE { get; set; }
        public decimal? QT_UNIDADES { get; set; }
       
    }
}