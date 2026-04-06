using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Produccion
{

	public class DiferenciaInsumosProducidos
	{
		public string Producto { get; set; }
		public decimal Teorico { get; set; }
		public decimal Producido { get; set; }
	}

}
