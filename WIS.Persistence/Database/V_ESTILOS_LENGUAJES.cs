using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
	[Table("V_ESTILOS_LENGUAJES")]
	public partial class V_ESTILOS_LENGUAJES
	{
		[Key]
		[StringLength(15)]
		[Column]
		public string CD_LABEL_ESTILO { get; set; }

		[StringLength(30)]
		[Column]
		public string DS_LABEL_ESTILO { get; set; }

		[StringLength(20)]
		[Column]
		public string TP_LABEL { get; set; }

		[StringLength(100)]
		[Column]
		public string DS_DOMINIO_VALOR { get; set; }

		[Key]
		[StringLength(10)]
		[Column]
		public string CD_LENGUAJE_IMPRESION { get; set; }

		[StringLength(10)]
		[Column]
		public string TP_CONTENEDOR { get; set; }

		[Required]
		[StringLength(1)]
		[Column]
		public string FL_HABILITADO { get; set; }

		[StringLength(50)]
		[Column]
		public string DS_TIPO_CONTENEDOR { get; set; }

	}
}