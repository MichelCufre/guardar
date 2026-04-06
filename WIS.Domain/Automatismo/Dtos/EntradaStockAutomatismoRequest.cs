using System;
using System.Collections.Generic;
using WIS.Domain.General.Filters;
using WIS.Domain.Validation;

namespace WIS.Domain.Automatismo.Dtos
{
	public class EntradaStockAutomatismoRequest : IApiAutomatismoRequest
	{
		/// <summary>
		/// Código de empresa de la ejecución
		/// </summary>
		/// <example>1</example>
		[RequiredValidation]
		[ExisteEmpresaValidation()]
		public int Empresa { get; set; }

		/// <summary>
		/// Sirve para generar una referencia o un campo de búsqueda en el panel de ejecuciones de interfaces de WIS. Mediante este campo es posible identificar o buscar la traza de la ejecución de la interfaz. Ante un incidente o un procesamiento no esperado se puede reportar el problema haciendo referencia al valor de este campo.
		/// </summary>
		/// <example>Entrada de stock</example>
		[RequiredValidation]
		[StringLengthValidation(200, MinimumLength = 1)]
		public string DsReferencia { get; set; }

		/// <summary>
		/// Generalmente se utiliza en sistemas externos que generan archivos y un middleware que oficia de intermediario entre el archivo y el API. 
		/// En este campo se puede guardar el nombre del archivo original en el cual vinieron los datos. En caso de que la implantación no utilice archivos se puede utilizar con otros fines.
		/// </summary>
		/// <example>Archivo</example>
		[StringLengthValidation(100)]
		public string Archivo { get; set; }

		public long Ejecucion { get; set; }

		public string Ubicacion { get; set; }

		[SwaggerIgnore]
		public string Predio { get; set; }

		/// <summary>
		/// Valor serializado. 
		/// Reservado para datos adicionales.
		/// Debe tener una estructura JSON válida.
		/// </summary>
		/// <example></example>
		[StringLengthValidation(4000, MinimumLength = 0)]
		public string Serializado { get; set; }

		/// <summary>
		/// Lista de detalles
		/// </summary>
		[RequiredListValidation]
		public List<EntradaStockLineaAutomatismoRequest> Detalles { get; set; }

		public EntradaStockAutomatismoRequest()
		{
			Detalles = new List<EntradaStockLineaAutomatismoRequest>();
		}

		public void Add(IApiAutomatismoItemRequest item)
		{
			Detalles.Add(item as EntradaStockLineaAutomatismoRequest);
		}
	}

	public class EntradaStockLineaAutomatismoRequest : IApiAutomatismoItemRequest
	{
		public string Producto { get; set; }

		public string ProductoExterno { get; set; }

		public string Identificador { get; set; }

		public string EtiquetaCarro { get; set; }

		public string EtiquetaPosicion { get; set; }

		public decimal Cantidad { get; set; }

		public DateTime? FechaVencimiento { get; set; }

        public string ManejoVencimiento { get; set; }

        public string CodigoAgente { get; set; }

		public string TipoAgente { get; set; }

		public string DescripcionAgente { get; set; }

		[StringLengthValidation(4000, MinimumLength = 0)]
		public string Serializado { get; set; }

		public string EtiquetaOperacion { get; set; }

		public string EtiquetaInterna { get; set; }

		public int LineaEntrada { get; set; }
	}
}
