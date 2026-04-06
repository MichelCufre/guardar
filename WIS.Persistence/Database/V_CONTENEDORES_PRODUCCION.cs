using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{
	[Table("V_CONTENEDORES_PRODUCCION")]
	public partial class V_CONTENEDORES_PRODUCCION
	{
		[Key]
		[Column(Order = 0)]
		public int NU_CONTENEDOR { get; set; }

		[Key]
		[Column(Order = 1)]
		public int NU_PREPARACION { get; set; }

		[StringLength(60)]
		[Column]
		public string DS_CONTENEDOR { get; set; }

		[StringLength(60)]
		[Column]
		public string NU_PRDC_INGRESO { get; set; }

		[StringLength(40)]
		[Column]
		public string CD_ENDERECO { get; set; }

		[Column]
		public short? CD_SITUACAO { get; set; }

		[Column]
		public int? CD_FUNCIONARIO_EXPEDICION { get; set; }
	}
}
