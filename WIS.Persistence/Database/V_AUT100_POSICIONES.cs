
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_AUT100_POSICIONES")]
    public partial class V_AUT100_POSICIONES
    {

        [Key]
        [Column(Order = 0)]
        public int NU_AUTOMATISMO_POSICION { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        public int NU_AUTOMATISMO { get; set; }

        [Required]
        [StringLength(40)]
        public string ND_TIPO_ENDERECO { get; set; }

        [StringLength(100)]
        public string DS_DOMINIO_VALOR { get; set; }

        [Required]
        [StringLength(40)]
        public string VL_POSICION_EXTERNA { get; set; }

        public DateTime DT_ADDROW { get; set; }

        public DateTime? DT_UDPROW { get; set; }

        public int? TP_AGRUPACION_UBIC { get; set; }

        public short? NU_ORDEN { get; set; }

        [StringLength(1000)]
        public string VL_COMPARTE_AGRUPACION { get; set; }

    }
}