using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WIS.Persistence.Database
{

    [Table("T_LPN_DET_ATRIBUTO")]
	public partial class T_LPN_DET_ATRIBUTO
	{

		[Key]
		[Column(Order = 0)]
		public int ID_LPN_DET { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(10)]
		public string TP_LPN_TIPO { get; set; }

		[Key]
		[Column(Order = 2)]
		public int ID_ATRIBUTO { get; set; }

		[StringLength(400)]
		public string VL_LPN_DET_ATRIBUTO { get; set; }

		[Key]
		[Column(Order = 4)]
		[StringLength(40)]
		public string CD_PRODUTO { get; set; }

		[Key]
		[Column(Order = 5)]
		public decimal CD_FAIXA { get; set; }

		[Key]
		[Column(Order = 6)]
		public int CD_EMPRESA { get; set; }

		[Key]
		[Column(Order = 7)]
		[StringLength(40)]
		public string NU_IDENTIFICADOR { get; set; }

		[Key]
		[Column(Order = 8)]
		public long NU_LPN { get; set; }

        public long? NU_TRANSACCION { get; set; }

        public long? NU_TRANSACCION_DELETE { get; set; }

        [StringLength(6)]
		public string ID_ESTADO { get; set; }

		public T_ATRIBUTO T_ATRIBUTO { get; set; }
	}
}
