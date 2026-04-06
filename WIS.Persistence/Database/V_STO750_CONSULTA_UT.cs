using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;

namespace WIS.Persistence.Database
{
    [Table("V_STO750_CONSULTA_UT")]
    public partial class V_STO750_CONSULTA_UT
    {
        [Key]
        [Column(Order = 0)]
        public int NU_UNIDAD_TRANSPORTE { get; set; }

        [StringLength(40)]
        [Column]
        public string NU_EXTERNO_UNIDAD { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_UNIDAD_TRANSPORTE { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_SITUACAO { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_BARRAS { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_GRUPO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_SUGERIDO { get; set; }

        public decimal? PS_REAL { get; set; }

        public decimal? VL_ALTURA { get; set; }

        public decimal? VL_LARGURA { get; set; }

        public decimal? VL_PROFUNDIDADE { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_UNIDAD_BULTO { get; set; }

        [StringLength(10)]
        [Column]
        public string QT_BULTO { get; set; }

        public decimal? QT_CONTENEDOR { get; set; }
    }
}
