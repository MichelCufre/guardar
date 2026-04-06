
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("T_UNIDAD_TRANSPORTE")]
    public partial class T_UNIDAD_TRANSPORTE
    {

        [Key]
        [Column(Order = 0)]
        public int NU_UNIDAD_TRANSPORTE { get; set; }

        [StringLength(40)]
        public string NU_EXTERNO_UNIDAD { get; set; }

        [StringLength(3)]
        public string TP_UNIDAD_TRANSPORTE { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        public short? CD_SITUACAO { get; set; }

        [StringLength(50)]
        public string CD_BARRAS { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(50)]
        public string CD_GRUPO { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO_SUGERIDO { get; set; }

        public decimal? PS_REAL { get; set; }

        public decimal? VL_ALTURA { get; set; }

        public decimal? VL_LARGURA { get; set; }

        public decimal? VL_PROFUNDIDADE { get; set; }

        [StringLength(10)]
        public string CD_UNIDAD_BULTO { get; set; }

        [StringLength(10)]
        public string QT_BULTO { get; set; }

        public decimal? QT_CONTENEDOR { get; set; }

    }
}
