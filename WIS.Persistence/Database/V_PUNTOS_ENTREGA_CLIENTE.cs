using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Persistence.Database
{
    public class V_PUNTOS_ENTREGA_CLIENTE
	{
		[Key]
		[Column(Order = 1)]
		public string CD_CLIENTE { get; set; }
		[Key]
		[Column(Order = 2)]
		public int CD_EMPRESA { get; set; }

		[Column]
        public string CD_AGENTE { get; set; }
		[Column]
        public string TP_AGENTE { get; set; }


		[Column]
        public string CD_PUNTO_ENTREGA_CLIENTE { get; set; }
		[Column]
		public string DS_ENDERECO_CLIENTE { get; set; }
		[Column]
		public string CD_PUNTO_ENTREGA_PEDIDO { get; set; }
		[Column]
		public string DS_ENDERECO_PEDIDO { get; set; }
	}
}
