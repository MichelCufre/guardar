namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_PRE250_REGLA_LIBERACION")]
    public partial class V_PRE250_REGLA_LIBERACION
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NU_REGLA { get; set; }

        [StringLength(200)]
        [Column]
        public string DS_REGLA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_ACTIVE { get; set; }

        public short? NU_ORDEN { get; set; }

        [Column]
        public string DT_INICIO { get; set; }

        [Column]
        public string DT_FIN { get; set; }


        [StringLength(200)]
        [Column]
        public string DS_DIAS { get; set; }

        public long? HR_INICIO { get; set; }

        public long? HR_FIN { get; set; }

        public int? NU_FRECUENCIA { get; set; }

        [StringLength(50)]
        [Column]
        public string TP_FRECUENCIA { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_PALLET_COMPLETO { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_STOCK { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_CONTROLA_STOCK { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_PALLET_INCOMPLETO { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_ORDEN_PEDIDOS { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_RESPETAR_FIFO { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_PREPARAR_SOLO_CAMION { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_REPARTIR_ESCASEZ { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_LIBERAR_POR_UNIDAD { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_AGRUPAR_POR_CAMION { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_AGRUPACION { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_LIBERAR_POR_CURVAS { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_EMPRESA { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CD_ONDA { get; set; }

        [StringLength(3)]
        [Column]
        public string TP_AGENTE { get; set; }

        [StringLength(10)]
        [Column]
        public string NU_PREDIO { get; set; }

        public DateTime? DT_ULTIMA_EJECUCION { get; set; }

        [StringLength(50)]
        [Column]
        public string CD_ORDEN_PEDIDOS_AUTO { get; set; }

        public short? NU_CLIS_POR_PREPARACION { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_SOLO_PEDIDOS_NUEVOS { get; set; }

        [StringLength(30)]
        [Column]
        public string DS_ONDA { get; set; }

        [StringLength(55)]
        [Column]
        public string NM_EMPRESA { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_RESPETAR_INTERVALO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_EXCLUIR_UBICACIONES_PICKING { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_USAR_SOLO_STK_PICKING { get; set; }

        [StringLength(3)]
        public string DS_DIAS_COLA_TRABAJO { get; set; }

    }
}
