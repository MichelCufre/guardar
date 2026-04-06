namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("I_S_ESTAN_CONF_PEDI_CAMION")]
    public partial class I_S_ESTAN_CONF_PEDI_CAMION
    {
        [StringLength(1)]
        [Column]
        public string ID_PROCESADO { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NU_INTERFAZ_EJECUCION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string NU_REGISTRO { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_CAMION { get; set; }

        [StringLength(15)]
        [Column]
        public string CD_PLACA_CARRO { get; set; }

        [StringLength(3)]
        [Column]
        public string CD_ROTA { get; set; }

        [StringLength(10)]
        [Column]
        public string CD_TRANSPORTADORA { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_CAMION { get; set; }

        [StringLength(4)]
        [Column]
        public string CD_VEICULO { get; set; }

        [StringLength(20)]
        [Column]
        public string DT_FACTURACION { get; set; }

        [Column]
        public DateTime? DT_ADDROW_INTERFAZ { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
