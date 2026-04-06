using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_REPORTE_PACKING_LIST")]
    public partial class V_REPORTE_PACKING_LIST
    {

        [Key]
        [Column(Order = 0)]
        public int CD_CAMION { get; set; }

        public int? CD_EMPRESA { get; set; }

        [StringLength(15)]
        public string CD_PLACA_CARRO { get; set; }

        public short? CD_ROTA { get; set; }

        public int CD_TRANSPORTADORA { get; set; }

        [StringLength(50)]
        public string DS_CAMION { get; set; }

        public int? CD_VEICULO { get; set; }

        public DateTime? DT_CIERRE { get; set; }

        [StringLength(100)]
        public string DS_TRANSPORTADORA { get; set; }

        [StringLength(30)]
        public string DS_ROTA { get; set; }

        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

    }
}
