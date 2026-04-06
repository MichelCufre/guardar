using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_CAMION_EXP050")]
    public partial class V_CAMION_EXP050
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_CAMION { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(15)]
        public string CD_PLACA_CARRO { get; set; }

        public short? CD_PORTA { get; set; }

        [StringLength(30)]
        public string DS_PORTA { get; set; }

        public short CD_SITUACAO { get; set; }

        [StringLength(30)]
        public string DS_SITUACAO { get; set; }

        public short? CD_ROTA { get; set; }

        [StringLength(30)]
        public string DS_ROTA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

    }
}
