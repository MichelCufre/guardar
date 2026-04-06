using System;
using System.Collections.Generic;
using System.Text;
using WIS.Common.API.Attributes;
using WIS.Domain.General.Filters;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
	public class TransferenciaStockRequest : IApiEntradaRequest
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
		/// <example>Transferencia de Stock</example>
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
		/// Sirve para controlar la unicidad de las ejecuciones
		/// </summary>
		/// <example>123</example>
		[ApiDtoExample("123")]
		[StringLengthValidation(50, MinimumLength = 0)]
		public string IdRequest { get; set; }

		[SwaggerIgnore]
		public string IdEntrada { get; set; }

		//[SwaggerIgnore]
		//public bool UltimaOperacion { get; set; }

		[SwaggerIgnore]
		public UsuarioRequest Usuario { get; set; }

		public List<TransferenciaRequest> Transferencias { get; set; }

		public TransferenciaStockRequest()
		{
			Transferencias = new List<TransferenciaRequest>();
		}

		public void Add(IApiEntradaItemRequest item)
		{
			Transferencias.Add(item as TransferenciaRequest);
		}
	}

	public class TransferenciaRequest : IApiEntradaItemRequest
	{
		/// <summary>
		/// Ubicación de origen de donde se levantará el stock.
		/// </summary>
		/// <example>1AA00000</example>
		[RequiredValidation]
		[StringLengthValidation(40, MinimumLength = 1)]
		public string Ubicacion { get; set; }

		/// <summary>
		/// Ubicación destino donde se transferirá el stock.
		/// </summary>
		/// <example>1AA00100</example>
		[RequiredValidation]
		[StringLengthValidation(40, MinimumLength = 1)]
		public string UbicacionDestino { get; set; }

		/// <summary>
		/// Código de producto
		/// </summary>
		/// <example>PR4</example>
		[RequiredValidation]
		[StringLengthValidation(40, MinimumLength = 1)]
		public string CodigoProducto { get; set; }

		/// <summary>
		/// Serie o Lote del producto en caso que lo maneje.
		/// </summary>
		/// <example>LOTE1</example>
		[StringLengthValidation(40, MinimumLength = 0)]
		public string Identificador { get; set; }

		/// <summary>
		/// Cantidad de stock a transferir.
		/// </summary>
		/// <example>100</example>
		[RequiredValidation]
		public decimal Cantidad { get; set; }

		[SwaggerIgnore]
		public bool UltimaOperacion { get; set; }

		[SwaggerIgnore]
		public string EtiquetaOperacion { get; set; }

		[SwaggerIgnore]
		public string EtiquetaInterna { get; set; }
		[SwaggerIgnore]
		public int IdLinea { get; set; }
        public decimal CantidadSolicitada { get; set; }
    }
}
