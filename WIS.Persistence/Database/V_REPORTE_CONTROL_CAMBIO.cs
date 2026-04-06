using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

    [Table("V_REPORTE_CONTROL_CAMBIO")]
    public partial class V_REPORTE_CONTROL_CAMBIO
    {

        [Key]
        [Column(Order = 0)]
        public int NU_PREPARACION { get; set; }

        [Key]
        [Column(Order = 1)]
        public int NU_CONTENEDOR { get; set; }

        [Key]
        [Column(Order = 2)]
        public int CD_CAMION { get; set; }

        public int? QT_BULTO { get; set; }

        [StringLength(50)]
        public string DS_TIPO_CONTENEDOR { get; set; }

        [StringLength(15)]
        public string CD_PLACA_CARRO { get; set; }

        public int CD_TRANSPORTADORA { get; set; }

        public DateTime? DT_CIERRE { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [Key]
        [Column(Order = 9)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [StringLength(3)]
        public string TP_AGENTE { get; set; }

        [StringLength(40)]
        public string CD_AGENTE { get; set; }

        [StringLength(100)]
        public string DS_CLIENTE { get; set; }

        [StringLength(100)]
        public string DS_TRANSPORTADORA { get; set; }

        [StringLength(50)]
        [Column]
        public string ID_EXTERNO_CONTENEDOR { get; set; }

        public long? NU_LPN { get; set; }

    }
}
