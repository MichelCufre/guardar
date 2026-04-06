using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WIS.Persistence.Database
{
    [Table("V_DISPONIBLE_CROSS_DOCKING")]
    public partial class V_DISPONIBLE_CROSS_DOCKING
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_AGENDA { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_ETIQUETA_LOTE { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        
        [Key]
        [Column(Order = 5)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Key]
        [Column(Order = 6)]
        public decimal CD_FAIXA { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public decimal? QT_DISPONIBLE { get; set; }

        [StringLength(1)]
        [Column]
        public string ID_MANEJO_IDENTIFICADOR { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
