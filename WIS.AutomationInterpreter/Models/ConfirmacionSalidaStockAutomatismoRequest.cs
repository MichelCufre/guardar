using System;
using System.Collections.Generic;
using WIS.Domain.Validation;

namespace WIS.AutomationInterpreter.Models
{
	public class ConfirmacionSalidaStockAutomatismoRequest
	{
		/// <summary>
		/// Código de empresa de la ejecución
		/// </summary>
		/// <example>1</example>
		[RequiredValidation]
		public int Empresa { get; set; }

		/// <summary>
		/// Sirve para generar una referencia o un campo de búsqueda en el panel de ejecuciones de interfaces de WIS. Mediante este campo es posible identificar o buscar la traza de la ejecución de la interfaz. Ante un incidente o un procesamiento no esperado se puede reportar el problema haciendo referencia al valor de este campo.
		/// </summary>
		/// <example>Confirmación de salida de stock</example>
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

		public string IdSalida { get; set; }

		public int Preparacion { get; set; }

		public string Pedido { get; set; }

		public string CodigoAgente { get; set; }

		public string TipoAgente { get; set; }

		public string Predio { get; set; }

		public string Puesto { get; set; }


		public string Usuario { get; set; }

		/// <summary>
		/// Lista de detalles
		/// </summary>
		public List<ConfirmacionSalidaStockLineaAutomatismoRequest> Detalles { get; set; }

		public ConfirmacionSalidaStockAutomatismoRequest()
		{
			Detalles = new List<ConfirmacionSalidaStockLineaAutomatismoRequest>();
		}
	}

	public class ConfirmacionSalidaStockLineaAutomatismoRequest
	{
		public int IdLinea { get; set; }

		public string Producto { get; set; }

		public decimal Cantidad { get; set; }

		public string Identificador { get; set; }

		public DateTime? FechaVencimiento { get; set; }

		public int Contenedor { get; set; }
	}
}
