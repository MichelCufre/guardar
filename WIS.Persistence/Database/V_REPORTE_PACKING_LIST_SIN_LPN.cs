using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
    [Table("V_REPORTE_PACKING_LIST_SIN_LPN")]
    public partial class V_REPORTE_PACKING_LIST_SIN_LPN
    {
        [Key]
        [Column(Order = 0)]
        public int CD_CAMION { get; set; }

        [Key]
        [Column(Order = 1)]
        public int NU_PREPARACION { get; set; }

        [Key]
        [Column(Order = 3)]
        public int CD_EMPRESA { get; set; }

        [Required]
        [StringLength(55)]
        public string NM_EMPRESA { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(40)]
        public string NU_PEDIDO { get; set; }

        public decimal? QT_PRODUTO { get; set; }

        public decimal? QT_PREPARADO { get; set; }

        public int? QT_BULTO_CONTENEDOR { get; set; }

        [Key]
        [Column(Order = 2)]
        public int NU_CONTENEDOR { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(40)]
        public string NU_IDENTIFICADOR { get; set; }

        public DateTime? VENCIMIENTO { get; set; }

        [Required]
        [StringLength(50)]
        public string DS_TIPO_CONTENEDOR { get; set; }

        [StringLength(400)]
        public string DS_ENDERECO { get; set; }

        [StringLength(100)]
        public string CLI_DS_ENDERECO { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(10)]
        public string CD_CLIENTE { get; set; }

        [StringLength(100)]
        public string DS_CLIENTE { get; set; }

        [StringLength(40)]
        public string CD_AGENTE { get; set; }

        [StringLength(3)]
        public string TP_AGENTE { get; set; }

        [StringLength(30)]
        public string NU_TELEFONE { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(40)]
        public string CD_PRODUTO { get; set; }

        [Required]
        [StringLength(65)]
        public string DS_PRODUTO { get; set; }

        [StringLength(50)]
        public string DS_CAMION { get; set; }

        public DateTime? FECHA_EGRESO { get; set; }

        public int CD_TRANSPORTADORA { get; set; }

        [StringLength(118)]
        public string DS_TRANSPORTADORA { get; set; }

        [StringLength(15)]
        public string CD_PLACA_CARRO { get; set; }

        [StringLength(10)]
        public string NU_PREDIO { get; set; }

        public short? CD_ROTA { get; set; }

        [StringLength(30)]
        public string DS_ROTA { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(14)]
        public string VL_VENCIMIENTO { get; set; }

        [StringLength(50)]
        public string ID_EXTERNO_CONTENEDOR { get; set; }
    }
}
