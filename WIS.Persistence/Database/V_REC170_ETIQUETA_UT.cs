
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("V_REC170_ETIQUETA_UT")]
    public partial class V_REC170_ETIQUETA_UT
    {

        [Key]
        [Column(Order = 0)]
        public int NU_UNIDAD_TRANSPORTE { get; set; }

        [StringLength(40)]
        public string NU_EXTERNO_UNIDAD { get; set; }

        [Required]
        [StringLength(3)]
        public string TP_ETIQUETA { get; set; }

        public int NU_AGENDA { get; set; }

        [StringLength(40)]
        public string CD_ENDERECO { get; set; }

        public short? CD_SITUACAO { get; set; }

        [Required]
        [StringLength(30)]
        public string DS_SITUACAO { get; set; }

        public DateTime? DT_RECEPCION { get; set; }

        public DateTime? DT_ALMACENAMIENTO { get; set; }

        [StringLength(3)]
        public string TP_AGENTE { get; set; }

        [StringLength(40)]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        public string DS_AGENTE { get; set; }

        [Key]
        [Column(Order = 12)]
        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

    }
}