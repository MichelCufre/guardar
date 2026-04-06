using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

	[Table("V_FOTO_STOCK_WSTO310")]
	public partial class V_FOTO_STOCK_WSTO310
	{

		[StringLength(10)]
		public string DT_ADDROW { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(10)]
		public string NU_FOTO { get; set; }

		[Key]
		[Column(Order = 2)]
		[StringLength(10)]
		public string NU_PREDIO { get; set; }

		[Key]
		[Column(Order = 3)]
		public int CD_EMPRESA { get; set; }

		[Key]
		[Column(Order = 4)]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Key]
		[Column(Order = 5)]
		public decimal CD_FAIXA { get; set; }

		[Key]
		[Column(Order = 6)]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		public decimal? QT_ESTOQUE { get; set; }

		public decimal? QT_FACT_SIN_ENVIAR { get; set; }

		public decimal? QT_REC_SIN_CERRAR { get; set; }

		public decimal? QT_AVERIA { get; set; }

		public decimal? QT_CTRL_CALIDAD { get; set; }

		[StringLength(20)]
		public string CD_MOTIVO_AVERIA { get; set; }

	}
}
