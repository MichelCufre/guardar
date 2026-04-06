using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Produccion
{

	public class DiferenciaInsumosConsumidos
	{
		public string Producto { get; set; }
		public decimal Teorico { get; set; }
		public decimal Consumido { get; set; }
	}

}
