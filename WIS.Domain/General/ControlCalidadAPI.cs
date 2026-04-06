using System;
using System.Collections.Generic;
using WIS.Domain.General.Enums;

namespace WIS.Domain.General
{
	public class ControlCalidadAPI
	{
		public int Id { get; set; }
		public int Empresa { get ; set ; }
		public int CodigoControlCalidad { get; set; }
		public ControlCalidadOperacion Estado { get; set; }
		public string Descripcion { get; set; }

		public List <CriterioControlCalidadAPI> Criterios { get; set; }
	}

	public class CriterioControlCalidadAPI
	{
		public int Id { get; set; }
		public int Empresa { get ; set ; }
		public string Predio { get; set; }

		// Key para identificar
		public string Producto { get; set; }
		public string Lote { get; set; }

		public decimal Faixa { get; set; }

		// Filtro por los siguientes atributos
		public string EtiquetaExterna { get; set; }
		public string TipoEtiquetaExterna { get; set; }
		public string Ubicacion { get; set; }
		public ControlCalidadCriterio Operacion { get; set; }
      
	}
}
