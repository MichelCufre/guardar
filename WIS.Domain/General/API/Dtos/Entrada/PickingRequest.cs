using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WIS.Common.API.Attributes;
using WIS.Domain.General.Filters;
using WIS.Domain.Validation;

namespace WIS.Domain.General.API.Dtos.Entrada
{
	public class PickingRequest : IApiEntradaRequest
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
		/// <example>Preparación de Picking</example>
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
		/// Estado de detalle de picking.
		/// Necesario para el modulo de Automatismo
		/// Valores posibles: 'ESTAD_PREP_PEND' - 'ESTAD_PEND_AUT'
		/// </summary>
		/// <example>1AA00000</example>
		[SwaggerIgnore]
		[StringLengthValidation(20, MinimumLength = 0)]
		public string EstadoDetalle { get; set; }

		[SwaggerIgnore]
        public int EstadoSalida { get; set; }

        [JsonIgnore]
		public int Preparacion { get; set; }


		/// <summary>
		/// Sirve para controlar la unicidad de las ejecuciones
		/// </summary>
		/// <example>123</example>
		[ApiDtoExample("123")]
		[StringLengthValidation(50, MinimumLength = 0)]
		public string IdRequest { get; set; }

		/// <summary>
		/// Lista de pickeos.         
		/// </summary>
		public List<DetallePickingRequest> Detalles { get; set; }
        /// <summary>
        /// Lista de pickeos finalizados.         
        /// </summary>
        public List<DetallePickingFinalizadoRequest> DetallesFinalizados { get; set; }

        [SwaggerIgnore]
		public UsuarioRequest Usuario { get; set; }

		public PickingRequest()
		{
			Detalles = new List<DetallePickingRequest>();
		}
		public void Add(IApiEntradaItemRequest item)
		{
			Detalles.Add(item as DetallePickingRequest);
		}
	}

	public class DetallePickingRequest : IApiEntradaItemRequest
	{
		/// <summary>
		/// Número de preparacion
		/// </summary>
		/// <example>2</example>
		[RequiredValidation]
		public int Preparacion { get; set; }

		/// <summary>
		/// Ubicación donde fue liberado el stock.
		/// </summary>
		/// <example>1AA00000</example>
		[RequiredValidation]
		[StringLengthValidation(40, MinimumLength = 1)]
		public string Ubicacion { get; set; }

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

		/// <summary>
		/// Número de contenedor.
		/// </summary>
		/// <example>100</example>
		[RequiredValidation]
        [StringLengthValidation(50, MinimumLength = 1)]
		public string IdExternoContenedor { get; set; }

        /// <summary>
        /// Tipo de contenedor
        /// </summary>
        /// <example>W</example>
        [RequiredValidation]
		[StringLengthValidation(10, MinimumLength = 1)]
		public string TipoContenedor { get; set; }

		/// <summary>
		/// Ubicación destino del contenedor
		/// </summary>
		/// <example>1ZF000101</example>
		[SwaggerIgnore]
		[StringLengthValidation(40, MinimumLength = 0)]
		public string UbicacionContenedor { get; set; }

		/// <summary>
		/// Agrupación para pickear. 
		/// Debe corresponder a la agrupacón de la preparación.
		/// </summary>
		/// <example>P</example>
		[SwaggerIgnore]
		[StringLengthValidation(1, MinimumLength = 0)]
		public string Agrupacion { get; set; }

		/// <summary>
		/// Código de agente. Requerido cuando la agrupación de la preparación es P (Pedido) y C (Cliente).
		/// </summary>
		/// <example>AGE01</example>
		[StringLengthValidation(40, MinimumLength = 0)]
		public string CodigoAgente { get; set; }

		/// <summary>
		/// Tipo de Agente. Requerido cuando la agrupación de la preparación es P (Pedido) y C (Cliente).         
		/// </summary>
		/// <example>CLI</example>
		[StringLengthValidation(3, MinimumLength = 0)]
		public string TipoAgente { get; set; }

		/// <summary>
		/// Nro de pedido. Requerido cuando la agrupación de la preparación es P (Pedido).
		/// </summary>
		/// <example>PED100</example>
		[StringLengthValidation(40, MinimumLength = 0)]
		public string Pedido { get; set; }

		/// <summary>
		/// Nro. de Carga. Requerido cuando la agrupación de la preparación es R (Ruta).
		/// </summary>
		/// <example>123</example>
		public long? Carga { get; set; }

		/// <summary>
		/// Nro de pedido. Requerido cuando la agrupación de la preparación es C (Cliente), R (Ruta) y O (Onda).
		/// </summary>
		/// <example>REF001</example>
		[StringLengthValidation(200, MinimumLength = 0)]
		public string ComparteContenedorPicking { get; set; }
        /// <summary>
        /// Fecha Vencimiento
        /// </summary>
        /// <example>10/10/2025</example>
        public DateTime? FechaVencimiento { get; set; }
    }

    public class DetallePickingFinalizadoRequest : IApiEntradaItemRequest
    {

        public string CodigoProducto { get; set; }
        public decimal CantidadSolicitada { get; set; }
        public decimal CantidadPreparada { get; set; }
    }
}
