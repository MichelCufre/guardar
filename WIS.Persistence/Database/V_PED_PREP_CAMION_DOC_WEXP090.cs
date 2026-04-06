using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WIS.Persistence.Database
{

	[Table("V_PED_PREP_CAMION_DOC_WEXP090")]
	public partial class V_PED_PREP_CAMION_DOC_WEXP090
	{

		[Key]
		[Column(Order = 0)]
		[StringLength(40)]
		public string NU_PEDIDO { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(10)]
		public string CD_CLIENTE { get; set; }

		[Key]
		[Column(Order = 2)]
		public int? CD_EMPRESA { get; set; }

		[Key]
		[Column(Order = 3)]
		public int? NU_PREPARACION { get; set; }

		[Key]
		[Column(Order = 4)]
		public long? NU_CARGA { get; set; }

		[Key]
		[Column(Order = 5)]
		public int? CD_CAMION { get; set; }

		[StringLength(6)]
		public string TP_DOCUMENTO { get; set; }

		[StringLength(10)]
		public string NU_DOCUMENTO { get; set; }

		[StringLength(6)]
		public string ID_ESTADO { get; set; }

		public short? CD_SITUACAO { get; set; }

		[StringLength(100)]
		public string DS_CLIENTE { get; set; }

		[StringLength(55)]
		public string NM_EMPRESA { get; set; }

		[StringLength(10)]
		public string NU_PREDIO { get; set; }

		[StringLength(50)]
		public string DS_DOCUMENTO { get; set; }

		[StringLength(15)]
		public string CD_PLACA_CARRO { get; set; }

	}
}
