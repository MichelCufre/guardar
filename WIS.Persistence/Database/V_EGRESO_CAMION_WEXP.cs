namespace WIS.Persistence.Database
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    [Table("V_EGRESO_CAMION_WEXP")]
    public partial class V_EGRESO_CAMION_WEXP
    {
        public int? NU_CONTENEDOR { get; set; }
        [Key]
        [Column(Order = 0)]
        public int CD_CAMION { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CD_CLIENTE { get; set; }
        [Key]
        [Column(Order = 2)]
        public int CD_EMPRESA { get; set; }
        [Key]
        [Column(Order = 1)]
        public string CD_PRODUTO { get; set; }
        public string DS_PRODUTO { get; set; }
        [Key]
        [Column(Order = 5)]
        public decimal CD_FAIXA { get; set; }
        [Key]
        [Column(Order = 4)]
        public string NU_IDENTIFICADOR { get; set; }
        public decimal? QT_PRODUTO { get; set; }
        public string NU_PREDIO { get; set; }
        public string TP_DOCUMENTO { get; set; }

		[StringLength(50)]
		[Column]
		public string ID_EXTERNO_CONTENEDOR { get; set; }
	}
}
