namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("V_POTERIA_VEHICULO_CAMION")]
    public partial class V_POTERIA_VEHICULO_CAMION
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PORTERIA_VEHICULO { get; set; }

        [Required]
        [StringLength(15)]
        [Column]
        public string ND_TRANSPORTE { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_TRANSPORTE { get; set; }

        [StringLength(100)]
        [Column]
        public string DS_TRANSPORTE { get; set; }

        [StringLength(10)]
        [Column]
        public string VL_MATRICULA_1 { get; set; }

        [StringLength(10)]
        [Column]
        public string VL_MATRICULA_2 { get; set; }

        public decimal? VL_PESO_ENTRADA { get; set; }

        public decimal? VL_PESO_SALIDA { get; set; }

        public int? CD_EMPRESA { get; set; }

        public DateTime? DT_PORTERIA_ENTRADA { get; set; }

        public DateTime? DT_PORTERIA_SALIDA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_PORTERIA_VEHICULO_CAMION { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_CAMION { get; set; }

        [StringLength(15)]
        [Column]
        public string CD_PLACA_CARRO { get; set; }

        public short? CD_ROTA { get; set; }

        public short CD_SITUACAO { get; set; }

        public short? CD_PORTA { get; set; }

        [Required]
        [StringLength(1)]
        [Column]
        public string ID_RESPETA_ORD_CARGA { get; set; }

        public int CD_TRANSPORTADORA { get; set; }

        public DateTime? DT_ADDROW_CAMION { get; set; }

        public DateTime? DT_UPDROW_CAMION { get; set; }

        public int? CD_EMPRESA_CAMION { get; set; }

        public DateTime? DT_PROGRAMADO { get; set; }

        [StringLength(50)]
        [Column]
        public string DS_CAMION { get; set; }

        public short? CD_VEICULO { get; set; }

        public DateTime? DT_FACTURACION { get; set; }

        public int? CD_FUNC_CIERRE { get; set; }

        public DateTime? DT_CIERRE { get; set; }

        public long? NU_INTERFAZ_EJECUCION { get; set; }

        public long? NU_INTERFAZ_EJECUCION_FACT { get; set; }
    }
}
