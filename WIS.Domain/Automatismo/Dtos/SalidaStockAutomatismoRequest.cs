using System;
using System.Collections.Generic;
using WIS.Domain.General.Filters;
using WIS.Domain.Validation;

namespace WIS.Domain.Automatismo.Dtos
{
	public class SalidaStockAutomatismoRequest : IApiAutomatismoRequest
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
		/// <example>Salida de Stock</example>
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

		/// <summary>
		/// Valor serializado. 
		/// Reservado para datos adicionales.
		/// Debe tener una estructura JSON válida.
		/// </summary>
		/// <example></example>
		[StringLengthValidation(4000, MinimumLength = 0)]
		public string Serializado { get; set; }

        public long Ejecucion { get; set; }

        public int Preparacion { get; set; }

        /// <summary>
        /// Lista de códigos de barras
        /// </summary>
        [RequiredListValidation]
		public List<SalidaStockLineaAutomatismoRequest> Detalles { get; set; }

		public SalidaStockAutomatismoRequest()
		{
			Detalles = new List<SalidaStockLineaAutomatismoRequest>();
		}

		public void Add(IApiAutomatismoItemRequest item)
		{
			Detalles.Add(item as SalidaStockLineaAutomatismoRequest);
		}
	}

	public class SalidaStockLineaAutomatismoRequest : IApiAutomatismoItemRequest
	{
		public string Pedido { get; set; }

		public int Preparacion { get; set; }

		public string CodigoAgente { get; set; }

		public string TipoAgente { get; set; }

		public string DescripcionAgente { get; set; }

		public int Prioridad { get; set; }

		public DateTime? FechaEntrega { get; set; }

		public string TipoSalida { get; set; }

		public string ValorLanzamiento { get; set; }

		public string Observacion { get; set; }

		public string Zona { get; set; }

		[SwaggerIgnore]
		public string ModoLanzamiento { get; set; }

		[SwaggerIgnore]
		public string Predio { get; set; }

		public string Producto { get; set; }

		public string Identificador { get; set; }

		public decimal Cantidad { get; set; }

		[StringLengthValidation(4000, MinimumLength = 0)]
		public string Serializado { get; set; }
		public string Agrupacion { get; set; }
		public long? Carga { get; set; }
		public string ComparteContenedorPicking { get; set; }

	}
}
