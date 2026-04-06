
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_STO730_AUDITORIA_LPN")]
    public partial class V_STO730_AUDITORIA_LPN
    {

        [Key]
        [Column(Order = 0)]
        public long NU_AUDITORIA_AGRUPADOR { get; set; }

        public long NU_LPN { get; set; }

        [Required]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        public decimal CD_FAIXA { get; set; }

        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [Required]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        [StringLength(6)]
        public string ID_ESTADO { get; set; }

        [Required]
        [StringLength(100)]
        public string DS_ESTADO { get; set; }

        [StringLength(50)]
        public string ID_LPN_EXTERNO { get; set; }

        [StringLength(10)]
        public string TP_LPN_TIPO { get; set; }

    }
}
