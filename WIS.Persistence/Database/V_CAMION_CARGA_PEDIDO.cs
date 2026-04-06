namespace WIS.Persistence.Database
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	[Table("V_CAMION_CARGA_PEDIDO")]
	public partial class V_CAMION_CARGA_PEDIDO
	{
		[Key]
		[Column(Order = 0)]
		public int CD_CAMION { get; set; }

		[Key]
		[Column(Order = 1)]
		public long NU_CARGA { get; set; }

		[Key]
		[Column(Order = 2)]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		[Key]
		[Column(Order = 3)]
		public int CD_EMPRESA { get; set; }

		[Key]
		[Column(Order = 4)]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

        [Key]
        [Column(Order = 5)]
        public int NU_PREPARACION { get; set; }
    }
}
