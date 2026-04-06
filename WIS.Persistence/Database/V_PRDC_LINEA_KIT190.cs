namespace WIS.Persistence.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    [Table("V_PRDC_LINEA_KIT190")]
    public partial class V_PRDC_LINEA_KIT190
    {
        [Key]
        [StringLength(10)]
        [Column]
        public string CD_PRDC_LINEA { get; set; }

        [StringLength(2000)]
        [Column]
        public string DS_PRDC_LINEA { get; set; }

        public DateTime? DT_ADDROW { get; set; }

        public DateTime? DT_UPDROW { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_ENTRADA { get; set; }

		[StringLength(40)]
		[Column]
		public string CD_ENDERECO_BLACKBOX { get; set; }

		[StringLength(40)]
        [Column]
        public string CD_ENDERECO_SALIDA { get; set; }

		[StringLength(40)]
		[Column]
		public string CD_ENDERECO_SALIDA_TRAN { get; set; }

		[StringLength(10)]
        [Column]
        public string NU_PRDC_INGRESO { get; set; }

        [StringLength(20)]
        [Column]
        public string ND_TIPO_LINEA { get; set; }

        [StringLength(20)]
        [Column]
        public string CD_TIPO_LINEA { get; set; }

		[StringLength(40)]
		[Column]
		public string FL_CONF_MAN { get; set; }

		[StringLength(40)]
		[Column]
		public string FL_STOCK_CONSUMIBLE { get; set; }

		[StringLength(2)]
        [Column]
        public string NU_PREDIO { get; set; }
    }
}
