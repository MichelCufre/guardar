using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WIS.Persistence.Database;

namespace WIS.Persistence.Database
{
    [Table("T_PRDC_LINEA")]
    public partial class T_PRDC_LINEA
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
        public string CD_ENDERECO_SALIDA { get; set; }

		[StringLength(40)]
		[Column]
		public string CD_ENDERECO_SALIDA_TRAN { get; set; }
        
        [StringLength(10)]
        [Column]
        public string NU_PRDC_INGRESO { get; set; }

        [StringLength(40)]
        [Column]
        public string CD_ENDERECO_PRODUCCION { get; set; }


		[StringLength(20)]
        [Column]
        public string ND_TIPO_LINEA { get; set; }

        [StringLength(2)]
        [Column]
        public string NU_PREDIO { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_CONF_MAN { get; set; }

        [StringLength(1)]
        [Column]
        public string FL_STOCK_CONSUMIBLE { get; set; }

		public long? NU_TRANSACCION { get; set; }

		[ForeignKey("CD_ENDERECO_ENTRADA")]
		public virtual T_ENDERECO_ESTOQUE T_ENDERECO_ESTOQUE_ENTRADA { get; set; }

		[ForeignKey("CD_ENDERECO_SALIDA")]
		public virtual T_ENDERECO_ESTOQUE T_ENDERECO_ESTOQUE_SALIDA { get; set; }

		[ForeignKey("CD_ENDERECO_SALIDA_TRAN")]
		public virtual T_ENDERECO_ESTOQUE T_ENDERECO_ESTOQUE_SALIDA_TRAN { get; set; }
		
        [ForeignKey("CD_ENDERECO_PRODUCCION")]
		public virtual T_ENDERECO_ESTOQUE T_ENDERECO_ESTOQUE_PRODUCCION { get; set; }


	}
}
